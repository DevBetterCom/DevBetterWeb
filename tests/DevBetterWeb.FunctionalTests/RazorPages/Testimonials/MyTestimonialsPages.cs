using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.FunctionalTests.Auth;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Modules.Testimonials.Contracts;
using DevBetterWeb.Modules.Testimonials.Domain;
using DevBetterWeb.Modules.Testimonials.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

#nullable enable

namespace DevBetterWeb.FunctionalTests.RazorPages.Testimonials;

public class MyTestimonialsPages : IClassFixture<AuthenticatedWebApplicationFactory>
{
	private const string IndexUrl = "/User/MyProfile/Testimonials";
	private const string EditUrl = "/User/MyProfile/Testimonials/Edit";
	private const string Author = AuthConstants.Users.Demo.EMAIL;
	private const string OtherMember = AuthConstants.Users.Demo2.EMAIL;

	private readonly AuthenticatedWebApplicationFactory _factory;

	public MyTestimonialsPages(AuthenticatedWebApplicationFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public async Task AnonymousUsersAreChallenged()
	{
		var client = _factory.CreateAnonymousClient();

		var response = await client.GetAsync(IndexUrl);

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task IndexListsOnlyTheMembersOwnTestimonials()
	{
		string mine = Unique("mine");
		string theirs = Unique("theirs");
		await SeedDraftAsync(Author, mine);
		await SeedDraftAsync(OtherMember, theirs);

		string html = await GetSuccessHtmlAsync(_factory.CreateClientFor(Author), IndexUrl);

		Assert.Contains(mine, html);
		Assert.DoesNotContain(theirs, html);
	}

	[Fact]
	public async Task NewTestimonialFormDefaultsDisplayNameToMemberName()
	{
		string html = await GetSuccessHtmlAsync(_factory.CreateClientFor(Author), EditUrl);

		Assert.Contains($"value=\"{AuthConstants.Users.Demo.FIRST_NAME} {AuthConstants.Users.Demo.LAST_NAME}\"", html);
	}

	[Fact]
	public async Task MemberCanCreateDraft()
	{
		var client = _factory.CreateClientFor(Author);
		string content = Unique("created");

		var response = await PostFormAsync(client, EditUrl, EditUrl, FormFor(content, title: "Engineer", organization: "Acme"));

		AssertRedirectsTo(IndexUrl, response);
		var created = (await ListAsync(Author)).Single(t => t.Content == content);
		Assert.Equal(TestimonialStatus.Draft, created.Status);
		Assert.Equal("Demo User", created.DisplayName);
		Assert.Equal("Engineer", created.Title);
		Assert.Equal("Acme", created.Organization);
		Assert.Null(created.PhotoUrl);
	}

	[Fact]
	public async Task InvalidInputRedisplaysFormWithErrors()
	{
		var client = _factory.CreateClientFor(Author);
		string displayName = Unique("invalid");

		var form = FormFor(content: "Valid text", displayName: displayName, photoUrl: "http://insecure.example.com/me.png");
		var response = await PostFormAsync(client, EditUrl, EditUrl, form);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Contains("must be an absolute https:// URL", await response.Content.ReadAsStringAsync());
		Assert.DoesNotContain(await ListAsync(Author), t => t.DisplayName == displayName);
	}

	[Fact]
	public async Task MemberCanEditDraft()
	{
		int id = await SeedDraftAsync(Author, Unique("before"));
		var client = _factory.CreateClientFor(Author);
		string edited = Unique("after");

		var response = await PostFormAsync(client, $"{EditUrl}/{id}", $"{EditUrl}/{id}", FormFor(edited));

		AssertRedirectsTo(IndexUrl, response);
		var testimonial = await GetAsync(Author, id);
		Assert.Equal(edited, testimonial!.Content);
		Assert.Equal(TestimonialStatus.Draft, testimonial.Status);
	}

	[Fact]
	public async Task MemberCanSubmitFromList()
	{
		int id = await SeedDraftAsync(Author, Unique("submit-list"));
		var client = _factory.CreateClientFor(Author);

		var response = await PostFormAsync(client, IndexUrl, $"{IndexUrl}?handler=Submit&id={id}", []);

		AssertRedirectsTo(IndexUrl, response);
		Assert.Equal(TestimonialStatus.PendingReview, (await GetAsync(Author, id))!.Status);
	}

	[Fact]
	public async Task MemberCanSaveAndSubmitFromEditPage()
	{
		int id = await SeedDraftAsync(Author, Unique("submit-edit"));
		var client = _factory.CreateClientFor(Author);
		string finalText = Unique("final");

		var response = await PostFormAsync(client, $"{EditUrl}/{id}", $"{EditUrl}/{id}?handler=SubmitForReview", FormFor(finalText));

		AssertRedirectsTo(IndexUrl, response);
		var testimonial = (await GetAsync(Author, id))!;
		Assert.Equal(TestimonialStatus.PendingReview, testimonial.Status);
		Assert.Equal(finalText, testimonial.Content);
	}

	[Fact]
	public async Task EditingIsBlockedAfterSubmit()
	{
		string original = Unique("locked");
		int id = await SeedDraftAsync(Author, original);
		await SubmitAsync(Author, id);
		var client = _factory.CreateClientFor(Author);

		// The page renders read-only: content shown, no form fields.
		string html = await GetSuccessHtmlAsync(client, $"{EditUrl}/{id}");
		Assert.Contains(original, html);
		Assert.Contains("Pending review", html);
		Assert.DoesNotContain("<textarea", html);

		// A crafted POST is rejected and changes nothing. The read-only page has no form, so take the token from the list page.
		var response = await PostFormAsync(client, IndexUrl, $"{EditUrl}/{id}", FormFor(Unique("tampered")));
		Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

		var submitAgain = await PostFormAsync(client, IndexUrl, $"{IndexUrl}?handler=Submit&id={id}", []);
		AssertRedirectsTo(IndexUrl, submitAgain);
		Assert.Contains("submit a testimonial while it is pending review", await GetSuccessHtmlAsync(client, IndexUrl));

		var deleteAttempt = await PostFormAsync(client, IndexUrl, $"{IndexUrl}?handler=Delete&id={id}", []);
		AssertRedirectsTo(IndexUrl, deleteAttempt);
		Assert.Contains("delete a testimonial while it is pending review", await GetSuccessHtmlAsync(client, IndexUrl));

		var testimonial = (await GetAsync(Author, id))!;
		Assert.Equal(original, testimonial.Content);
		Assert.Equal(TestimonialStatus.PendingReview, testimonial.Status);
	}

	[Fact]
	public async Task MemberSeesFeedbackAndCanResubmitAfterChangesRequested()
	{
		string feedback = Unique("Please mention a specific result");
		int id = await SeedDraftAsync(Author, Unique("needs-work"));
		await SubmitAsync(Author, id);
		await ReviewAsync(id, (t, now) => t.RequestChanges(feedback, now));
		var client = _factory.CreateClientFor(Author);

		Assert.Contains(feedback, await GetSuccessHtmlAsync(client, IndexUrl));
		string editHtml = await GetSuccessHtmlAsync(client, $"{EditUrl}/{id}");
		Assert.Contains(feedback, editHtml);
		Assert.Contains("Changes requested", editHtml);
		Assert.Contains("<textarea", editHtml);

		string revised = Unique("revised");
		var response = await PostFormAsync(client, $"{EditUrl}/{id}", $"{EditUrl}/{id}?handler=SubmitForReview", FormFor(revised));

		AssertRedirectsTo(IndexUrl, response);
		var testimonial = (await GetAsync(Author, id))!;
		Assert.Equal(TestimonialStatus.PendingReview, testimonial.Status);
		Assert.Equal(revised, testimonial.Content);
		Assert.Null(testimonial.ReviewerFeedback);
	}

	[Fact]
	public async Task AcceptedTestimonialIsReadOnly()
	{
		string original = Unique("published");
		int id = await SeedDraftAsync(Author, original);
		await SubmitAsync(Author, id);
		await ReviewAsync(id, (t, now) => t.Accept(now));
		var client = _factory.CreateClientFor(Author);

		string html = await GetSuccessHtmlAsync(client, $"{EditUrl}/{id}");
		Assert.Contains(original, html);
		Assert.Contains("Accepted", html);
		Assert.DoesNotContain("<textarea", html);

		var response = await PostFormAsync(client, IndexUrl, $"{EditUrl}/{id}", FormFor(Unique("tampered")));
		Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
		Assert.Equal(original, (await GetAsync(Author, id))!.Content);
	}

	[Fact]
	public async Task MemberCanDeleteDraft()
	{
		int id = await SeedDraftAsync(Author, Unique("delete-me"));
		var client = _factory.CreateClientFor(Author);

		var response = await PostFormAsync(client, IndexUrl, $"{IndexUrl}?handler=Delete&id={id}", []);

		AssertRedirectsTo(IndexUrl, response);
		Assert.Null(await GetAsync(Author, id));
	}

	[Fact]
	public async Task MembersCannotViewOrChangeAnotherMembersTestimonial()
	{
		string original = Unique("private");
		int id = await SeedDraftAsync(Author, original);
		var intruder = _factory.CreateClientFor(OtherMember);

		Assert.Equal(HttpStatusCode.NotFound, (await intruder.GetAsync($"{EditUrl}/{id}")).StatusCode);
		Assert.Equal(HttpStatusCode.NotFound, (await PostFormAsync(intruder, EditUrl, $"{EditUrl}/{id}", FormFor(Unique("hijack")))).StatusCode);
		Assert.Equal(HttpStatusCode.NotFound, (await PostFormAsync(intruder, IndexUrl, $"{IndexUrl}?handler=Submit&id={id}", [])).StatusCode);
		Assert.Equal(HttpStatusCode.NotFound, (await PostFormAsync(intruder, IndexUrl, $"{IndexUrl}?handler=Delete&id={id}", [])).StatusCode);

		var testimonial = (await GetAsync(Author, id))!;
		Assert.Equal(original, testimonial.Content);
		Assert.Equal(TestimonialStatus.Draft, testimonial.Status);
	}

	private static string Unique(string label) => $"{label}-{Guid.NewGuid():N}";

	private static Dictionary<string, string> FormFor(
		string content,
		string displayName = "Demo User",
		string title = "",
		string organization = "",
		string photoUrl = "") => new()
		{
			["Input.Content"] = content,
			["Input.DisplayName"] = displayName,
			["Input.Title"] = title,
			["Input.Organization"] = organization,
			["Input.PhotoUrl"] = photoUrl,
		};

	private static async Task<string> GetSuccessHtmlAsync(HttpClient client, string url)
	{
		var response = await client.GetAsync(url);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsStringAsync();
	}

	/// <summary>
	/// Loads <paramref name="formPageUrl"/> to obtain an antiforgery token, then posts the form to <paramref name="postUrl"/>.
	/// </summary>
	private static async Task<HttpResponseMessage> PostFormAsync(HttpClient client, string formPageUrl, string postUrl, Dictionary<string, string> fields)
	{
		string html = await GetSuccessHtmlAsync(client, formPageUrl);
		var match = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
		Assert.True(match.Success, $"No antiforgery token found on {formPageUrl}.");

		var form = new Dictionary<string, string>(fields) { ["__RequestVerificationToken"] = match.Groups[1].Value };
		return await client.PostAsync(postUrl, new FormUrlEncodedContent(form));
	}

	private static void AssertRedirectsTo(string expectedPath, HttpResponseMessage response)
	{
		Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
		Assert.Equal(expectedPath, response.Headers.Location!.OriginalString.Split('?')[0]);
	}

	private async Task<int> GetMemberIdAsync(IServiceProvider services, string userName)
	{
		var user = await services.GetRequiredService<UserManager<ApplicationUser>>().FindByNameAsync(userName);
		return services.GetRequiredService<AppDbContext>().Members!.Single(m => m.UserId == user!.Id).Id;
	}

	private async Task<int> SeedDraftAsync(string userName, string content)
	{
		using var scope = _factory.Services.CreateScope();
		int memberId = await GetMemberIdAsync(scope.ServiceProvider, userName);
		var result = await scope.ServiceProvider.GetRequiredService<ITestimonialAuthoring>()
			.CreateDraftAsync(memberId, new TestimonialDraftInput(content, "Demo User", null, null, null));
		Assert.True(result.IsSuccess);
		return result.TestimonialId!.Value;
	}

	private async Task SubmitAsync(string userName, int id)
	{
		using var scope = _factory.Services.CreateScope();
		int memberId = await GetMemberIdAsync(scope.ServiceProvider, userName);
		Assert.True((await scope.ServiceProvider.GetRequiredService<ITestimonialAuthoring>().SubmitAsync(memberId, id)).IsSuccess);
	}

	/// <summary>
	/// Stands in for the admin review queue (#1491) by applying a review transition directly.
	/// </summary>
	private async Task ReviewAsync(int id, Action<Testimonial, DateTimeOffset> review)
	{
		using var scope = _factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<TestimonialsDbContext>();
		var testimonial = db.Testimonials.Single(t => t.Id == id);
		review(testimonial, DateTimeOffset.UtcNow);
		await db.SaveChangesAsync();
	}

	private async Task<IReadOnlyList<MemberTestimonialDto>> ListAsync(string userName)
	{
		using var scope = _factory.Services.CreateScope();
		int memberId = await GetMemberIdAsync(scope.ServiceProvider, userName);
		return await scope.ServiceProvider.GetRequiredService<ITestimonialAuthoring>().ListForMemberAsync(memberId);
	}

	private async Task<MemberTestimonialDto?> GetAsync(string userName, int id)
	{
		using var scope = _factory.Services.CreateScope();
		int memberId = await GetMemberIdAsync(scope.ServiceProvider, userName);
		return await scope.ServiceProvider.GetRequiredService<ITestimonialAuthoring>().GetForMemberAsync(memberId, id);
	}
}
