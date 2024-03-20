using System.Threading.Tasks;
using DevBetterWeb.Web;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Extensions;

public class MigrationServiceExtensionsTests
{
	[Theory]
	[InlineData("Development", true, 1)]
	[InlineData("Development", false, 0)]
	[InlineData("Prod", true, 0)]
	public async Task OnlyMigratesWhenEnvironmentIsDevelopment(
		string environment,
		bool runningInContainer,
		int expectedRuns)
	{
		var migrationService = Substitute.For<ILocalMigrationService<DbContext>>();
		await migrationService.ApplyLocalMigrationAsync(environment, runningInContainer);

		await migrationService.Received(expectedRuns).ApplyDatabaseMigrationsAsync();
	}
}
