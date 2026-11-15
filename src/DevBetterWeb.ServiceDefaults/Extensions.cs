using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

// Adds common Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// To learn more about using this project, see https://aka.ms/aspire/service-defaults
public static class Extensions
{
	public const string HealthEndpointPath = "/health";
	public const string AlivenessEndpointPath = "/alive";
	public const string OtlpEndpointConfigKey = "OTEL_EXPORTER_OTLP_ENDPOINT";

	public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
	{
		builder.ConfigureOpenTelemetry();

		builder.AddDefaultHealthChecks();

		builder.Services.AddServiceDiscovery();

		builder.Services.ConfigureHttpClientDefaults(http =>
		{
			// Turn on resilience by default, but never retry non-idempotent requests
			// (POST/PUT/PATCH/DELETE) such as payments, webhooks, and emails.
			http.AddStandardResilienceHandler(options => options.Retry.DisableForUnsafeHttpMethods());

			// Turn on service discovery by default
			http.AddServiceDiscovery();
		});

		return builder;
	}

	public static bool IsOtlpExporterEnabled(this IHostApplicationBuilder builder) =>
		!string.IsNullOrWhiteSpace(builder.Configuration[OtlpEndpointConfigKey]);

	public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
	{
		// Only wire up OpenTelemetry when an OTLP endpoint is configured (e.g. under the Aspire AppHost).
		// Application Insights 3.x is itself built on OpenTelemetry, so registering instrumentation
		// unconditionally would change what production sends to Azure Monitor (extra metrics, and
		// health check requests filtered out of request telemetry).
		if (!builder.IsOtlpExporterEnabled())
		{
			return builder;
		}

		builder.Logging.AddOpenTelemetry(logging =>
		{
			logging.IncludeFormattedMessage = true;
			logging.IncludeScopes = true;
		});

		builder.Services.AddOpenTelemetry()
			.WithMetrics(metrics =>
			{
				metrics.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation()
					.AddRuntimeInstrumentation();
			})
			.WithTracing(tracing =>
			{
				tracing.AddSource(builder.Environment.ApplicationName)
					.AddAspNetCoreInstrumentation(tracing =>
						// Exclude health check requests from tracing
						tracing.Filter = context =>
							!context.Request.Path.StartsWithSegments(HealthEndpointPath)
							&& !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
					)
					.AddHttpClientInstrumentation();
			})
			.UseOtlpExporter();

		return builder;
	}

	public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
	{
		builder.Services.AddHealthChecks()
			// Add a default liveness check to ensure app is responsive
			.AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

		return builder;
	}

	public static WebApplication MapDefaultEndpoints(this WebApplication app)
	{
		// Unlike the Aspire template, these are mapped in every environment because the
		// production site already exposes /health for its hosting platform's probes.
		// Only the default (non-detailed) response writer is used, so no check details leak.

		// All health checks must pass for app to be considered ready to accept traffic after starting
		app.MapHealthChecks(HealthEndpointPath);

		// Only health checks tagged with the "live" tag must pass for app to be considered alive
		app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
		{
			Predicate = r => r.Tags.Contains("live")
		});

		return app;
	}
}
