using System.Collections.Generic;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class Book : BaseEntity, IAggregateRoot
{
  public string? Title { get; set; }
  public string? Author { get; set; }
	public int? MemberWhoUploadId { get; set; }
	public string? Details { get; set; }
  public string? PurchaseUrl { get; set; }
  public int? BookCategoryId { get; set; }
  public List<Member>? MembersWhoHaveRead { get; set; } = new List<Member>();
	public Member? MemberWhoUpload { get; set; }
  public BookCategory? BookCategory { get; set; }

	public void AddDomainEvent(NewBookCreatedEvent bookAddedEvent)
	{
		this.RegisterDomainEvent(bookAddedEvent);
	}

	public override string ToString()
  {
    return Title + " by " + Author;
  }
}
