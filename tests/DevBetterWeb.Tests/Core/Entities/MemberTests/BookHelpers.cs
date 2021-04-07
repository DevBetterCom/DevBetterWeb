using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.ValueObjects;
using System;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
  public static class BookHelpers
  {
    public static Book GetDefaultTestBook()
    {
      return new Book
      {
        Title = "Book",
        Author = "Ilyana Smith",
        Details = "this is a book",
        PurchaseUrl = "https://buyabook.com"
      };
    }
    public static Book GetDefaultTestBookWithSpecifiedTitle(string title)
    {
      return new Book
      {
        Title = title,
        Author = "Ilyana Smith",
        Details = "this is a book",
        PurchaseUrl = "https://buyabook.com"
      };
    }
  }

  public static class SubscriptionHelpers
  {
    public static Subscription GetDefaultTestSubscription()
    {
      return new Subscription
      {
        Dates = new DateTimeRange(new DateTime(2020, 3, 14), DateTime.Today.AddDays(20))
      };
    }

    public static Subscription GetSubscriptionWithPastEndDate()
    {
      return new Subscription
      {
        Dates = new DateTimeRange(new DateTime(2020, 3, 14), DateTime.Today.AddDays(-20))
      };
    }
  }
}

