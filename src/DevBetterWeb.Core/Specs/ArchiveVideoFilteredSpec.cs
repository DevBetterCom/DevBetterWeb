using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class ArchiveVideoFilteredSpec : Specification<ArchiveVideo>
{
  public ArchiveVideoFilteredSpec(string? search)
  {
    if (string.IsNullOrEmpty(search))
    {
      Query
	      .OrderByDescending(x => x.DateCreated);
    }
    else
    {
      Query
        .Where(s => (s.Description != null && s.Description.Contains(search)) || 
                    (s.Title != null && s.Title.Contains(search)))
        .OrderByDescending(x => x.DateCreated);
    }
  }
}
