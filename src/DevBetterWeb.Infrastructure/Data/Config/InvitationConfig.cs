using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config
{
  public class InvitationConfig : IEntityTypeConfiguration<Invitation>
  {
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
      builder.Property(i => i.Email).HasMaxLength(200);
      builder.Property(i => i.InviteCode).HasMaxLength(500);
      builder.Property(i => i.PaymentHandlerSubscriptionId).HasMaxLength(500);
      builder.Property(i => i.PaymentHandlerSubscriptionId).HasMaxLength(500);
      builder.Property(i => i.DateCreated);
      builder.Property(i => i.DateOfUserPing);
      builder.Property(i => i.DateOfLastAdminPing);
    }
  }
}
