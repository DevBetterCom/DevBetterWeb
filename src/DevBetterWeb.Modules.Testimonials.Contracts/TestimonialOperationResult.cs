namespace DevBetterWeb.Modules.Testimonials.Contracts;

public enum TestimonialOperationStatus
{
	Success = 0,

	/// <summary>The testimonial does not exist or belongs to another member.</summary>
	NotFound = 1,

	/// <summary>The operation is not allowed in the testimonial's current status, or it changed concurrently.</summary>
	NotAllowed = 2,

	/// <summary>The input failed validation; see <see cref="TestimonialOperationResult.Errors"/>.</summary>
	Invalid = 3,
}

/// <param name="Field">The <see cref="TestimonialDraftInput"/> property name, or null for a general error.</param>
public sealed record TestimonialValidationError(string? Field, string Message);

/// <summary>
/// Outcome of an authoring command. Contracts stay dependency-free, so this is a small local result type.
/// </summary>
public sealed record TestimonialOperationResult
{
	private TestimonialOperationResult(TestimonialOperationStatus status, int? testimonialId, IReadOnlyList<TestimonialValidationError> errors)
	{
		Status = status;
		TestimonialId = testimonialId;
		Errors = errors;
	}

	public TestimonialOperationStatus Status { get; }

	/// <summary>The affected testimonial's id when the operation succeeded.</summary>
	public int? TestimonialId { get; }

	public IReadOnlyList<TestimonialValidationError> Errors { get; }

	public bool IsSuccess => Status == TestimonialOperationStatus.Success;

	public static TestimonialOperationResult Success(int testimonialId) =>
		new(TestimonialOperationStatus.Success, testimonialId, []);

	public static TestimonialOperationResult NotFound() =>
		new(TestimonialOperationStatus.NotFound, null, []);

	public static TestimonialOperationResult NotAllowed(string message) =>
		new(TestimonialOperationStatus.NotAllowed, null, [new TestimonialValidationError(null, message)]);

	public static TestimonialOperationResult Invalid(IReadOnlyList<TestimonialValidationError> errors) =>
		new(TestimonialOperationStatus.Invalid, null, errors);
}
