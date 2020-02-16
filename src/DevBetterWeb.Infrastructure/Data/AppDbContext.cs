using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DevBetterWeb.Infrastructure.Data
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
            modelBuilder.Entity<Member>().Property(x => x.Address).HasMaxLength(500);
            modelBuilder.Entity<Member>().Property(x => x.BlogUrl).HasMaxLength(200);
            modelBuilder.Entity<Member>().Property(x => x.GithubUrl).HasMaxLength(200);
            modelBuilder.Entity<Member>().Property(x => x.LinkedInUrl).HasMaxLength(200);
            modelBuilder.Entity<Member>().Property(x => x.OtherUrl).HasMaxLength(200);
            modelBuilder.Entity<Member>().Property(x => x.TwitchUrl).HasMaxLength(200);
            modelBuilder.Entity<Member>().Property(x => x.TwitterUrl).HasMaxLength(200);
            modelBuilder.Entity<Member>().Property(x => x.FirstName).HasMaxLength(100);
            modelBuilder.Entity<Member>().Property(x => x.LastName).HasMaxLength(100);
            modelBuilder.Entity<Member>().HasKey(x => x.UserId);
        }

        public DbSet<ArchiveVideo> ArchiveVideos { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Member> Members { get; set; }

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
