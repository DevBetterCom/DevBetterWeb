using System;
using System.Linq;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web;

public static class SeedData
{
  public static void PopulateTestData(AppDbContext dbContext,
    UserManager<ApplicationUser> userManager)
  {
    if (dbContext.ArchiveVideos!.Any()) return;

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
    var questionA = new Question()
    {
      QuestionText = "How do I A?",
      TimestampSeconds = 30
    };
    var questionB = new Question()
    {
      QuestionText = "How do I B?",
      TimestampSeconds = 245
    };
    vid1.Questions.Add(questionA);
    vid1.Questions.Add(questionB);

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

    AddMembers(dbContext, userManager);
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
    dbContext.Members?.Add(adminMember);

    dbContext.SaveChanges();
  }
}
