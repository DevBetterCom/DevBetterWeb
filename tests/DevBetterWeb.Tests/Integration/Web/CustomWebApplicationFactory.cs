using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web;
using DevBetterWeb.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;

namespace DevBetterWeb.Tests.Integration.Web
{
  public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.ConfigureServices(services =>
      {
        // Create a new service provider.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        // Add a database context (AppDbContext) using an in-memory
        // database for testing.
        services.AddDbContext<AppDbContext>(options =>
        {
          options.UseInMemoryDatabase("InMemoryDbForTesting");
          options.UseInternalServiceProvider(serviceProvider);
        });

        services.AddScoped<IDomainEventDispatcher, NoOpDomainEventDispatcher>();

        // Build the service provider.
        var sp = services.BuildServiceProvider();

        // Create a scope to obtain a reference to the database
        // context (AppDbContext).
        using (var scope = sp.CreateScope())
        {
          var scopedServices = scope.ServiceProvider;
          var db = scopedServices.GetRequiredService<AppDbContext>();

          var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

          // Ensure the database is created.
          db.Database.EnsureCreated();

          try
          {
            // Seed the database with test data.
            SeedData.PopulateTestData(db, scopedServices.GetRequiredService<UserManager<ApplicationUser>>());
          }
          catch (Exception ex)
          {
            logger.LogError(ex, "An error occurred seeding the " +
                      "database with test messages. Error: {ex.Message}");
          }
        }
      });
    }
  }

  //public class StripeWebhookTestWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
  //{
  //  public Mock<ILogger<StripeWebhookHandler>> _logger {get; private set;}
  //  public Mock<INewMemberService> _newMemberService { get; private set; }
  //  public Mock<IPaymentHandlerSubscription> _paymentHandlerSubscription { get; private set; }
  //  public Mock<IPaymentHandlerCustomer> _paymentHandlerCustomer { get; private set; }
    
  //  public StripeWebhookHandler _stripeWebhookHandler { get; private set; }

  //  private string _customerId = "cus_IuWbmpXNfiznUM";
  //  private string _email = "testemail@testemail.com";
  //  private string _stripeEventId = "TestStripeId";
  //  private string _inviteCode = "TestInviteCode";

  //  private Invitation _invitation;

  //  public StripeWebhookTestWebApplicationFactory()
  //  {
  //    _invitation = new Invitation(_email, _inviteCode, _stripeEventId);

  //    _logger = new Mock<ILogger<StripeWebhookHandler>>();
  //    _newMemberService = new Mock<INewMemberService>();
  //    _paymentHandlerSubscription = new Mock<IPaymentHandlerSubscription>();
  //    _paymentHandlerCustomer = new Mock<IPaymentHandlerCustomer>();

  //    _paymentHandlerCustomer.Setup(p => p.GetCustomerEmail(_customerId)).Returns(_email);
  //    _newMemberService.Setup(n => n.CreateInvitation(_email, _stripeEventId)).ReturnsAsync(_invitation);
  //    _newMemberService.Setup(n => n.SendRegistrationEmail(_invitation)).Returns(Task.CompletedTask);

  //    _stripeWebhookHandler = new StripeWebhookHandler
  //      (_logger.Object,
  //      _newMemberService.Object,
  //      _paymentHandlerSubscription.Object,
  //      _paymentHandlerCustomer.Object);
  //  }

  //  protected override void ConfigureWebHost(IWebHostBuilder builder)
  //  {
  //    builder.ConfigureServices(services =>
  //    {
  //      // Create a new service provider.
  //      var serviceProvider = new ServiceCollection()
  //          .AddEntityFrameworkInMemoryDatabase()
  //          .BuildServiceProvider();

  //      // Add a database context (AppDbContext) using an in-memory
  //      // database for testing.
  //      services.AddDbContext<AppDbContext>(options =>
  //      {
  //        options.UseInMemoryDatabase("InMemoryDbForTesting");
  //        options.UseInternalServiceProvider(serviceProvider);
  //      });

  //      services.AddScoped<IDomainEventDispatcher, NoOpDomainEventDispatcher>();

  //      // Build the service provider.
  //      var sp = services.BuildServiceProvider();

  //      // Create a scope to obtain a reference to the database
  //      // context (AppDbContext).
  //      using (var scope = sp.CreateScope())
  //      {
  //        var scopedServices = scope.ServiceProvider;
  //        var db = scopedServices.GetRequiredService<AppDbContext>();

  //        var logger = scopedServices
  //                  .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

  //        // Ensure the database is created.
  //        db.Database.EnsureCreated();

  //        try
  //        {
  //          // Seed the database with test data.
  //          SeedData.PopulateTestData(db);
  //        }
  //        catch (Exception ex)
  //        {
  //          logger.LogError(ex, "An error occurred seeding the " +
  //                    "database with test messages. Error: {ex.Message}");
  //        }
  //      }
  //    });
  //  }
  //}
}
