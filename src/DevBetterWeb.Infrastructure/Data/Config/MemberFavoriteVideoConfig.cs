using DevBetterWeb.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class MemberFavoriteVideoConfig : IEntityTypeConfiguration<MemberFavoriteArchiveVideo>
{
  public void Configure(EntityTypeBuilder<MemberFavoriteArchiveVideo> builder)
  {
    builder.ToTable("MemberFavoriteArchiveVideos");
		builder.HasKey(x => new { x.MemberId, x.ArchiveVideoId });
  }
}
