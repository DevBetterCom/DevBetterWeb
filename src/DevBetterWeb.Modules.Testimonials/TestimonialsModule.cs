using DevBetterWeb.Modules.Testimonials.Application;
using DevBetterWeb.Modules.Testimonials.Contracts;
using DevBetterWeb.Modules.Testimonials.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DevBetterWeb.Modules.Testimonials;

/// <summary>
/// Public entry points of the Testimonials module. Everything else in the module is internal.
/// </summary>
public static class TestimonialsModule
{
	/// <summary>
	/// Registers the module's services.
	/// </summary>
	/// <param name="configureDbContext">
	/// Configures the module's DbContext provider, typically via <see cref="UseTestimonialsSqlServer"/>.
	/// Pass null to skip DbContext registration (e.g. when a test host registers its own provider).
	/// </param>
	public static IServiceCollection AddTestimonialsModule(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder>? configureDbContext)
	{
		if (configureDbContext is not null)
		{
			services.AddDbContext<TestimonialsDbContext>(configureDbContext);
		}

		services.TryAddSingleton(TimeProvider.System);
		services.TryAddScoped<ITestimonialQueries, TestimonialQueries>();
		services.TryAddScoped<ITestimonialAuthoring, TestimonialAuthoring>();
		return services;
	}

	/// <summary>
	/// Configures SQL Server for the module, keeping its migrations and history table in the module's schema.
	/// </summary>
	public static DbContextOptionsBuilder UseTestimonialsSqlServer(this DbContextOptionsBuilder optionsBuilder, string? connectionString)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

		return optionsBuilder.UseSqlServer(connectionString, sql => sql
			.MigrationsAssembly(typeof(TestimonialsDbContext).Assembly.GetName().Name)
			.MigrationsHistoryTable("__EFMigrationsHistory", TestimonialsDbContext.Schema));
	}

	/// <summary>
	/// Applies any pending migrations for the module's DbContext.
	/// </summary>
	/// <param name="services">A scoped service provider (the DbContext is scoped).</param>
	public static async Task ApplyTestimonialsMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
	{
		var dbContext = services.GetRequiredService<TestimonialsDbContext>();
		if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
		{
			await dbContext.Database.MigrateAsync(cancellationToken);
		}
	}
}
