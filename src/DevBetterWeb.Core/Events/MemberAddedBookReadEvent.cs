using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
  public class MemberAddedBookReadEvent : BaseDomainEvent
  {
    public MemberAddedBookReadEvent(Member member, Book book)
    {
      Member = member;
      Book = book;
    }

    public Member Member { get; }
    public Book Book { get; }
  }
}
