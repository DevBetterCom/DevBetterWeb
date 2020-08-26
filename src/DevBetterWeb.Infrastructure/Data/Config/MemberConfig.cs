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
            builder.Property(x => x.BlogUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
            builder.Property(x => x.GitHubUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
            builder.Property(x => x.LinkedInUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
            builder.Property(x => x.OtherUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
            builder.Property(x => x.TwitchUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
            builder.Property(x => x.TwitterUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
            builder.Property(x => x.YouTubeUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
            builder.Property(x => x.FirstName).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
            builder.Property(x => x.LastName).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
            builder.Property(x => x.PEFriendCode).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
            builder.Property(x => x.PEUsername).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
        }
    }
}
