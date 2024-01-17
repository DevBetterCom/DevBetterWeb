using System.Collections.Generic;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class BookCategory : BaseEntity, IAggregateRoot
{
  public string? Title { get; set; }
  public List<Book> Books { get; private set; } = new List<Book>();

  public override string ToString()
  {
    return Title + string.Empty;
  }
}
