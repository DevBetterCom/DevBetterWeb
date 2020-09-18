using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using System;
using System.Linq;

namespace DevBetterWeb.Web
{
    public static class SeedData
    {
        public static void PopulateTestData(AppDbContext dbContext)
        {
            if (dbContext.ArchiveVideos.Any()) return;

            var vid1 = new ArchiveVideo()
            {
                Title = "Video One",
                ShowNotes = @"
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

            //dbContext.ArchiveVideos.Add(new ArchiveVideo()
            //{
            //    DateCreated = DateTime.Now,
            //    Title = "Test Video",
            //    VideoUrl = "http://youtube.com"
            //});
            //dbContext.SaveChanges();
        }

    }
}
