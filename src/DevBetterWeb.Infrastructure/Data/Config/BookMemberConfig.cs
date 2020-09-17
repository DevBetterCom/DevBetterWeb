using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config
{
    public class BookMemberConfig : IEntityTypeConfiguration<BookMember>
    {

        public void Configure(EntityTypeBuilder<BookMember> builder)
        {
            builder.HasOne(x => x.Member);
            builder.HasOne(x => x.Book);
        }
    }
}
