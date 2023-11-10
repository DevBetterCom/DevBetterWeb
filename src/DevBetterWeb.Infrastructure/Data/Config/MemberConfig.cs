using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class MemberConfig : IEntityTypeConfiguration<Member>
{

  public void Configure(EntityTypeBuilder<Member> builder)
  {
	  builder.Property(x => x.UserId).HasMaxLength(500);
    builder.Property(x => x.Address).HasMaxLength(500);
    builder.Property(x => x.BlogUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
    builder.Property(x => x.CodinGameUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
    builder.Property(x => x.GitHubUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
    builder.Property(x => x.LinkedInUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
    builder.Property(x => x.OtherUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
    builder.Property(x => x.TwitchUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
		builder.Property(x => x.MastodonUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
    builder.Property(x => x.TwitterUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
    builder.Property(x => x.BlueskyUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
    builder.Property(x => x.YouTubeUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
    builder.Property(x => x.FirstName).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
    builder.Property(x => x.LastName).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
    builder.Property(x => x.PEFriendCode).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
    builder.Property(x => x.PEUsername).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
    builder.Property(x => x.DiscordUsername).HasMaxLength(200);

    builder.OwnsOne(x => x.ShippingAddress, sa =>
    {
      sa.Property(p => p!.Street).HasMaxLength(500).HasDefaultValue("");
      sa.Property(p => p!.City).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH).HasDefaultValue("");
      sa.Property(p => p!.State).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH).HasDefaultValue("");
      sa.Property(p => p!.PostalCode).HasMaxLength(12).HasDefaultValue("");
      sa.Property(p => p!.Country).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH).HasDefaultValue("");
    });

    builder.OwnsOne(x => x.CityLocation, cl =>
    {
      cl.Property(p => p!.Latitude).HasDefaultValue(null).HasColumnType("decimal(18,4)");
      cl.Property(p => p!.Longitude).HasDefaultValue(null).HasColumnType("decimal(18,4)");
    });

    builder.OwnsOne(x => x.Birthday, bd =>
    {
      bd.Property(p => p!.Day).HasDefaultValue(null);
      bd.Property(p => p!.Month).HasDefaultValue(null);
    });

    builder.HasMany(x => x.FavoriteArchiveVideos);
  }
}
