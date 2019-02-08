using DevBetterWeb.Web.Areas.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(DevBetterWeb.Web.Areas.Identity.IdentityHostingStartup))]
namespace DevBetterWeb.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<IdentityDbContext>(options =>
                    options.UseInMemoryDatabase("Identity"));
                //options.UseSqlServer(
                //    context.Configuration.GetConnectionString("IdentityDbContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>()
                    .AddEntityFrameworkStores<IdentityDbContext>();
            });
        }
    }
}
