using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class MemberAddressHistoryConfig : IEntityTypeConfiguration<MemberAddressHistory>
{
  public void Configure(EntityTypeBuilder<MemberAddressHistory> builder)
  {
    builder.Property(i => i.MemberId).IsRequired();
    builder.Property(i => i.UpdatedOn).IsRequired();

    builder.OwnsOne(x => x.Address, sa =>
    {
	    sa.Property(p => p!.Street).HasMaxLength(500).HasDefaultValue("");
	    sa.Property(p => p!.City).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH).HasDefaultValue("");
	    sa.Property(p => p!.State).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH).HasDefaultValue("");
	    sa.Property(p => p!.PostalCode).HasMaxLength(12).HasDefaultValue("");
	    sa.Property(p => p!.Country).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH).HasDefaultValue("");
    });
	}
}
