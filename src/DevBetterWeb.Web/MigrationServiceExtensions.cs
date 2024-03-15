using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web;

public static class MigrationServiceExtensions {

	public static async Task ApplyLocalMigrationAsync<TContext>(
		this ILocalMigrationService<TContext> migrationService,
		string                                environmentName)
		where TContext : DbContext
	{
		if (environmentName is not "Local")
		{
			return;
		}

		await migrationService.ApplyDatabaseMigrationsAsync();
	}
}
