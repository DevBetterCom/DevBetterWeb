using Ardalis.GuardClauses;
using DevBetterWeb.Modules.Testimonials.Contracts;

namespace DevBetterWeb.Modules.Testimonials.Domain;

/// <summary>
/// A member-written testimonial that must be reviewed by an admin before it is published.
/// The author is referenced by MemberId only; the Membership module owns members.
/// Authorization (who is the author / an admin) is enforced by the calling use case.
/// </summary>
internal sealed class Testimonial
{
	public const int MaxContentLength = TestimonialLimits.MaxContentLength;
	public const int MaxFeedbackLength = TestimonialLimits.MaxFeedbackLength;

	// EF Core
	private Testimonial()
	{
		Content = string.Empty;
		Author = null!;
	}

	private Testimonial(int memberId, string content, AuthorProfile author, DateTimeOffset now)
	{
		MemberId = memberId;
		Content = content;
		Author = author;
		Status = TestimonialStatus.Draft;
		CreatedAt = now;
		UpdatedAt = now;
	}

	public int Id { get; private set; }
	public int MemberId { get; private set; }
	public string Content { get; private set; }
	public AuthorProfile Author { get; private set; }
	public TestimonialStatus Status { get; private set; }

	/// <summary>
	/// Feedback from the reviewing admin (required when changes are requested, optional when declined).
	/// Cleared when the author resubmits.
	/// </summary>
	public string? ReviewerFeedback { get; private set; }

	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }
	public DateTimeOffset? SubmittedAt { get; private set; }
	public DateTimeOffset? ReviewedAt { get; private set; }

	public bool CanBeEditedByAuthor => Status is TestimonialStatus.Draft or TestimonialStatus.ChangesRequested;
	public bool CanBeSubmitted => CanBeEditedByAuthor;
	public bool CanBeDeletedByAuthor => Status == TestimonialStatus.Draft;

	/// <summary>
	/// Concurrency token so simultaneous reviews (e.g. two admins) fail instead of silently overwriting.
	/// </summary>
	public byte[] RowVersion { get; private set; } = [];

	public static Testimonial CreateDraft(int memberId, string content, AuthorProfile author, DateTimeOffset now)
	{
		Guard.Against.NegativeOrZero(memberId);
		Guard.Against.Null(author);
		return new Testimonial(memberId, NormalizeContent(content), author, now);
	}

	public void Edit(string content, AuthorProfile author, DateTimeOffset now)
	{
		EnsureStatus("edit", TestimonialStatus.Draft, TestimonialStatus.ChangesRequested);
		Guard.Against.Null(author);
		string normalizedContent = NormalizeContent(content);

		Content = normalizedContent;
		Author = author;
		UpdatedAt = now;
	}

	public void Submit(DateTimeOffset now)
	{
		EnsureStatus("submit", TestimonialStatus.Draft, TestimonialStatus.ChangesRequested);

		Status = TestimonialStatus.PendingReview;
		ReviewerFeedback = null;
		SubmittedAt = now;
		UpdatedAt = now;
	}

	public void Accept(DateTimeOffset now)
	{
		EnsureStatus("accept", TestimonialStatus.PendingReview);

		Status = TestimonialStatus.Accepted;
		ReviewerFeedback = null;
		MarkReviewed(now);
	}

	public void Decline(DateTimeOffset now, string? feedback = null)
	{
		EnsureStatus("decline", TestimonialStatus.PendingReview);
		string? normalizedFeedback = NormalizeOptionalFeedback(feedback);

		Status = TestimonialStatus.Declined;
		ReviewerFeedback = normalizedFeedback;
		MarkReviewed(now);
	}

	public void RequestChanges(string feedback, DateTimeOffset now)
	{
		EnsureStatus("request changes to", TestimonialStatus.PendingReview);
		Guard.Against.NullOrWhiteSpace(feedback);
		string? normalizedFeedback = NormalizeOptionalFeedback(feedback);

		Status = TestimonialStatus.ChangesRequested;
		ReviewerFeedback = normalizedFeedback;
		MarkReviewed(now);
	}

	/// <summary>
	/// Removes a previously accepted testimonial from public display.
	/// </summary>
	public void Unpublish(DateTimeOffset now, string? reason = null)
	{
		EnsureStatus("unpublish", TestimonialStatus.Accepted);
		string? normalizedReason = NormalizeOptionalFeedback(reason);

		Status = TestimonialStatus.Declined;
		ReviewerFeedback = normalizedReason;
		MarkReviewed(now);
	}

	/// <summary>
	/// Call before removing the testimonial on the author's behalf; only drafts may be deleted.
	/// </summary>
	public void EnsureCanBeDeletedByAuthor()
	{
		EnsureStatus("delete", TestimonialStatus.Draft);
	}

	private void MarkReviewed(DateTimeOffset now)
	{
		ReviewedAt = now;
		UpdatedAt = now;
	}

	private void EnsureStatus(string operation, params TestimonialStatus[] allowed)
	{
		if (!allowed.Contains(Status))
		{
			throw new InvalidTestimonialStateException(operation, Status);
		}
	}

	private static string NormalizeContent(string content)
	{
		Guard.Against.NullOrWhiteSpace(content);
		string trimmed = content.Trim();
		Guard.Against.StringTooLong(trimmed, MaxContentLength, nameof(content));
		return trimmed;
	}

	private static string? NormalizeOptionalFeedback(string? feedback)
	{
		if (string.IsNullOrWhiteSpace(feedback))
		{
			return null;
		}

		string trimmed = feedback.Trim();
		Guard.Against.StringTooLong(trimmed, MaxFeedbackLength, nameof(feedback));
		return trimmed;
	}
}
