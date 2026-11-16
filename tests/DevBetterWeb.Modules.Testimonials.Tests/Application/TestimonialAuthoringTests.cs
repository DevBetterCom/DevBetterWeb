using DevBetterWeb.Modules.Testimonials.Application;
using DevBetterWeb.Modules.Testimonials.Contracts;
using DevBetterWeb.Modules.Testimonials.Domain;
using DevBetterWeb.Modules.Testimonials.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Modules.Testimonials.Tests.Application;

public class TestimonialAuthoringTests
{
	private const int MemberId = TestimonialBuilder.MemberId;
	private const int OtherMemberId = 7;

	private readonly string _databaseName = Guid.NewGuid().ToString();
	private readonly FixedTimeProvider _time = new(new DateTimeOffset(2026, 11, 16, 9, 0, 0, TimeSpan.Zero));

	private static TestimonialDraftInput ValidInput(string content = "devBetter helped me grow.") =>
		new(content, "Grace Hopper", "Rear Admiral", "US Navy", "https://example.com/grace.png");

	private TestimonialsDbContext CreateContext() =>
		new(new DbContextOptionsBuilder<TestimonialsDbContext>().UseInMemoryDatabase(_databaseName).Options);

	private TestimonialAuthoring CreateSut(TestimonialsDbContext context) => new(context, _time);

	private async Task<int> SeedAsync(Testimonial testimonial)
	{
		await using var context = CreateContext();
		context.Testimonials.Add(testimonial);
		await context.SaveChangesAsync();
		return testimonial.Id;
	}

	private async Task<Testimonial?> LoadAsync(int id)
	{
		await using var context = CreateContext();
		return await context.Testimonials.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
	}

	[Fact]
	public async Task CreateDraftPersistsDraftForMember()
	{
		await using var context = CreateContext();

		var result = await CreateSut(context).CreateDraftAsync(MemberId, ValidInput());

		result.IsSuccess.ShouldBeTrue();
		var saved = (await LoadAsync(result.TestimonialId!.Value))!;
		saved.MemberId.ShouldBe(MemberId);
		saved.Status.ShouldBe(TestimonialStatus.Draft);
		saved.Content.ShouldBe("devBetter helped me grow.");
		saved.Author.DisplayName.ShouldBe("Grace Hopper");
		saved.Author.Title.ShouldBe("Rear Admiral");
		saved.Author.Organization.ShouldBe("US Navy");
		saved.Author.PhotoUrl.ShouldBe("https://example.com/grace.png");
		saved.CreatedAt.ShouldBe(_time.Now);
	}

	[Theory]
	[InlineData("", "Grace", null, nameof(TestimonialDraftInput.Content))]
	[InlineData("Great", " ", null, nameof(TestimonialDraftInput.DisplayName))]
	[InlineData("Great", "Grace", "http://example.com/a.png", nameof(TestimonialDraftInput.PhotoUrl))]
	[InlineData("Great", "Grace", "javascript:alert(1)", nameof(TestimonialDraftInput.PhotoUrl))]
	public async Task CreateDraftRejectsInvalidInputWithFieldErrors(string content, string displayName, string? photoUrl, string expectedField)
	{
		await using var context = CreateContext();

		var result = await CreateSut(context).CreateDraftAsync(MemberId, new TestimonialDraftInput(content, displayName, null, null, photoUrl));

		result.Status.ShouldBe(TestimonialOperationStatus.Invalid);
		result.Errors.ShouldContain(e => e.Field == expectedField);
		(await context.Testimonials.CountAsync()).ShouldBe(0);
	}

	[Fact]
	public async Task CreateDraftRejectsOverLongFields()
	{
		await using var context = CreateContext();
		var input = new TestimonialDraftInput(
			new string('a', TestimonialLimits.MaxContentLength + 1),
			new string('b', TestimonialLimits.MaxDisplayNameLength + 1),
			new string('c', TestimonialLimits.MaxTitleLength + 1),
			new string('d', TestimonialLimits.MaxOrganizationLength + 1),
			"https://example.com/" + new string('e', TestimonialLimits.MaxPhotoUrlLength));

		var result = await CreateSut(context).CreateDraftAsync(MemberId, input);

		result.Status.ShouldBe(TestimonialOperationStatus.Invalid);
		result.Errors.Select(e => e.Field).ShouldBe(
			[
				nameof(TestimonialDraftInput.Content),
				nameof(TestimonialDraftInput.DisplayName),
				nameof(TestimonialDraftInput.Title),
				nameof(TestimonialDraftInput.Organization),
				nameof(TestimonialDraftInput.PhotoUrl),
			],
			ignoreOrder: true);
	}

	[Theory]
	[InlineData(TestimonialStatus.Draft)]
	[InlineData(TestimonialStatus.ChangesRequested)]
	public async Task UpdateEditsWhenEditable(TestimonialStatus status)
	{
		int id = await SeedAsync(TestimonialBuilder.InStatus(status));
		_time.Advance(TimeSpan.FromHours(1));
		await using var context = CreateContext();

		var result = await CreateSut(context).UpdateAsync(MemberId, id, ValidInput("Edited text"));

		result.IsSuccess.ShouldBeTrue();
		var saved = (await LoadAsync(id))!;
		saved.Content.ShouldBe("Edited text");
		saved.Author.DisplayName.ShouldBe("Grace Hopper");
		saved.Status.ShouldBe(status);
		saved.UpdatedAt.ShouldBe(_time.Now);
	}

	[Theory]
	[InlineData(TestimonialStatus.PendingReview)]
	[InlineData(TestimonialStatus.Accepted)]
	[InlineData(TestimonialStatus.Declined)]
	public async Task UpdateIsBlockedOnceSubmitted(TestimonialStatus status)
	{
		int id = await SeedAsync(TestimonialBuilder.InStatus(status));
		await using var context = CreateContext();

		var result = await CreateSut(context).UpdateAsync(MemberId, id, ValidInput("Sneaky edit"));

		result.Status.ShouldBe(TestimonialOperationStatus.NotAllowed);
		result.Errors.ShouldHaveSingleItem().Field.ShouldBeNull();
		(await LoadAsync(id))!.Content.ShouldBe(TestimonialBuilder.DefaultContent);
	}

	[Fact]
	public async Task UpdateChecksStatusBeforeValidatingInput()
	{
		int id = await SeedAsync(TestimonialBuilder.InStatus(TestimonialStatus.PendingReview));
		await using var context = CreateContext();

		var result = await CreateSut(context).UpdateAsync(MemberId, id, new TestimonialDraftInput("", "", null, null, null));

		result.Status.ShouldBe(TestimonialOperationStatus.NotAllowed);
	}

	[Theory]
	[InlineData(TestimonialStatus.Draft)]
	[InlineData(TestimonialStatus.ChangesRequested)]
	public async Task SubmitMovesToPendingReviewAndClearsFeedback(TestimonialStatus status)
	{
		int id = await SeedAsync(TestimonialBuilder.InStatus(status));
		await using var context = CreateContext();

		var result = await CreateSut(context).SubmitAsync(MemberId, id);

		result.IsSuccess.ShouldBeTrue();
		var saved = (await LoadAsync(id))!;
		saved.Status.ShouldBe(TestimonialStatus.PendingReview);
		saved.ReviewerFeedback.ShouldBeNull();
		saved.SubmittedAt.ShouldBe(_time.Now);
	}

	[Theory]
	[InlineData(TestimonialStatus.PendingReview)]
	[InlineData(TestimonialStatus.Accepted)]
	[InlineData(TestimonialStatus.Declined)]
	public async Task SubmitIsBlockedOnceSubmitted(TestimonialStatus status)
	{
		int id = await SeedAsync(TestimonialBuilder.InStatus(status));
		await using var context = CreateContext();

		var result = await CreateSut(context).SubmitAsync(MemberId, id);

		result.Status.ShouldBe(TestimonialOperationStatus.NotAllowed);
		(await LoadAsync(id))!.Status.ShouldBe(status);
	}

	[Fact]
	public async Task DeleteRemovesDraft()
	{
		int id = await SeedAsync(TestimonialBuilder.Draft());
		await using var context = CreateContext();

		var result = await CreateSut(context).DeleteDraftAsync(MemberId, id);

		result.IsSuccess.ShouldBeTrue();
		(await LoadAsync(id)).ShouldBeNull();
	}

	[Theory]
	[InlineData(TestimonialStatus.PendingReview)]
	[InlineData(TestimonialStatus.ChangesRequested)]
	[InlineData(TestimonialStatus.Accepted)]
	[InlineData(TestimonialStatus.Declined)]
	public async Task DeleteIsBlockedUnlessDraft(TestimonialStatus status)
	{
		int id = await SeedAsync(TestimonialBuilder.InStatus(status));
		await using var context = CreateContext();

		var result = await CreateSut(context).DeleteDraftAsync(MemberId, id);

		result.Status.ShouldBe(TestimonialOperationStatus.NotAllowed);
		(await LoadAsync(id)).ShouldNotBeNull();
	}

	[Fact]
	public async Task OtherMembersTestimonialsAreNotFound()
	{
		int id = await SeedAsync(TestimonialBuilder.Draft(memberId: OtherMemberId));
		await using var context = CreateContext();
		var sut = CreateSut(context);

		(await sut.GetForMemberAsync(MemberId, id)).ShouldBeNull();
		(await sut.UpdateAsync(MemberId, id, ValidInput("Hijacked"))).Status.ShouldBe(TestimonialOperationStatus.NotFound);
		(await sut.SubmitAsync(MemberId, id)).Status.ShouldBe(TestimonialOperationStatus.NotFound);
		(await sut.DeleteDraftAsync(MemberId, id)).Status.ShouldBe(TestimonialOperationStatus.NotFound);

		var untouched = (await LoadAsync(id))!;
		untouched.Content.ShouldBe(TestimonialBuilder.DefaultContent);
		untouched.Status.ShouldBe(TestimonialStatus.Draft);
	}

	[Fact]
	public async Task ListReturnsOnlyTheMembersTestimonialsMostRecentlyUpdatedFirst()
	{
		var older = TestimonialBuilder.Draft();
		var newer = TestimonialBuilder.InStatus(TestimonialStatus.PendingReview, TestimonialBuilder.Created.AddDays(5));
		await SeedAsync(older);
		await SeedAsync(newer);
		await SeedAsync(TestimonialBuilder.Draft(memberId: OtherMemberId));
		await using var context = CreateContext();

		var results = await CreateSut(context).ListForMemberAsync(MemberId);

		results.Select(r => r.Id).ShouldBe([newer.Id, older.Id]);
	}

	[Theory]
	[InlineData(TestimonialStatus.ChangesRequested, "Please shorten it.")]
	[InlineData(TestimonialStatus.Draft, null)]
	public async Task GetExposesReviewerFeedbackAndPermissions(TestimonialStatus status, string? expectedFeedback)
	{
		int id = await SeedAsync(TestimonialBuilder.InStatus(status));
		await using var context = CreateContext();

		var dto = (await CreateSut(context).GetForMemberAsync(MemberId, id))!;

		dto.Status.ShouldBe(status);
		dto.ReviewerFeedback.ShouldBe(expectedFeedback);
		dto.CanEdit.ShouldBeTrue();
		dto.CanSubmit.ShouldBeTrue();
		dto.CanDelete.ShouldBe(status == TestimonialStatus.Draft);
	}

	[Fact]
	public async Task GetShowsDeclineFeedbackToAuthor()
	{
		var declined = TestimonialBuilder.InStatus(TestimonialStatus.PendingReview);
		declined.Decline(TestimonialBuilder.Created.AddDays(2), "Not a fit for the home page.");
		int id = await SeedAsync(declined);
		await using var context = CreateContext();

		var dto = (await CreateSut(context).GetForMemberAsync(MemberId, id))!;

		dto.ReviewerFeedback.ShouldBe("Not a fit for the home page.");
		dto.CanEdit.ShouldBeFalse();
		dto.CanDelete.ShouldBeFalse();
	}

	[Fact]
	public void AddTestimonialsModuleResolvesAuthoring()
	{
		var services = new ServiceCollection();
		services.AddTestimonialsModule((_, options) => options.UseInMemoryDatabase(_databaseName));

		using var provider = services.BuildServiceProvider();
		using var scope = provider.CreateScope();

		scope.ServiceProvider.GetRequiredService<ITestimonialAuthoring>().ShouldBeOfType<TestimonialAuthoring>();
	}

	private sealed class FixedTimeProvider(DateTimeOffset start) : TimeProvider
	{
		public DateTimeOffset Now { get; private set; } = start;

		public void Advance(TimeSpan by) => Now += by;

		public override DateTimeOffset GetUtcNow() => Now;
	}
}
