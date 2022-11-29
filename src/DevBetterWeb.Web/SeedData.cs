using System;
using System.Linq;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web;

public static class SeedData
{
	public static void PopulateInitData(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
	{
		PopulateBookCategoriesInitData(dbContext);
		PopulateMembersInitData(dbContext, userManager);
		PopulateBookUploaderMemberInitData(dbContext, userManager);
	}

	public static void PopulateTestData(AppDbContext dbContext,
    UserManager<ApplicationUser> userManager)
  {
    if (dbContext.ArchiveVideos!.Any()) return;

    AddMembers(dbContext, userManager);

    var questionA = new Question(1, "How do I A?");
    var questionB = new Question(1, "How do I B?");
    var coachingSession = new CoachingSession(DateTime.UtcNow);
    coachingSession.AddQuestion(questionA);
    coachingSession.AddQuestion(questionB);
    dbContext.CoachingSessions!.Add(coachingSession);

    var vid1 = new ArchiveVideo()
    {
      Title = "Video One",
      Description = @"
# Video about using markdown in ASP.NET Core

In this video we talk about some stuff. In this video we talk about some stuff. In this video we talk about some stuff. In this video we talk about some stuff. In this video we talk about some stuff. In this video we talk about some stuff. 

<a href="" "" class=""ts"" data-time=""20"">00:20</a>

<a href = "" "" class=""ts"" data-time=""40"">00:40</a>

<a href = "" "" class=""ts"" data-time=""60"">00:60</a>

## References

- [Steve's Blog](https://ardalis.com)
- [Google](https://google.com)
",
      DateCreated = new DateTime(2019, 3, 8),
      VideoUrl = "2019-05-17-3.mp4"
    };
    var vid2 = new ArchiveVideo()
    {
      Title = "Video Two",
      DateCreated = new DateTime(2019, 3, 15)
    };

    dbContext.ArchiveVideos!.Add(vid1);
    dbContext.ArchiveVideos.Add(vid2);

    dbContext.Books!.Add(new Book
    {
      Author = "Steve Smith",
      Title = "ASP.NET By Example",
      PurchaseUrl = "https://ardalis.com",
      Details = "A classic."
    });
    dbContext.SaveChanges();
  }

  private static void AddMembers(AppDbContext dbContext,
    UserManager<ApplicationUser> userManager)
  {
    var regularUser = userManager.FindByNameAsync("demouser@microsoft.com").GetAwaiter().GetResult();
    var regularMember = Member.SeedData(regularUser.Id, "Demo", "User");
    dbContext.Members?.Add(regularMember);

	var adminUser = userManager.FindByNameAsync("admin@test.com").GetAwaiter().GetResult();
    var adminMember = Member.SeedData(adminUser.Id, "Admin", "User");
    dbContext.Members?.Add(adminMember);

	var alumniUser = userManager.FindByNameAsync("alumni@test.com").GetAwaiter().GetResult();
    var alumniMember = Member.SeedData(alumniUser.Id, "Alumni", "User");
    dbContext.Members?.Add(alumniMember);

    dbContext.SaveChanges();
  }

  private static void PopulateMembersInitData(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
  {
	  if (dbContext.Members!.Any()) return;

	  var members = dbContext.Members!.Where(member => string.IsNullOrEmpty(member.Email)).ToList();
	  foreach (var member in members)
	  {
		  var user = userManager.Users.FirstOrDefault(user => user.Id == member.UserId);
		  if (string.IsNullOrEmpty(user?.Email))
		  {
				continue;
		  }

		  member.UpdateEmail(user.Email);
		  dbContext.Members!.Update(member);
	  }

	  dbContext.SaveChanges();
  }
	
	private static void PopulateBookUploaderMemberInitData(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
  {
	  if (!dbContext.Books!.Any()) return;

	  var adminMember = dbContext.Members!.FirstOrDefault(member => !string.IsNullOrEmpty(member.DiscordUsername) && member.DiscordUsername!.Contains("9871"));
		if (adminMember == null) return;	

	  var books = dbContext.Books!.Where(b => b.MemberWhoUploadId == null).ToList();
		foreach(var book in books)
		{
			book.MemberWhoUploadId = adminMember.Id;
			dbContext.Books!.Update(book);
		}	  

	  dbContext.SaveChanges();
  }

	private static void PopulateBookCategoriesInitData(AppDbContext dbContext)
  {
	  if (dbContext.BookCategories!.Any()) return;

	  var softwareDevelopmentCategory = new BookCategory { Title = "Software Development" };
	  dbContext.BookCategories!.Add(softwareDevelopmentCategory);

	  var personalCareerCategory = new BookCategory { Title = "Personal/Career" };
	  dbContext.BookCategories!.Add(personalCareerCategory);
	  dbContext.SaveChanges();

		var books = dbContext.Books!;
		foreach (var book in books)
		{
			book.BookCategoryId = 1;
			dbContext.Books!.Update(book);
		}
		
		dbContext.SaveChanges();
  }
}
