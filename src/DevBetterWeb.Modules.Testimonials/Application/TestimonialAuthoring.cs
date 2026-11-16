using System.Linq.Expressions;
using DevBetterWeb.Modules.Testimonials.Contracts;
using DevBetterWeb.Modules.Testimonials.Domain;
using DevBetterWeb.Modules.Testimonials.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Modules.Testimonials.Application;

internal sealed class TestimonialAuthoring : ITestimonialAuthoring
{
	internal const string ConcurrencyMessage = "This testimonial was changed by someone else. Reload the page and try again.";

	private readonly TestimonialsDbContext _dbContext;
	private readonly TimeProvider _timeProvider;

	public TestimonialAuthoring(TestimonialsDbContext dbContext, TimeProvider timeProvider)
	{
		_dbContext = dbContext;
		_timeProvider = timeProvider;
	}

	public async Task<IReadOnlyList<MemberTestimonialDto>> ListForMemberAsync(int memberId, CancellationToken cancellationToken = default)
	{
		return await _dbContext.Testimonials
			.AsNoTracking()
			.Where(t => t.MemberId == memberId)
			.OrderByDescending(t => t.UpdatedAt)
			.ThenByDescending(t => t.Id)
			.Select(ToDto)
			.ToListAsync(cancellationToken);
	}

	public async Task<MemberTestimonialDto?> GetForMemberAsync(int memberId, int testimonialId, CancellationToken cancellationToken = default)
	{
		return await _dbContext.Testimonials
			.AsNoTracking()
			.Where(t => t.Id == testimonialId && t.MemberId == memberId)
			.Select(ToDto)
			.SingleOrDefaultAsync(cancellationToken);
	}

	public async Task<TestimonialOperationResult> CreateDraftAsync(int memberId, TestimonialDraftInput input, CancellationToken cancellationToken = default)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(memberId);

		var errors = TestimonialDraftInputValidator.Validate(input);
		if (errors.Count > 0)
		{
			return TestimonialOperationResult.Invalid(errors);
		}

		var testimonial = Testimonial.CreateDraft(memberId, input.Content, ToAuthor(input), Now);
		_dbContext.Testimonials.Add(testimonial);
		await _dbContext.SaveChangesAsync(cancellationToken);

		return TestimonialOperationResult.Success(testimonial.Id);
	}

	public async Task<TestimonialOperationResult> UpdateAsync(int memberId, int testimonialId, TestimonialDraftInput input, CancellationToken cancellationToken = default)
	{
		var testimonial = await FindOwnedAsync(memberId, testimonialId, cancellationToken);
		if (testimonial is null)
		{
			return TestimonialOperationResult.NotFound();
		}

		if (!testimonial.CanBeEditedByAuthor)
		{
			return NotAllowed("edit", testimonial.Status);
		}

		var errors = TestimonialDraftInputValidator.Validate(input);
		if (errors.Count > 0)
		{
			return TestimonialOperationResult.Invalid(errors);
		}

		return await ApplyAsync(testimonial, t => t.Edit(input.Content, ToAuthor(input), Now), cancellationToken);
	}

	public async Task<TestimonialOperationResult> SubmitAsync(int memberId, int testimonialId, CancellationToken cancellationToken = default)
	{
		var testimonial = await FindOwnedAsync(memberId, testimonialId, cancellationToken);
		if (testimonial is null)
		{
			return TestimonialOperationResult.NotFound();
		}

		return await ApplyAsync(testimonial, t => t.Submit(Now), cancellationToken);
	}

	public async Task<TestimonialOperationResult> DeleteDraftAsync(int memberId, int testimonialId, CancellationToken cancellationToken = default)
	{
		var testimonial = await FindOwnedAsync(memberId, testimonialId, cancellationToken);
		if (testimonial is null)
		{
			return TestimonialOperationResult.NotFound();
		}

		return await ApplyAsync(testimonial, t =>
		{
			t.EnsureCanBeDeletedByAuthor();
			_dbContext.Testimonials.Remove(t);
		}, cancellationToken);
	}

	private DateTimeOffset Now => _timeProvider.GetUtcNow();

	private Task<Testimonial?> FindOwnedAsync(int memberId, int testimonialId, CancellationToken cancellationToken) =>
		_dbContext.Testimonials.SingleOrDefaultAsync(t => t.Id == testimonialId && t.MemberId == memberId, cancellationToken);

	private async Task<TestimonialOperationResult> ApplyAsync(Testimonial testimonial, Action<Testimonial> change, CancellationToken cancellationToken)
	{
		try
		{
			change(testimonial);
		}
		catch (InvalidTestimonialStateException ex)
		{
			return NotAllowed(ex.Operation, ex.CurrentStatus);
		}

		try
		{
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
		catch (DbUpdateConcurrencyException)
		{
			return TestimonialOperationResult.NotAllowed(ConcurrencyMessage);
		}

		return TestimonialOperationResult.Success(testimonial.Id);
	}

	private static TestimonialOperationResult NotAllowed(string operation, TestimonialStatus status) =>
		TestimonialOperationResult.NotAllowed($"You can't {operation} a testimonial while it is {StatusText(status)}.");

	private static string StatusText(TestimonialStatus status) => status switch
	{
		TestimonialStatus.PendingReview => "pending review",
		TestimonialStatus.ChangesRequested => "awaiting your changes",
		TestimonialStatus.Accepted => "accepted",
		TestimonialStatus.Declined => "declined",
		_ => "a draft",
	};

	private static AuthorProfile ToAuthor(TestimonialDraftInput input) =>
		AuthorProfile.Create(input.DisplayName, input.Title, input.Organization, input.PhotoUrl);

	private static readonly Expression<Func<Testimonial, MemberTestimonialDto>> ToDto = t =>
		new MemberTestimonialDto(
			t.Id,
			t.Content,
			t.Author.DisplayName,
			t.Author.Title,
			t.Author.Organization,
			t.Author.PhotoUrl,
			t.Status,
			t.Status == TestimonialStatus.ChangesRequested || t.Status == TestimonialStatus.Declined ? t.ReviewerFeedback : null,
			t.CreatedAt,
			t.UpdatedAt,
			t.SubmittedAt,
			t.ReviewedAt);
}
