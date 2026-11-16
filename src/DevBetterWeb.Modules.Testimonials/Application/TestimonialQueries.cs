using DevBetterWeb.Modules.Testimonials.Contracts;
using DevBetterWeb.Modules.Testimonials.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Modules.Testimonials.Application;

internal sealed class TestimonialQueries : ITestimonialQueries
{
	private readonly TestimonialsDbContext _dbContext;

	public TestimonialQueries(TestimonialsDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IReadOnlyList<TestimonialDto>> ListAcceptedAsync(int? maxCount = null, CancellationToken cancellationToken = default)
	{
		if (maxCount is <= 0)
		{
			return [];
		}

		IQueryable<TestimonialDto> query = _dbContext.Testimonials
			.AsNoTracking()
			.Where(t => t.Status == TestimonialStatus.Accepted)
			.OrderByDescending(t => t.ReviewedAt)
			.ThenByDescending(t => t.Id)
			.Select(t => new TestimonialDto(
				t.Id,
				t.MemberId,
				t.Content,
				t.Author.DisplayName,
				t.Author.Title,
				t.Author.Organization,
				t.Author.PhotoUrl,
				t.Status,
				t.ReviewedAt));

		if (maxCount is int limit)
		{
			query = query.Take(limit);
		}

		return await query.ToListAsync(cancellationToken);
	}
}
