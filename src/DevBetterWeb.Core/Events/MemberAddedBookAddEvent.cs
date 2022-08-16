using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class MemberAddedBookAddEvent : BaseDomainEvent
{
  public MemberAddedBookAddEvent(Member member, Book book)
  {
    Member = member;
    Book = book;
  }

  public Member Member { get; }
  public Book Book { get; }
}
