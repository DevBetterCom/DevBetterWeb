using System;
using System.Linq;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web;
using DevBetterWeb.Web.Areas.Identity;
using DevBetterWeb.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.FunctionalTests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Program>
{
	/// <summary>
	/// Overriding CreateHost to avoid creating a separate ServiceProvider per this thread:
	/// https://github.com/dotnet-architecture/eShopOnWeb/issues/465
	/// </summary>
	/// <param name="builder"></param>
	/// <returns></returns>
	protected override IHost CreateHost(IHostBuilder builder)
	{
		// disable sql logging in tests
		builder.ConfigureLogging(logging =>
		{
			logging.ClearProviders();
			logging.AddConsole();
		});
		var host = builder.Build();

		// Get service provider.
		var serviceProvider = host.Services;

		// Create a scope to obtain a reference to the database
		// context (AppDbContext).
		using (var scope = serviceProvider.CreateScope())
		{
			var scopedServices = scope.ServiceProvider;
			var db = scopedServices.GetRequiredService<AppDbContext>();
			var identitydb = scopedServices.GetRequiredService<IdentityDbContext>();
			var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
			var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

			var logger = scopedServices
					.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

			// Ensure the database is created.
			db.Database.EnsureCreated();
			identitydb.Database.EnsureCreated();

			try
			{
				// Seed the database with test data.
				AppIdentityDbContextSeed.SeedAsync(userManager, roleManager).GetAwaiter().GetResult();

				SeedData.PopulateTestData(db, userManager);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred seeding the " +
														$"database with test messages. Error: {ex.Message}");
			}
		}

		host.Start();
		return host;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder
			.UseSolutionRelativeContentRoot("src/DevBetterWeb.Web")
			.UseEnvironment("Testing")
			.ConfigureServices(services =>
			{
				string inMemoryAppDb = Guid.NewGuid().ToString();
				string inMemoryIdentityDb = Guid.NewGuid().ToString();

				services.AddDbContext<AppDbContext>(options =>
				{
					options.UseInMemoryDatabase(inMemoryAppDb);
					options.UseInternalServiceProvider(new ServiceCollection()
						.AddEntityFrameworkInMemoryDatabase()
						.BuildServiceProvider());
				});

				services.AddDbContext<IdentityDbContext>(options =>
				{
					options.UseInMemoryDatabase(inMemoryIdentityDb);
					options.UseInternalServiceProvider(new ServiceCollection()
						.AddEntityFrameworkInMemoryDatabase()
						.BuildServiceProvider());
				});

				services.AddIdentityCore<ApplicationUser>(options =>
				{
					options.User.RequireUniqueEmail = false;
				})
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<IdentityDbContext>()
				.AddSignInManager()
				.AddDefaultTokenProviders();

				services.AddScoped<IMediator, NoOpMediator>();
				services.AddScoped<IDomainEventDispatcher, NoOpDomainEventDispatcher>();
			});

	}

	private void RemoveAll<T>(IServiceCollection services)
	{
		var descriptors = services.Where(d =>
			d.ServiceType == typeof(T) ||
			(d.ServiceType.IsGenericType && d.ServiceType.GetGenericTypeDefinition() == typeof(T))).ToList();

		foreach (var descriptor in descriptors)
		{
			services.Remove(descriptor);
		}
	}

}
