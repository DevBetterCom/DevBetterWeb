using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web;

public static class ServiceProviderExtensions
{
	public static async Task ApplyDatabaseMigrationsAsync<TContext>(this IServiceScope scope)
		where TContext : DbContext
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
		var logger    = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

		if (await VerifyNoPendingMigrationsAsync(dbContext))
		{
			logger.LogInformation($"No pending migrations to apply for context: {typeof(TContext).Name}");
			return;
		}

		logger.LogInformation($"Applying pending migrations for context: {typeof(TContext).Name}...");
		await dbContext.Database.MigrateAsync();
		logger.LogInformation($"Migrations applied successfully for context: {typeof(TContext).Name}");
	}

	private static async Task<bool> VerifyNoPendingMigrationsAsync<TContext>(TContext dbContext) 
		where TContext : DbContext =>
		!(await dbContext.Database.GetPendingMigrationsAsync()).Any();
}
