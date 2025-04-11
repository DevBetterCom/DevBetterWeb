using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NimblePros.Metronome;
using NimblePros.Vimeo.Models;

[assembly: HostingStartup(typeof(DevBetterWeb.Web.Areas.Identity.IdentityHostingStartup))]
namespace DevBetterWeb.Web.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
  public void Configure(IWebHostBuilder builder)
  {


		builder.ConfigureServices((context, services) =>
    {
			if (context.HostingEnvironment.IsEnvironment("Testing"))
			{
				return;
			}
			services.AddDbContext<IdentityDbContext>((provider, options) =>
                  options.UseSqlServer(
                      context.Configuration.GetConnectionString("DefaultConnection"))
														.AddMetronomeDbTracking(provider)
			);

      services.AddIdentity<ApplicationUser, IdentityRole>(x =>
                      {
                  x.SignIn.RequireConfirmedEmail = true;
                })
                  .AddEntityFrameworkStores<IdentityDbContext>()
                  .AddDefaultTokenProviders();

      services.ConfigureApplicationCookie(options =>
              {
          options.Cookie.Name = "DevBetterAuth";
          options.LoginPath = $"/Identity/Account/Login";
          options.LogoutPath = $"/Identity/Account/Logout";
          options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
        });

      services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,
                  UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();
    });
  }
}
