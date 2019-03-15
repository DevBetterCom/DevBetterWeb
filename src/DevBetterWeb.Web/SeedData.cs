using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;
using System;
using System.Linq;

namespace CleanArchitecture.Web
{
    public static class SeedData
    {
        public static void PopulateTestData(AppDbContext dbContext)
        {
            if (dbContext.ArchiveVideos.Any()) return;

            var vid1 = new ArchiveVideo()
            {
                Title = "Video One",
                DateCreated = new DateTime(2019, 3, 8)
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

            dbContext.ArchiveVideos.Add(vid1);
            dbContext.ArchiveVideos.Add(vid2);
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
