using System.Threading.Tasks;
using DevBetterWeb.Web;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Extensions;

public class MigrationServiceExtensionsTests
{
	[Theory]
	[InlineData("Local", 1)]
	[InlineData("Dev", 0)]
	[InlineData("Prod", 0)]
	public async Task OnlyMigratesWhenEnvironmentIsLocal(string environment, int expectedRuns)
	{
		var migrationService = Substitute.For<ILocalMigrationService<DbContext>>();
		await migrationService.ApplyLocalMigrationAsync(environment);

		await migrationService.Received(expectedRuns).ApplyDatabaseMigrationsAsync();
	}
}
