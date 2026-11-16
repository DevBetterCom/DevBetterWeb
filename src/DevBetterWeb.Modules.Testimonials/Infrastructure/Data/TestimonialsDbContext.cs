using DevBetterWeb.Modules.Testimonials.Domain;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Modules.Testimonials.Infrastructure.Data;

/// <summary>
/// Module-owned DbContext. Shares the physical database but owns the <c>testimonials</c> schema
/// and keeps its own migrations history table there.
/// </summary>
internal sealed class TestimonialsDbContext : DbContext
{
	public const string Schema = "testimonials";

	public TestimonialsDbContext(DbContextOptions<TestimonialsDbContext> options)
		: base(options)
	{
	}

	public DbSet<Testimonial> Testimonials => Set<Testimonial>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.HasDefaultSchema(Schema);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(TestimonialsDbContext).Assembly);
	}
}
