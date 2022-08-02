using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class VideoCommentConfig : IEntityTypeConfiguration<VideoComment>
{

  public void Configure(EntityTypeBuilder<VideoComment> builder)
  {
	  builder
		  .Ignore(x => x.MdBody);

		builder
	    .Property(x => x.Body)
	    .HasMaxLength(2000);

    builder
	    .HasOne(t => t.Video)
	    .WithMany(p => p.Comments)
	    .HasForeignKey(d => d.VideoId)
	    .OnDelete(DeleteBehavior.ClientSetNull);

    builder
	    .HasOne(t => t.MemberWhoCreate)
	    .WithMany(p => p.VideosComments)
	    .HasForeignKey(d => d.MemberId)
	    .OnDelete(DeleteBehavior.ClientSetNull);

    builder
	    .HasOne(t => t.ParentComment)
	    .WithMany(p => p.Replies)
	    .HasForeignKey(d => d.ParentCommentId)
	    .OnDelete(DeleteBehavior.ClientSetNull);
	}
}
