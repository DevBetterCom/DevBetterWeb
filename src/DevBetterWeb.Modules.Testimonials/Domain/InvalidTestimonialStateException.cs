using DevBetterWeb.Modules.Testimonials.Contracts;

namespace DevBetterWeb.Modules.Testimonials.Domain;

/// <summary>
/// Thrown when an operation is not allowed for a testimonial's current status.
/// </summary>
internal sealed class InvalidTestimonialStateException : InvalidOperationException
{
	public InvalidTestimonialStateException(string operation, TestimonialStatus currentStatus)
		: base($"Cannot {operation} a testimonial whose status is {currentStatus}.")
	{
		Operation = operation;
		CurrentStatus = currentStatus;
	}

	public string Operation { get; }
	public TestimonialStatus CurrentStatus { get; }
}
