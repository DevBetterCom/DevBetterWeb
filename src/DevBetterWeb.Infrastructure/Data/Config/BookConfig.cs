using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {

        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(500);
            builder.Property(x => x.Author).HasMaxLength(DataConfigConstants.NAME_COLUMN_WIDTH);
            builder.Property(x => x.Details).HasMaxLength(1000);
            builder.Property(x => x.PurchaseUrl).HasMaxLength(DataConfigConstants.URL_COLUMN_WIDTH);
        }
    }
}
