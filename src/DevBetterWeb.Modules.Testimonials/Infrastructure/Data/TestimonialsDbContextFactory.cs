using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DevBetterWeb.Modules.Testimonials.Infrastructure.Data;

/// <summary>
/// Lets <c>dotnet ef</c> create migrations for this module without the web host.
/// </summary>
internal sealed class TestimonialsDbContextFactory : IDesignTimeDbContextFactory<TestimonialsDbContext>
{
	public TestimonialsDbContext CreateDbContext(string[] args)
	{
		const string localConnectionString = @"Server=(localdb)\mssqllocaldb;TrustServerCertificate=true;Database=DevBetterWeb.App;Trusted_Connection=True;MultipleActiveResultSets=true";

		var optionsBuilder = new DbContextOptionsBuilder<TestimonialsDbContext>();
		optionsBuilder.UseTestimonialsSqlServer(localConnectionString);
		return new TestimonialsDbContext(optionsBuilder.Options);
	}
}
