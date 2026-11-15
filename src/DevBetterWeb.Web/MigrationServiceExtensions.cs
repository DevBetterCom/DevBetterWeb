using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web;

public static class MigrationServiceExtensions {

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
