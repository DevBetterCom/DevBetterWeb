using Autofac.Extensions.DependencyInjection;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Web.Areas.Identity;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = CreateHostBuilder(args);

            //string env = "Development";
            //if (args.Any())
            //{
            //    env = args[0];
            //}
            //Console.WriteLine($"Starting using environment: {env}");
            //builder.UseEnvironment(env);
            var host = builder.Build();

            //if (env == "Development")
            //{
            //    await SeedDatabase(host);
            //}

            host.Run();
        }

        private static async Task SeedDatabase(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Seeding database...");
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    if (context.Questions.Any())
                    {
                        logger.LogDebug("Database already has data in it.");
                    }
                    else
                    {
                        SeedData.PopulateTestData(context);
                        logger.LogDebug("Populated AppDbContext test data.");
                    }

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
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the database.");
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
                    .UseStartup<Startup>();
                });
    }
}
