using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class CoachingSessionConfig : IEntityTypeConfiguration<CoachingSession>
{
  public void Configure(EntityTypeBuilder<CoachingSession> builder)
  {
	  builder
		  .ToTable("CoachingSessions")
		  .Ignore(x => x.IsActive);

	  builder
		  .HasMany(t => t.Questions)
		  .WithOne(p => p.CoachingSession)
		  .HasForeignKey(x => x.CoachingSessionId)
		  .OnDelete(DeleteBehavior.Cascade)
		  .IsRequired();
  }
}
