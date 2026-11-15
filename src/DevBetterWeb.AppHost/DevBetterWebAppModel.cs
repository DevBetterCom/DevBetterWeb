using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace DevBetterWeb.AppHost;

public static class DevBetterWebAppModel
{
	public const string SqlServerResourceName = "sql";
	public const string DatabaseResourceName = "devbetterdb";
	public const string DatabaseName = "DevBetterWeb";
	public const string WebResourceName = "web";

	// Matches DevBetterWeb.Core.Constants.DEFAULT_CONNECTION_STRING_NAME so the web project
	// picks up ConnectionStrings:DefaultConnection without any code changes.
	public const string ConnectionStringName = "DefaultConnection";

	public const string ApplyLocalMigrationsKey = "ApplyLocalMigrations";

	public static IDistributedApplicationBuilder AddDevBetterWeb(this IDistributedApplicationBuilder builder)
	{
		var sql = builder.AddSqlServer(SqlServerResourceName)
			.WithDataVolume()
			.WithLifetime(ContainerLifetime.Persistent);

		var database = sql.AddDatabase(DatabaseResourceName, DatabaseName);

		builder.AddProject<Projects.DevBetterWeb_Web>(WebResourceName, launchProfileName: "DevBetterWeb.Web - DEV")
			.WithReference(database, ConnectionStringName)
			.WaitFor(database)
			.WithEnvironment(ApplyLocalMigrationsKey, "true")
			.WithHttpHealthCheck("/health");

		return builder;
	}
}
