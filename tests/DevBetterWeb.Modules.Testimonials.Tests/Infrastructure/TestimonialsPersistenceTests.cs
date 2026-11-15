using DevBetterWeb.Modules.Testimonials.Application;
using DevBetterWeb.Modules.Testimonials.Contracts;
using DevBetterWeb.Modules.Testimonials.Domain;
using DevBetterWeb.Modules.Testimonials.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Modules.Testimonials.Tests.Infrastructure;

public class TestimonialsPersistenceTests
{
	private static TestimonialsDbContext CreateInMemoryContext(string databaseName)
	{
		var options = new DbContextOptionsBuilder<TestimonialsDbContext>()
			.UseInMemoryDatabase(databaseName)
			.Options;
		return new TestimonialsDbContext(options);
	}

	private static TestimonialsDbContext CreateSqlServerModelContext()
	{
		// No connection is opened; SQL Server is only used to build the relational model.
		var builder = new DbContextOptionsBuilder<TestimonialsDbContext>();
		builder.UseTestimonialsSqlServer("Server=unused;Database=unused;TrustServerCertificate=true");
		return new TestimonialsDbContext(builder.Options);
	}

	[Fact]
	public void ModelUsesTestimonialsSchema()
	{
		using var context = CreateSqlServerModelContext();

		var entityType = context.Model.FindEntityType(typeof(Testimonial))!;

		entityType.GetSchema().ShouldBe(TestimonialsDbContext.Schema);
		entityType.GetTableName().ShouldBe("Testimonials");
	}

	[Fact]
	public void MigrationsMatchTheModel()
	{
		using var context = CreateSqlServerModelContext();

		context.Database.GetMigrations().ShouldNotBeEmpty();
		context.Database.HasPendingModelChanges().ShouldBeFalse(
			"Model changed without a migration. Run: dotnet ef migrations add <Name> --context TestimonialsDbContext -o Infrastructure/Data/Migrations (from src/DevBetterWeb.Modules.Testimonials)");
	}

	[Fact]
	public async Task RoundTripsTestimonialWithAuthorProfile()
	{
		string db = Guid.NewGuid().ToString();
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.ChangesRequested);

		await using (var context = CreateInMemoryContext(db))
		{
			context.Testimonials.Add(testimonial);
			await context.SaveChangesAsync();
		}

		await using (var context = CreateInMemoryContext(db))
		{
			var loaded = await context.Testimonials.SingleAsync(t => t.Id == testimonial.Id);

			loaded.MemberId.ShouldBe(TestimonialBuilder.MemberId);
			loaded.Content.ShouldBe(TestimonialBuilder.DefaultContent);
			loaded.Status.ShouldBe(TestimonialStatus.ChangesRequested);
			loaded.ReviewerFeedback.ShouldBe("Please shorten it.");
			loaded.Author.DisplayName.ShouldBe("Ada Lovelace");
			loaded.Author.Title.ShouldBe("Engineer");
			loaded.Author.Organization.ShouldBe("Analytical Engines Ltd");
			loaded.Author.PhotoUrl.ShouldBe("https://example.com/ada.png");
		}
	}

	[Fact]
	public async Task ListAcceptedReturnsOnlyAcceptedNewestFirst()
	{
		string db = Guid.NewGuid().ToString();
		var day = TestimonialBuilder.Created;
		var olderAccepted = TestimonialBuilder.InStatus(TestimonialStatus.Accepted, day.AddDays(1), memberId: 1);
		var newerAccepted = TestimonialBuilder.InStatus(TestimonialStatus.Accepted, day.AddDays(3), memberId: 2);

		await using (var context = CreateInMemoryContext(db))
		{
			context.Testimonials.AddRange(
				olderAccepted,
				TestimonialBuilder.InStatus(TestimonialStatus.Draft),
				TestimonialBuilder.InStatus(TestimonialStatus.PendingReview),
				TestimonialBuilder.InStatus(TestimonialStatus.ChangesRequested),
				TestimonialBuilder.InStatus(TestimonialStatus.Declined),
				newerAccepted);
			await context.SaveChangesAsync();
		}

		await using (var context = CreateInMemoryContext(db))
		{
			var results = await new TestimonialQueries(context).ListAcceptedAsync();

			results.Select(r => r.Id).ShouldBe([newerAccepted.Id, olderAccepted.Id]);
			results.ShouldAllBe(r => r.Status == TestimonialStatus.Accepted);

			var first = results[0];
			first.MemberId.ShouldBe(2);
			first.DisplayName.ShouldBe("Ada Lovelace");
			first.Title.ShouldBe("Engineer");
			first.Organization.ShouldBe("Analytical Engines Ltd");
			first.PhotoUrl.ShouldBe("https://example.com/ada.png");
			first.ReviewedAt.ShouldBe(day.AddDays(3));
		}
	}

	[Fact]
	public async Task ListAcceptedHonorsMaxCount()
	{
		string db = Guid.NewGuid().ToString();
		await using (var context = CreateInMemoryContext(db))
		{
			for (int i = 1; i <= 3; i++)
			{
				context.Testimonials.Add(TestimonialBuilder.InStatus(TestimonialStatus.Accepted, TestimonialBuilder.Created.AddDays(i), memberId: i));
			}
			await context.SaveChangesAsync();
		}

		await using (var context = CreateInMemoryContext(db))
		{
			var queries = new TestimonialQueries(context);

			(await queries.ListAcceptedAsync(2)).Select(r => r.MemberId).ShouldBe([3, 2]);
			(await queries.ListAcceptedAsync(0)).ShouldBeEmpty();
		}
	}

	[Fact]
	public void AddTestimonialsModuleResolvesQueries()
	{
		string db = Guid.NewGuid().ToString();
		var services = new ServiceCollection();
		services.AddTestimonialsModule((_, options) => options.UseInMemoryDatabase(db));

		using var provider = services.BuildServiceProvider();
		using var scope = provider.CreateScope();

		scope.ServiceProvider.GetRequiredService<ITestimonialQueries>().ShouldBeOfType<TestimonialQueries>();
	}

	[Fact]
	public void UseTestimonialsSqlServerRequiresConnectionString()
	{
		Should.Throw<ArgumentException>(() =>
			new DbContextOptionsBuilder<TestimonialsDbContext>().UseTestimonialsSqlServer(""));
	}
}
