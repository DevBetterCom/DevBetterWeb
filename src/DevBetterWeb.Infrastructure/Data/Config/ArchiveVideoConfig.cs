using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config
{
    public class ArchiveVideoConfig : IEntityTypeConfiguration<ArchiveVideo>
    {
        public void Configure(EntityTypeBuilder<ArchiveVideo> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(200);
            builder.Property(x => x.VideoUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
        }
    }
}
