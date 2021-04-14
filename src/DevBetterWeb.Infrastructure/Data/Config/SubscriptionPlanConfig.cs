using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config
{
  public class SubscriptionPlanConfig : IEntityTypeConfiguration<SubscriptionPlan>
  {
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
      builder.OwnsOne(x => x.Details, d =>
      {
        d.Property(p => p!.Name).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
        d.Property(p => p!.PricePerBillingPeriod).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
        d.Property(p => p!.BillingPeriod).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
      });
    }
  }
}
