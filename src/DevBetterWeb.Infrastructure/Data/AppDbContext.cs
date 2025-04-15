using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DevBetterWeb.Infrastructure.Data;

public class AppDbContext : DbContext
{
	private readonly IDomainEventDispatcher? _dispatcher;

	public AppDbContext(DbContextOptions<AppDbContext> options,
			IDomainEventDispatcher? dispatcher)
			: base(options)
	{
		_dispatcher = dispatcher;
	}	
	
	public AppDbContext(DbContextOptions<AppDbContext> options)
			: this(options, null!)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}

	public DbSet<ArchiveVideo>? ArchiveVideos { get; set; }
	public DbSet<VideoComment>? VideoComments { get; set; }
	public DbSet<Question>? Questions { get; set; }
	public DbSet<Member>? Members { get; set; }
	public DbSet<Book>? Books { get; set; }
	public DbSet<MemberSubscription>? MemberSubscriptions { get; set; }
	public DbSet<Invitation>? Invitations { get; set; }
	public DbSet<BillingActivity>? BillingActivities { get; set; }
	public DbSet<DailyCheck>? DailyChecks { get; set; }
	public DbSet<MemberSubscriptionPlan>? MemberSubscriptionPlan { get; set; }
	public DbSet<MemberVideoProgress>? MembersVideosProgress { get; set; }
	public DbSet<CoachingSession>? CoachingSessions { get; set; }
	public DbSet<BookCategory>? BookCategories { get; set; }

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
	{
		// TODO: See why were doing these before saving - find all handlers of MemberAddressUpdatedEvent
		//if (_dispatcher is not null)
		//{
		//	var membersWithAddressUpdatedEvents = ChangeTracker.Entries<Member>()
		//	 .Select(e => e.Entity)
		//	 .Where(e => e.DomainEvents.Any(x => x.GetType() == typeof(MemberAddressUpdatedEvent)))
		//	 .ToArray();

		//	foreach (var member in membersWithAddressUpdatedEvents)
		//	{
		//		var addressUpdatedEvents = member.DomainEvents
		//			.Where(e => e.GetType() == typeof(MemberAddressUpdatedEvent))
		//			.ToArray();

		//		member.DomainEvents
		//			.Where(e => e.GetType() == typeof(MemberAddressUpdatedEvent))
		//			.ToList()
		//			.Clear();

		//			await _dispatcher.DispatchAndClearEvents(addressUpdatedEvents!).ConfigureAwait(false);
		//	}
		//}

		int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		// ignore events if no dispatcher provided
		if (_dispatcher == null) return result;

		// dispatch events only if save was successful
		var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
				.Select(e => e.Entity)
				.Where(e => e.DomainEvents.Any())
				.ToArray();
		await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

		return result;
	}

	public override int SaveChanges()
	{
		return SaveChangesAsync().GetAwaiter().GetResult();
	}
}


// chatgpt made this - it allowed dotnet ef database update to work locally
//public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
//{
//	public AppDbContext CreateDbContext(string[] args)
//	{
//		var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

//		// Replace with your actual connection string
//		var connectionString = "Server=localhost\\SQLEXPRESS;TrustServerCertificate=true;Database=DevBetterWeb.App;Trusted_Connection=True;MultipleActiveResultSets=true";
//		optionsBuilder.UseSqlServer(connectionString);

//		return new AppDbContext(optionsBuilder.Options);
//	}
//}

