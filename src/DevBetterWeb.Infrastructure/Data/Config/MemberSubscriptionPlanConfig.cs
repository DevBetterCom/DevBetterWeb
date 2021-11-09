using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class MemberSubscriptionPlanConfig : IEntityTypeConfiguration<MemberSubscriptionPlan>
{
  public void Configure(EntityTypeBuilder<MemberSubscriptionPlan> builder)
  {
    builder.OwnsOne(x => x.Details, d =>
    {
      d.Property(p => p!.Name).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
      d.Property(p => p!.PricePerBillingPeriod).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
      d.Property(p => p!.BillingPeriod).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
    });
  }
}
