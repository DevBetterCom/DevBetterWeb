using DevBetterWeb.Modules.Testimonials.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBetterWeb.Modules.Testimonials.Infrastructure.Data.Config;

internal sealed class TestimonialConfig : IEntityTypeConfiguration<Testimonial>
{
	public void Configure(EntityTypeBuilder<Testimonial> builder)
	{
		builder.ToTable("Testimonials");
		builder.HasKey(t => t.Id);

		builder.Property(t => t.MemberId).IsRequired();
		builder.Property(t => t.Content).IsRequired().HasMaxLength(Testimonial.MaxContentLength);
		builder.Property(t => t.Status).IsRequired().HasConversion<string>().HasMaxLength(32);
		builder.Property(t => t.ReviewerFeedback).HasMaxLength(Testimonial.MaxFeedbackLength);
		builder.Property(t => t.RowVersion).IsRowVersion();

		builder.OwnsOne(t => t.Author, author =>
		{
			author.Property(a => a.DisplayName).HasColumnName("AuthorDisplayName").IsRequired().HasMaxLength(AuthorProfile.MaxDisplayNameLength);
			author.Property(a => a.Title).HasColumnName("AuthorTitle").HasMaxLength(AuthorProfile.MaxTitleLength);
			author.Property(a => a.Organization).HasColumnName("AuthorOrganization").HasMaxLength(AuthorProfile.MaxOrganizationLength);
			author.Property(a => a.PhotoUrl).HasColumnName("AuthorPhotoUrl").HasMaxLength(AuthorProfile.MaxPhotoUrlLength);
		});
		builder.Navigation(t => t.Author).IsRequired();

		builder.Ignore(t => t.CanBeEditedByAuthor);
		builder.Ignore(t => t.CanBeSubmitted);
		builder.Ignore(t => t.CanBeDeletedByAuthor);

		builder.HasIndex(t => t.MemberId);
		builder.HasIndex(t => new { t.Status, t.ReviewedAt });
	}
}
