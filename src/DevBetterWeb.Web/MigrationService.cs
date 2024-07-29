using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web;

public interface ILocalMigrationService<TContext> where TContext : DbContext
{
	Task ApplyDatabaseMigrationsAsync();
}

public class MigrationService<TContext> : ILocalMigrationService<TContext> 
	where TContext : DbContext
{
	private readonly ILogger<MigrationService<TContext>> _logger;

	private readonly TContext _dbContext;

	public MigrationService(
		TContext            context,
		ILoggerFactory      loggerFactory)
	{
		_logger       = loggerFactory.CreateLogger<MigrationService<TContext>>();
		_dbContext    = context;
	}

	public async Task ApplyDatabaseMigrationsAsync()
	{
		if (await NoPendingMigrationsAsync(_dbContext))
		{
			_logger.LogInformation($"No pending migrations to apply for context: {typeof(TContext).Name}");
			return;
		}

		_logger.LogInformation($"Applying pending migrations for context: {typeof(TContext).Name}...");
		await _dbContext.Database.MigrateAsync();
		_logger.LogInformation($"Migrations applied successfully for context: {typeof(TContext).Name}");
	}

	private static async Task<bool> NoPendingMigrationsAsync(TContext dbContext) =>
		!(await dbContext.Database.GetPendingMigrationsAsync()).Any();
}
