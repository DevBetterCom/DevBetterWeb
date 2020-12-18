using System.Collections.Generic;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities
{
  public class Book : BaseEntity
  {
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Details { get; set; }
    public string? PurchaseUrl { get; set; }
    public List<Member>? MembersWhoHaveRead { get; set; } = new List<Member>();

    public override string ToString()
    {
      return Title + " by " + Author;
    }
  }
}
