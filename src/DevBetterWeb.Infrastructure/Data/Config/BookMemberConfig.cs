using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config
{
    public class BookMemberConfig : IEntityTypeConfiguration<BookMember>
    {

        public void Configure(EntityTypeBuilder<BookMember> builder)
        {
            builder.HasKey(x => new { x.BookId, x.MemberId });

            builder.HasOne(bookmember => bookmember.Book)
                .WithMany("BookMembers")
                .HasForeignKey(b => b.BookId);

            builder.HasOne(bookmember => bookmember.Member)
                .WithMany("BookMembers")
                .HasForeignKey(m => m.MemberId);
        }
    }
}
