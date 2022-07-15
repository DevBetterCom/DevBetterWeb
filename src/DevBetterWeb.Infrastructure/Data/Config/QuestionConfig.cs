﻿using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class QuestionConfig : IEntityTypeConfiguration<Question>
{
  public void Configure(EntityTypeBuilder<Question> builder)
  {
    builder
	    .Property(x => x.QuestionText)
	    .HasMaxLength(500);

    builder
	    .ToTable("Questions");

    builder
	    .HasOne(t => t.MemberWhoCreate)
	    .WithMany(p => p.Questions)
	    .HasForeignKey(d => d.MemberId)
	    .OnDelete(DeleteBehavior.ClientSetNull);
	}
}
