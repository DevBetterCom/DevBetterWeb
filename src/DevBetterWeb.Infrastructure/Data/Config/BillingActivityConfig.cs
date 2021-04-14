using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config
{
  public class BillingActivityConfig : IEntityTypeConfiguration<BillingActivity>
  {
    public void Configure(EntityTypeBuilder<BillingActivity> builder)
    {
      builder.Property(x => x.MemberId).HasMaxLength(500);
      builder.OwnsOne(x => x.Details, d =>
      {
        d.Property(p => p!.Amount).HasDefaultValue(0);
        d.Property(p => p!.ActionVerbPastTense).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
        d.Property(p => p!.MemberName).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
        d.Property(p => p!.SubscriptionPlanName).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
        d.Property(p => p!.BillingPeriod).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
        d.Property(p => p!.Date);
      });
    }
  }
}
