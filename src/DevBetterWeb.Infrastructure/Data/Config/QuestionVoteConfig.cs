using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class QuestionVoteConfig : IEntityTypeConfiguration<QuestionVote>
{
  public void Configure(EntityTypeBuilder<QuestionVote> builder)
  {
    builder
	    .Property(x => x.QuestionId)
	    .IsRequired();

    builder
	    .Property(x => x.MemberId)
	    .IsRequired();

    builder
	    .HasOne(t => t.Member)
	    .WithMany(p => p.QuestionVotes)
	    .HasForeignKey(d => d.MemberId)
	    .OnDelete(DeleteBehavior.ClientSetNull);
  }
}
