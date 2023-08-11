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
				//.UseEnvironment("Testing")
				.ConfigureServices(services =>
				{
					// Remove the app's ApplicationDbContext registration.
					var descriptor = services.SingleOrDefault(
						d => d.ServiceType ==
								typeof(DbContextOptions<AppDbContext>));

					if (descriptor != null)
					{
						services.Remove(descriptor);
					}

					// This should be set for each individual test run
					string inMemoryCollectionName = Guid.NewGuid().ToString();

					// Add ApplicationDbContext using an in-memory database for testing.
					services.AddDbContext<AppDbContext>(options =>
				{
					options.UseInMemoryDatabase(inMemoryCollectionName);
				});

					// Remove the app's IdentityDbContext registration.
					var descriptorIdentityDbContext = services.SingleOrDefault(
						d => d.ServiceType ==
								typeof(DbContextOptions<IdentityDbContext>));

					if (descriptorIdentityDbContext != null)
					{
						services.Remove(descriptorIdentityDbContext);
					}

					// This should be set for each individual test run
					string inMemoryCollectionNameIdentityDbContext = Guid.NewGuid().ToString();

					// Add IdentityDbContext using an in-memory database for testing.
					services.AddDbContext<IdentityDbContext>(options =>
				{
					options.UseInMemoryDatabase(inMemoryCollectionNameIdentityDbContext);
				});

					services.AddScoped<IMediator, NoOpMediator>();
					services.AddScoped<IDomainEventDispatcher, NoOpDomainEventDispatcher>();
				});
	}
}
