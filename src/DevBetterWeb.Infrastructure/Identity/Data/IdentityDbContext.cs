using DevBetterWeb.Infrastructure.Identity.Data;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Web.Models;

public class IdentityDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
  public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IOptions<OperationalStoreOptions> operationalStoreOptions)
      : base(options, operationalStoreOptions)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
    // Customize the ASP.NET Identity model and override the defaults if needed.
    // For example, you can rename the ASP.NET Identity table names and more.
    // Add your customizations after calling base.OnModelCreating(builder);




  }
}
