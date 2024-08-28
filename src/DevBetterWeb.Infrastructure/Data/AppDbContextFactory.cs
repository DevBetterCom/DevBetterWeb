using System.IO;
using DevBetterWeb.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DevBetterWeb.Infrastructure.Data;
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json")
			.Build();

		var connectionString = configuration.GetConnectionString(Constants.DEFAULT_CONNECTION_STRING_NAME);

		var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
		optionsBuilder.UseSqlServer(connectionString);

		return new AppDbContext(optionsBuilder.Options);
	}
}
