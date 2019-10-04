using DevBetterWeb.Web.Areas.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: HostingStartup(typeof(DevBetterWeb.Web.Areas.Identity.IdentityHostingStartup))]
namespace DevBetterWeb.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        //private readonly ILogger<IdentityHostingStartup> _logger;

        //public IdentityHostingStartup(ILogger<IdentityHostingStartup> logger)
        //{
        //    _logger = logger;
        //}
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                
                if (context.HostingEnvironment.EnvironmentName == "Production")
                {
                    //_logger.LogInformation("Configuring real SQL SERVER for IDENTITY");
                    services.AddDbContext<IdentityDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("ProductionConnectionString")));
                }
                else
                {
                    //_logger.LogInformation("Configuring Local DB for IDENTITY");
                    services.AddDbContext<IdentityDbContext>(options =>
                        options.UseSqlServer(
                            context.Configuration.GetConnectionString("LocalDbConnectionString")));
                }

                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<IdentityDbContext>()
                    .AddDefaultUI(UIFramework.Bootstrap4)
                    .AddDefaultTokenProviders();

                services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,
                    UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();
            });
        }
    }
}
