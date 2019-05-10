using CleanArchitecture.Infrastructure.Data;
using DevBetterWeb.Web;
using DevBetterWeb.Web.Areas.Identity;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace CleanArchitecture.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateWebHostBuilder(args);

            string env = "Development";
            if(args.Any())
            {
                env = args[0];
            }
            builder.UseEnvironment(env);
            var host = builder.Build();


            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    SeedData.PopulateTestData(context);

                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    AppIdentityDbContextSeed.SeedAsync(userManager, roleManager).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = Constants.MAX_UPLOAD_FILE_SIZE; //500MB
                });
        }
}
