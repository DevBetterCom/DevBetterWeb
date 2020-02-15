using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Models
{
    public class IdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().Property(x => x.Address).HasMaxLength(500);
            builder.Entity<ApplicationUser>().Property(x => x.BlogUrl).HasMaxLength(200);
            builder.Entity<ApplicationUser>().Property(x => x.GithubUrl).HasMaxLength(200);
            builder.Entity<ApplicationUser>().Property(x => x.LinkedInUrl).HasMaxLength(200);
            builder.Entity<ApplicationUser>().Property(x => x.OtherUrl).HasMaxLength(200);
            builder.Entity<ApplicationUser>().Property(x => x.TwitchUrl).HasMaxLength(200);
            builder.Entity<ApplicationUser>().Property(x => x.TwitterUrl).HasMaxLength(200);


        }
    }
}
