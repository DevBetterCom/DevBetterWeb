using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class BookUpdatedEvent : BaseDomainEvent
{
  public BookUpdatedEvent(Book book, string updateDetails)
  {
    Book = book;
    UpdateDetails = updateDetails;
  }

  public Book Book { get; }
  public string UpdateDetails { get; internal set; }
}
