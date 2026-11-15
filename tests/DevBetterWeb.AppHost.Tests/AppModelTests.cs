using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using DevBetterWeb.AppHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace DevBetterWeb.AppHost.Tests;

// These tests build the Aspire application model without starting it, so they do not need
// Docker/Podman and are safe to run in CI.
public class AppModelTests
{
	private static async Task<DistributedApplicationModel> BuildModelAsync()
	{
		var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.DevBetterWeb_AppHost>();
		var app = await appHost.BuildAsync();
		return app.Services.GetRequiredService<DistributedApplicationModel>();
	}

	[Fact]
	public async Task DefinesSqlServerWithPersistentDataVolume()
	{
		var model = await BuildModelAsync();

		var sql = model.Resources.OfType<SqlServerServerResource>()
			.Single(r => r.Name == DevBetterWebAppModel.SqlServerResourceName);

		sql.Annotations.OfType<ContainerMountAnnotation>()
			.ShouldContain(m => m.Type == ContainerMountType.Volume);
		sql.Annotations.OfType<ContainerLifetimeAnnotation>()
			.ShouldContain(l => l.Lifetime == ContainerLifetime.Persistent);
	}

	[Fact]
	public async Task DefinesDevBetterWebDatabase()
	{
		var model = await BuildModelAsync();

		var database = model.Resources.OfType<SqlServerDatabaseResource>()
			.Single(r => r.Name == DevBetterWebAppModel.DatabaseResourceName);

		database.DatabaseName.ShouldBe(DevBetterWebAppModel.DatabaseName);
		database.Parent.Name.ShouldBe(DevBetterWebAppModel.SqlServerResourceName);
	}

	[Fact]
	public async Task WebWaitsForDatabase()
	{
		var model = await BuildModelAsync();
		var web = GetWeb(model);

		web.Annotations.OfType<WaitAnnotation>()
			.ShouldContain(w => w.Resource.Name == DevBetterWebAppModel.DatabaseResourceName);
	}

	[Fact]
	public async Task WebReceivesDefaultConnectionStringAndMigrationFlag()
	{
		var model = await BuildModelAsync();
		var web = GetWeb(model);

		// Publish mode resolves connection strings to expressions, so no endpoints need allocating.
		var configuration = await ExecutionConfigurationBuilder.Create(web)
			.WithEnvironmentVariablesConfig()
			.BuildAsync(
				new DistributedApplicationExecutionContext(DistributedApplicationOperation.Publish),
				NullLogger.Instance,
				CancellationToken.None);
		var environment = configuration.EnvironmentVariables.ToDictionary(kv => kv.Key, kv => kv.Value);

		environment.Keys.ShouldContain($"ConnectionStrings__{DevBetterWebAppModel.ConnectionStringName}");
		environment[DevBetterWebAppModel.ApplyLocalMigrationsKey].ShouldBe("true");
	}

	private static ProjectResource GetWeb(DistributedApplicationModel model) =>
		model.Resources.OfType<ProjectResource>()
			.Single(r => r.Name == DevBetterWebAppModel.WebResourceName);
}
