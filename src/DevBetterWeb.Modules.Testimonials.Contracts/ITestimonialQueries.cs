namespace DevBetterWeb.Modules.Testimonials.Contracts;

/// <summary>
/// Public read surface of the Testimonials module.
/// </summary>
public interface ITestimonialQueries
{
	/// <summary>
	/// Returns accepted testimonials, most recently accepted first.
	/// </summary>
	/// <param name="maxCount">Optional limit; null returns all accepted testimonials.</param>
	Task<IReadOnlyList<TestimonialDto>> ListAcceptedAsync(int? maxCount = null, CancellationToken cancellationToken = default);
}
