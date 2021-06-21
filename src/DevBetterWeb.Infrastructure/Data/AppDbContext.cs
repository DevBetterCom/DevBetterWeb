using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace DevBetterWeb.Infrastructure.Data
{
  public class AppDbContext : DbContext
  {
    private readonly IDomainEventDispatcher _dispatcher;

    public AppDbContext(DbContextOptions<AppDbContext> options,
        IDomainEventDispatcher dispatcher)
        : base(options)
    {
      _dispatcher = dispatcher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public DbSet<ArchiveVideo>? ArchiveVideos { get; set; }
    public DbSet<Question>? Questions { get; set; }
    public DbSet<Member>? Members { get; set; }
    public DbSet<Book>? Books { get; set; }
    public DbSet<MemberSubscription>? MemberSubscriptions { get; set; }
    public DbSet<Invitation>? Invitations { get; set; }
    public DbSet<BillingActivity>? BillingActivities { get; set; }
    public DbSet<DailyCheck>? DailyChecks { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
      if (_dispatcher is not null)
      {
        var membersWithAddressUpdatedEvents = ChangeTracker.Entries<Member>()
         .Select(e => e.Entity)
         .Where(e => e.Events.Any(x => x.GetType() == typeof(MemberAddressUpdatedEvent)))
         .ToArray();

        foreach (var member in membersWithAddressUpdatedEvents)
        {
          var addressUpdatedEvents = member.Events
            .Where(e => e.GetType() == typeof(MemberAddressUpdatedEvent))
            .ToArray();

          member.Events
            .Where(e => e.GetType() == typeof(MemberAddressUpdatedEvent))
            .ToList()
            .Clear();

          foreach (var addressUpdatedEvent in addressUpdatedEvents)
          {
            await _dispatcher.Dispatch(addressUpdatedEvent).ConfigureAwait(false);
          }
        }
      }

      int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

      // ignore events if no dispatcher provided
      if (_dispatcher == null) return result;

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
          await _dispatcher.Dispatch(domainEvent).ConfigureAwait(false);
        }
      }

      return result;
    }

    public override int SaveChanges()
    {
      return SaveChangesAsync().GetAwaiter().GetResult();
    }
  }
}
