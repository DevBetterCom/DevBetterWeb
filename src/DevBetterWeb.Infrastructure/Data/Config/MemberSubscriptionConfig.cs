using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

class MemberSubscriptionConfig : IEntityTypeConfiguration<MemberSubscription>
{
  public void Configure(EntityTypeBuilder<MemberSubscription> builder)
  {
    builder
	    .ComplexProperty(x => x.Dates);
	}
}
