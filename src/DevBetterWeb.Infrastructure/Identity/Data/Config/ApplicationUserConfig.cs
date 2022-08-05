using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Identity.Data.Config;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
  public void Configure(EntityTypeBuilder<ApplicationUser> builder)
  {
    builder
	    .Property(x => x.DateCreated);
  }
}
