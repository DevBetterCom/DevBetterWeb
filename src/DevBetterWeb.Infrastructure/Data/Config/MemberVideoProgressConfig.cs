using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class MemberVideoProgressConfig : IEntityTypeConfiguration<MemberVideoProgress>
{
  public void Configure(EntityTypeBuilder<MemberVideoProgress> builder)
  {
    builder
	    .ToTable("MembersVideosProgress")
	    .HasKey(x => x.Id);

    builder
		.Property(t => t.VideoWatchedStatus)
		.HasColumnName("VideoWatchedStatus")
		.HasMaxLength(DataConfigConstants.TYPE_ONE_CHAR_WIDTH)
		.HasConversion<VideoWatchedStatusConverter>()
		.IsRequired();

    builder
	    .HasOne(t => t.Video)
	    .WithMany(p => p.MembersVideoProgress)
	    .HasForeignKey(d => d.ArchiveVideoId)
	    .OnDelete(DeleteBehavior.ClientSetNull);

    builder
	    .HasOne(t => t.Member)
	    .WithMany(p => p.MemberVideosProgress)
	    .HasForeignKey(d => d.MemberId)
	    .OnDelete(DeleteBehavior.ClientSetNull);
	}
}
