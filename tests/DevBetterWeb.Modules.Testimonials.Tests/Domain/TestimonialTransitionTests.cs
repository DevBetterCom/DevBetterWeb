using DevBetterWeb.Modules.Testimonials.Contracts;
using DevBetterWeb.Modules.Testimonials.Domain;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Modules.Testimonials.Tests.Domain;

public class TestimonialTransitionTests
{
	private static readonly DateTimeOffset Now = TestimonialBuilder.Created.AddDays(5);

	public static TheoryData<TestimonialStatus> AllStatuses() => new(Enum.GetValues<TestimonialStatus>());

	public static TheoryData<TestimonialStatus> NotEditable() =>
		new(Except(TestimonialStatus.Draft, TestimonialStatus.ChangesRequested));

	public static TheoryData<TestimonialStatus> NotPendingReview() =>
		new(Except(TestimonialStatus.PendingReview));

	public static TheoryData<TestimonialStatus> NotDraft() =>
		new(Except(TestimonialStatus.Draft));

	public static TheoryData<TestimonialStatus> NotAccepted() =>
		new(Except(TestimonialStatus.Accepted));

	private static IEnumerable<TestimonialStatus> Except(params TestimonialStatus[] allowed) =>
		Enum.GetValues<TestimonialStatus>().Except(allowed);

	[Fact]
	public void CreateDraftStartsInDraftWithTimestamps()
	{
		var testimonial = TestimonialBuilder.Draft();

		testimonial.Status.ShouldBe(TestimonialStatus.Draft);
		testimonial.MemberId.ShouldBe(TestimonialBuilder.MemberId);
		testimonial.Content.ShouldBe(TestimonialBuilder.DefaultContent);
		testimonial.CreatedAt.ShouldBe(TestimonialBuilder.Created);
		testimonial.UpdatedAt.ShouldBe(TestimonialBuilder.Created);
		testimonial.SubmittedAt.ShouldBeNull();
		testimonial.ReviewedAt.ShouldBeNull();
		testimonial.ReviewerFeedback.ShouldBeNull();
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void CreateDraftRejectsInvalidMemberId(int memberId)
	{
		Should.Throw<ArgumentException>(() => TestimonialBuilder.Draft(memberId));
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void CreateDraftRejectsBlankContent(string content)
	{
		Should.Throw<ArgumentException>(() =>
			Testimonial.CreateDraft(1, content, TestimonialBuilder.Author(), Now));
	}

	[Fact]
	public void CreateDraftRejectsContentOverMaxLength()
	{
		string content = new('x', Testimonial.MaxContentLength + 1);

		Should.Throw<ArgumentException>(() =>
			Testimonial.CreateDraft(1, content, TestimonialBuilder.Author(), Now));
	}

	[Fact]
	public void CreateDraftTrimsContent()
	{
		var testimonial = Testimonial.CreateDraft(1, "  Great group!  ", TestimonialBuilder.Author(), Now);

		testimonial.Content.ShouldBe("Great group!");
	}

	[Theory]
	[InlineData(TestimonialStatus.Draft)]
	[InlineData(TestimonialStatus.ChangesRequested)]
	public void EditUpdatesContentAndAuthorWhenEditable(TestimonialStatus status)
	{
		var testimonial = TestimonialBuilder.InStatus(status);
		var newAuthor = AuthorProfile.Create("Grace Hopper", organization: "US Navy");

		testimonial.Edit("Updated text", newAuthor, Now);

		testimonial.Content.ShouldBe("Updated text");
		testimonial.Author.ShouldBeSameAs(newAuthor);
		testimonial.UpdatedAt.ShouldBe(Now);
		testimonial.Status.ShouldBe(status);
	}

	[Theory]
	[MemberData(nameof(NotEditable))]
	public void EditThrowsWhenNotEditable(TestimonialStatus status)
	{
		var testimonial = TestimonialBuilder.InStatus(status);

		var ex = Should.Throw<InvalidTestimonialStateException>(() =>
			testimonial.Edit("Updated text", TestimonialBuilder.Author(), Now));

		ex.CurrentStatus.ShouldBe(status);
		testimonial.Content.ShouldBe(TestimonialBuilder.DefaultContent);
	}

	[Fact]
	public void EditWithInvalidContentLeavesTestimonialUnchanged()
	{
		var testimonial = TestimonialBuilder.Draft();
		var originalAuthor = testimonial.Author;

		Should.Throw<ArgumentException>(() => testimonial.Edit("  ", AuthorProfile.Create("Someone Else"), Now));

		testimonial.Content.ShouldBe(TestimonialBuilder.DefaultContent);
		testimonial.Author.ShouldBeSameAs(originalAuthor);
		testimonial.UpdatedAt.ShouldBe(TestimonialBuilder.Created);
	}

	[Fact]
	public void SubmitMovesDraftToPendingReview()
	{
		var testimonial = TestimonialBuilder.Draft();

		testimonial.Submit(Now);

		testimonial.Status.ShouldBe(TestimonialStatus.PendingReview);
		testimonial.SubmittedAt.ShouldBe(Now);
		testimonial.UpdatedAt.ShouldBe(Now);
	}

	[Fact]
	public void ResubmitAfterChangesRequestedClearsFeedback()
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.ChangesRequested);
		testimonial.ReviewerFeedback.ShouldBe("Please shorten it.");

		testimonial.Submit(Now);

		testimonial.Status.ShouldBe(TestimonialStatus.PendingReview);
		testimonial.ReviewerFeedback.ShouldBeNull();
		testimonial.SubmittedAt.ShouldBe(Now);
	}

	[Theory]
	[MemberData(nameof(NotEditable))]
	public void SubmitThrowsWhenNotDraftOrChangesRequested(TestimonialStatus status)
	{
		var testimonial = TestimonialBuilder.InStatus(status);

		Should.Throw<InvalidTestimonialStateException>(() => testimonial.Submit(Now));
		testimonial.Status.ShouldBe(status);
	}

	[Fact]
	public void AcceptMovesPendingToAccepted()
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.PendingReview);

		testimonial.Accept(Now);

		testimonial.Status.ShouldBe(TestimonialStatus.Accepted);
		testimonial.ReviewedAt.ShouldBe(Now);
		testimonial.UpdatedAt.ShouldBe(Now);
		testimonial.ReviewerFeedback.ShouldBeNull();
	}

	[Fact]
	public void DeclineMovesPendingToDeclinedWithOptionalFeedback()
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.PendingReview);

		testimonial.Decline(Now, "  Not a fit.  ");

		testimonial.Status.ShouldBe(TestimonialStatus.Declined);
		testimonial.ReviewerFeedback.ShouldBe("Not a fit.");
		testimonial.ReviewedAt.ShouldBe(Now);
	}

	[Fact]
	public void DeclineWithoutFeedbackLeavesFeedbackNull()
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.PendingReview);

		testimonial.Decline(Now);

		testimonial.Status.ShouldBe(TestimonialStatus.Declined);
		testimonial.ReviewerFeedback.ShouldBeNull();
	}

	[Fact]
	public void RequestChangesMovesPendingToChangesRequestedWithFeedback()
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.PendingReview);

		testimonial.RequestChanges("Add your job title.", Now);

		testimonial.Status.ShouldBe(TestimonialStatus.ChangesRequested);
		testimonial.ReviewerFeedback.ShouldBe("Add your job title.");
		testimonial.ReviewedAt.ShouldBe(Now);
		testimonial.CanBeEditedByAuthor.ShouldBeTrue();
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void RequestChangesRequiresFeedback(string feedback)
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.PendingReview);

		Should.Throw<ArgumentException>(() => testimonial.RequestChanges(feedback, Now));
		testimonial.Status.ShouldBe(TestimonialStatus.PendingReview);
	}

	[Fact]
	public void FeedbackOverMaxLengthIsRejected()
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.PendingReview);
		string feedback = new('x', Testimonial.MaxFeedbackLength + 1);

		Should.Throw<ArgumentException>(() => testimonial.RequestChanges(feedback, Now));
		testimonial.Status.ShouldBe(TestimonialStatus.PendingReview);
	}

	[Theory]
	[MemberData(nameof(NotPendingReview))]
	public void ReviewActionsThrowWhenNotPendingReview(TestimonialStatus status)
	{
		var testimonial = TestimonialBuilder.InStatus(status);
		var before = Snapshot(testimonial);

		Should.Throw<InvalidTestimonialStateException>(() => testimonial.Accept(Now));
		Should.Throw<InvalidTestimonialStateException>(() => testimonial.Decline(Now, "feedback"));
		Should.Throw<InvalidTestimonialStateException>(() => testimonial.RequestChanges("feedback", Now));
		Snapshot(testimonial).ShouldBe(before);
	}

	[Fact]
	public void DeclineRejectsFeedbackOverMaxLength()
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.PendingReview);
		var before = Snapshot(testimonial);

		Should.Throw<ArgumentException>(() => testimonial.Decline(Now, new string('x', Testimonial.MaxFeedbackLength + 1)));
		Snapshot(testimonial).ShouldBe(before);
	}

	[Fact]
	public void UnpublishRejectsReasonOverMaxLength()
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.Accepted);
		var before = Snapshot(testimonial);

		Should.Throw<ArgumentException>(() => testimonial.Unpublish(Now, new string('x', Testimonial.MaxFeedbackLength + 1)));
		Snapshot(testimonial).ShouldBe(before);
	}

	[Fact]
	public void CreateDraftRejectsNullAuthor()
	{
		Should.Throw<ArgumentNullException>(() => Testimonial.CreateDraft(1, "text", null!, Now));
	}

	[Fact]
	public void EditRejectsNullAuthor()
	{
		var testimonial = TestimonialBuilder.Draft();

		Should.Throw<ArgumentNullException>(() => testimonial.Edit("text", null!, Now));
		testimonial.Content.ShouldBe(TestimonialBuilder.DefaultContent);
	}

	[Fact]
	public void EnsureCanBeDeletedByAuthorAllowsDraft()
	{
		Should.NotThrow(() => TestimonialBuilder.Draft().EnsureCanBeDeletedByAuthor());
	}

	[Theory]
	[MemberData(nameof(NotDraft))]
	public void EnsureCanBeDeletedByAuthorThrowsWhenNotDraft(TestimonialStatus status)
	{
		var testimonial = TestimonialBuilder.InStatus(status);

		Should.Throw<InvalidTestimonialStateException>(() => testimonial.EnsureCanBeDeletedByAuthor());
	}

	private static (TestimonialStatus, string?, DateTimeOffset, DateTimeOffset?, DateTimeOffset?) Snapshot(Testimonial t) =>
		(t.Status, t.ReviewerFeedback, t.UpdatedAt, t.SubmittedAt, t.ReviewedAt);

	[Fact]
	public void UnpublishMovesAcceptedToDeclined()
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.Accepted);

		testimonial.Unpublish(Now, "Member left the program.");

		testimonial.Status.ShouldBe(TestimonialStatus.Declined);
		testimonial.ReviewerFeedback.ShouldBe("Member left the program.");
		testimonial.ReviewedAt.ShouldBe(Now);
	}

	[Theory]
	[MemberData(nameof(NotAccepted))]
	public void UnpublishThrowsWhenNotAccepted(TestimonialStatus status)
	{
		var testimonial = TestimonialBuilder.InStatus(status);

		Should.Throw<InvalidTestimonialStateException>(() => testimonial.Unpublish(Now));
		testimonial.Status.ShouldBe(status);
	}

	[Theory]
	[MemberData(nameof(AllStatuses))]
	public void AuthorCapabilitiesMatchStatus(TestimonialStatus status)
	{
		var testimonial = TestimonialBuilder.InStatus(status);

		bool editable = status is TestimonialStatus.Draft or TestimonialStatus.ChangesRequested;
		testimonial.CanBeEditedByAuthor.ShouldBe(editable);
		testimonial.CanBeSubmitted.ShouldBe(editable);
		testimonial.CanBeDeletedByAuthor.ShouldBe(status == TestimonialStatus.Draft);
	}

	[Fact]
	public void DeclinedIsTerminal()
	{
		AssertTerminal(TestimonialBuilder.InStatus(TestimonialStatus.Declined));
	}

	[Fact]
	public void DeclinedViaUnpublishIsTerminal()
	{
		var testimonial = TestimonialBuilder.InStatus(TestimonialStatus.Accepted);
		testimonial.Unpublish(Now);

		AssertTerminal(testimonial);
	}

	private static void AssertTerminal(Testimonial testimonial)
	{
		testimonial.Status.ShouldBe(TestimonialStatus.Declined);

		Should.Throw<InvalidTestimonialStateException>(() => testimonial.Edit("x", TestimonialBuilder.Author(), Now));
		Should.Throw<InvalidTestimonialStateException>(() => testimonial.Submit(Now));
		Should.Throw<InvalidTestimonialStateException>(() => testimonial.Accept(Now));
		Should.Throw<InvalidTestimonialStateException>(() => testimonial.Decline(Now));
		Should.Throw<InvalidTestimonialStateException>(() => testimonial.RequestChanges("x", Now));
		Should.Throw<InvalidTestimonialStateException>(() => testimonial.Unpublish(Now));
	}
}
