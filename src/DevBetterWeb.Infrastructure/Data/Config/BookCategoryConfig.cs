using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Infrastructure.Data.Config;

public class BookCategoryConfig : IEntityTypeConfiguration<BookCategory>
{

  public void Configure(EntityTypeBuilder<BookCategory> builder)
  {
    builder
	    .Property(x => x.Title)
	    .HasMaxLength(500);


  }
}
