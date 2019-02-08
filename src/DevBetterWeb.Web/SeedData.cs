using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;
using System;

namespace CleanArchitecture.Web
{
    public static class SeedData
    {
        public static void PopulateTestData(AppDbContext dbContext)
        {
            var toDos = dbContext.ToDoItems;
            foreach (var item in toDos)
            {
                dbContext.Remove(item);
            }
            dbContext.SaveChanges();
            dbContext.ToDoItems.Add(new ToDoItem()
            {
                Title = "Test Item 1",
                Description = "Test Description One"
            });
            dbContext.ToDoItems.Add(new ToDoItem()
            {
                Title = "Test Item 2",
                Description = "Test Description Two"
            });

            dbContext.ArchiveVideos.Add(new ArchiveVideo()
            {
                DateCreated = DateTime.Now,
                Title = "Test Video",
                VideoUrl = "http://youtube.com"
            });
            dbContext.SaveChanges();
        }

    }
}
