using System.Linq;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;
public sealed class ArchiveVideoByPageSpec : Specification<ArchiveVideo>
{
  public ArchiveVideoByPageSpec(int skip, int size, string? search, bool filterFavorites, int memberId)
  {
    Query.Include(x => x.MemberFavorites);

	if (filterFavorites)
    {
			Query
			  .Where(x => x.MemberFavorites.Any(m => m.MemberId == memberId));
    }

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
        .Where(s => (s.Description != null && s.Description.Contains(search)) ||
                    (s.Title != null && s.Title.Contains(search)))
        .OrderByDescending(x => x.DateCreated)
        .Skip(skip)
        .Take(size);
    }
  }
}
