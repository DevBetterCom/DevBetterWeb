using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;
public class ArchiveVideoByPageSpec : Specification<ArchiveVideo>
{
  public ArchiveVideoByPageSpec(int skip, int size, string? search)
  {
    if (string.IsNullOrEmpty(search))
    {
      Query
        .OrderByDescending(x => x.DateCreated)
        .Skip(skip)
        .Take(size);
    }
    else
    {
      Query
        .Where(s => s.ShowNotes != null && !string.IsNullOrEmpty(search) && s.ShowNotes.Contains(search))
        .OrderByDescending(x => x.DateCreated)
        .Skip(skip)
        .Take(size);
    }
  }
}
