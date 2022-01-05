using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class ArchiveVideoFilteredSpec : Specification<ArchiveVideo>
{
  public ArchiveVideoFilteredSpec(string? search)
  {
    if (string.IsNullOrEmpty(search))
    {
      Query.OrderByDescending(x => x.DateCreated);
    }
    else
    {
      Query
        .Where(s => s.ShowNotes != null && !string.IsNullOrEmpty(search) && s.ShowNotes.Contains(search))
          .OrderByDescending(x => x.DateCreated);
    }
  }
}
