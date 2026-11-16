namespace DevBetterWeb.Modules.Testimonials.Contracts;

/// <summary>
/// Member-facing use cases for writing testimonials. Every call is scoped to <c>memberId</c>:
/// a testimonial owned by another member is reported as not found.
/// The caller is responsible for passing the signed-in member's id.
/// </summary>
public interface ITestimonialAuthoring
{
	/// <summary>Returns the member's testimonials, most recently updated first.</summary>
	Task<IReadOnlyList<MemberTestimonialDto>> ListForMemberAsync(int memberId, CancellationToken cancellationToken = default);

	/// <summary>Returns the member's testimonial, or null if it does not exist or belongs to someone else.</summary>
	Task<MemberTestimonialDto?> GetForMemberAsync(int memberId, int testimonialId, CancellationToken cancellationToken = default);

	Task<TestimonialOperationResult> CreateDraftAsync(int memberId, TestimonialDraftInput input, CancellationToken cancellationToken = default);

	/// <summary>Edits a testimonial in Draft or ChangesRequested.</summary>
	Task<TestimonialOperationResult> UpdateAsync(int memberId, int testimonialId, TestimonialDraftInput input, CancellationToken cancellationToken = default);

	/// <summary>Submits (or resubmits) a testimonial for admin review.</summary>
	Task<TestimonialOperationResult> SubmitAsync(int memberId, int testimonialId, CancellationToken cancellationToken = default);

	/// <summary>Deletes a testimonial that is still a Draft.</summary>
	Task<TestimonialOperationResult> DeleteDraftAsync(int memberId, int testimonialId, CancellationToken cancellationToken = default);
}
