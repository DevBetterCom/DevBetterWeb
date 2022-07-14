using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Areas.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web;

public class Program
{
  public static async Task Main(string[] args)
  {
    var builder = CreateHostBuilder(args);
    var host = builder.Build();
    await SeedDatabase(host);
    host.Run();
  }

  private static async Task SeedDatabase(IHost host)
  {
    using (var scope = host.Services.CreateScope())
    {
      var services = scope.ServiceProvider;
      var logger = services.GetRequiredService<ILogger<Program>>();
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
      logger.LogInformation($"Current environment: {environment}");
      if (environment == "Production")
      {
	      var context = services.GetRequiredService<AppDbContext>();
	      SeedData.PopulateInitData(context);
				return;
      }

      logger.LogInformation("Seeding database...");
      try
      {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        if (userManager.Users.Any() || roleManager.Roles.Any())
        {
          logger.LogDebug("User/Role data already exists.");
        }
        else
        {
          await AppIdentityDbContextSeed.SeedAsync(userManager, roleManager);
          logger.LogDebug("Populated AppIdentityDbContext test data.");
        }
        var context = services.GetRequiredService<AppDbContext>();
        if (await context.Questions!.AnyAsync())
        {
          logger.LogDebug("Database already has data in it.");
        }
        else
        {
          SeedData.PopulateTestData(context, userManager);
          logger.LogDebug("Populated AppDbContext test data.");
        }
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An error occurred while seeding the database.");
      }
      finally
      {
        logger.LogInformation("Finished seeding database...");
      }
    }
  }

  public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .UseServiceProviderFactory(new AutofacServiceProviderFactory())
          .ConfigureWebHostDefaults(webBuilder =>
          {
            webBuilder.ConfigureKestrel(serverOptions =>
                {
                  serverOptions.Limits.MaxRequestBodySize = Constants.MAX_UPLOAD_FILE_SIZE; // 500MB
                  })
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                  logging.ClearProviders();
                  logging.AddConsole();
                  logging.AddAzureWebAppDiagnostics();
                });
          });
}
