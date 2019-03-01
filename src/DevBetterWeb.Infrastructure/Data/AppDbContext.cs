﻿using CleanArchitecture.Core.Entities;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CleanArchitecture.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _dispatcher;

        public AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher dispatcher)
            : base(options)
        {
            _dispatcher = dispatcher;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Question>().ToTable("Question");
        }

        public DbSet<ToDoItem> ToDoItems { get; set; }
        public DbSet<ArchiveVideo> ArchiveVideos { get; set; }
        public DbSet<Question> Questions { get; set; }

        public override int SaveChanges()
        {
            int result = base.SaveChanges();

            // dispatch events only if save was successful
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.Events.Any())
                .ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events)
                {
                    _dispatcher.Dispatch(domainEvent);
                }
            }

            return result;
        }
    }
}
