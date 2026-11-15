using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web;

public static class MigrationServiceExtensions {

	/// <summary>
	/// Local migrations run when the app is in a docker-compose container or when the Aspire
	/// AppHost sets the <c>ApplyLocalMigrations</c> flag. They still only run in Development.
	/// </summary>
	public static bool ShouldApplyLocalMigrations(bool runningInContainer, bool applyLocalMigrationsFlag) =>
		runningInContainer || applyLocalMigrationsFlag;

	public static async Task ApplyLocalMigrationAsync<TContext>(
		this ILocalMigrationService<TContext> migrationService,
		string                                environmentName,
		bool shouldApplyMigrations)
		where TContext : DbContext
	{
		if (environmentName is not "Development"
		    || !shouldApplyMigrations)
		{
			return;
		}

		await migrationService.ApplyDatabaseMigrationsAsync();
	}
}
