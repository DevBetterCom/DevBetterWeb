using System;
using System.Linq;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Areas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

    dbContext.Books!.Add(new Book
    {
	    Author = "Test 2",
	    Title = "Test 2",
	    PurchaseUrl = "https://ardalis.com",
	    Details = "B classic."
    });
    dbContext.SaveChanges();

    dbContext.Books!.Add(new Book
    {
	    Author = "Test 3",
	    Title = "Test 3",
	    PurchaseUrl = "https://ardalis.com",
	    Details = "C classic."
    });
    dbContext.SaveChanges();

    dbContext.Books!.Add(new Book
    {
	    Author = "Test 4",
	    Title = "Test 4",
	    PurchaseUrl = "https://ardalis.com",
	    Details = "D classic."
    });
    dbContext.SaveChanges();

		AssignBooksToCategories(dbContext);
		AddReadBooks(dbContext);

		_ = AppIdentityDbContextSeed.RemoveUserFromRoleAsync(userManager, "non-member@microsoft.com", Constants.MEMBER_ROLE_NAME).Result;
  }

	private static void AddReadBooks(AppDbContext dbContext)
	{
		var members = dbContext.Members!;
		var books = dbContext.Books!;

		foreach (var member in members)
		{
			foreach (var book in books)
			{
				member.AddBookRead(book);
			}
			dbContext.Members!.Update(member);
		}

		dbContext.SaveChanges();
	}

	private static void AddMembers(AppDbContext dbContext,
    UserManager<ApplicationUser> userManager)
  {
    var regularUser = userManager.FindByNameAsync("demouser@microsoft.com").GetAwaiter().GetResult();
    var regularMember = Member.SeedData(regularUser!.Id, "Demo", "User");
    dbContext.Members?.Add(regularMember);

    var regularUser2 = userManager.FindByNameAsync("demouser2@microsoft.com").GetAwaiter().GetResult();
    var regularMember2 = Member.SeedData(regularUser2!.Id, "Demo2", "User2");
    dbContext.Members?.Add(regularMember2);

    var regularUser3 = userManager.FindByNameAsync("demouser3@microsoft.com").GetAwaiter().GetResult();
    var regularMember3 = Member.SeedData(regularUser3!.Id, "Demo3", "User3");
    dbContext.Members?.Add(regularMember3);

    var regularUser4 = userManager.FindByNameAsync("demouser4@microsoft.com").GetAwaiter().GetResult();
    var regularMember4 = Member.SeedData(regularUser4!.Id, "Demo4", "User4");
    dbContext.Members?.Add(regularMember4);

    var nonMember = userManager.FindByNameAsync("non-member@microsoft.com").GetAwaiter().GetResult();
    var nonMemberUser = Member.SeedData(nonMember!.Id, "non-member", "non-member");
    dbContext.Members?.Add(nonMemberUser);

		var adminUser = userManager.FindByNameAsync("admin@test.com").GetAwaiter().GetResult();
    var adminMember = Member.SeedData(adminUser!.Id, "Admin", "User");
    dbContext.Members?.Add(adminMember);

		var alumniUser = userManager.FindByNameAsync("alumni@test.com").GetAwaiter().GetResult();
    var alumniMember = Member.SeedData(alumniUser!.Id, "Alumni", "User");
    dbContext.Members?.Add(alumniMember);

    var alumniUser2 = userManager.FindByNameAsync("alumni2@test.com").GetAwaiter().GetResult();
    var alumniMember2 = Member.SeedData(alumniUser2!.Id, "Alumni2", "User2");
    dbContext.Members?.Add(alumniMember2);

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

	  AssignBooksToCategories(dbContext);
  }

	private static void AssignBooksToCategories(AppDbContext dbContext)
	{
		var books = dbContext.Books!;
		var cnt = 0;
		foreach (var book in books)
		{
			book.BookCategoryId = 1;
			if (cnt > 1)
			{
				book.BookCategoryId = 2;
			}
			dbContext.Books!.Update(book);
			cnt++;
		}

		dbContext.SaveChanges();
	}
}
