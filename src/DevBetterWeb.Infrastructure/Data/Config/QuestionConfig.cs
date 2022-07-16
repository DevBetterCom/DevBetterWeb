﻿using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class QuestionConfig : IEntityTypeConfiguration<Question>
{
  public void Configure(EntityTypeBuilder<Question> builder)
  {
	  builder
		  .ToTable("Questions");

		builder
		  .Property(x => x.CoachingSessionId)
		  .IsRequired();

		builder
	    .Property(x => x.QuestionText)
	    .HasMaxLength(500)
	    .IsRequired();

		builder
	    .HasOne(t => t.MemberWhoCreate)
	    .WithMany(p => p.Questions)
	    .HasForeignKey(d => d.MemberId)
	    .OnDelete(DeleteBehavior.ClientSetNull);

    builder
	    .HasMany(t => t.QuestionVotes)
	    .WithOne(p => p.Question)
	    .HasForeignKey(x => x.QuestionId)
	    .OnDelete(DeleteBehavior.Cascade);
	}
}
