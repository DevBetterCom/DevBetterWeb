using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config
{
    public class MemberConfig : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(x => x.UserId).HasMaxLength(500);
            builder.Property(x => x.Address).HasMaxLength(500);
            builder.Property(x => x.BlogUrl).HasMaxLength(200);
            builder.Property(x => x.GithubUrl).HasMaxLength(200);
            builder.Property(x => x.LinkedInUrl).HasMaxLength(200);
            builder.Property(x => x.OtherUrl).HasMaxLength(200);
            builder.Property(x => x.TwitchUrl).HasMaxLength(200);
            builder.Property(x => x.TwitterUrl).HasMaxLength(200);
            builder.Property(x => x.FirstName).HasMaxLength(100);
            builder.Property(x => x.LastName).HasMaxLength(100);
        }
    }
}
