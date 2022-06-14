using System.Linq;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class ArchiveVideoByVideoIdWithMemberFavoritesSpec : Specification<ArchiveVideo>, ISingleResultSpecification
{
  public ArchiveVideoByVideoIdWithMemberFavoritesSpec(string videoId)
  {
    Query
      .Where(x => x.VideoId == videoId)
      .Include(X => X.MemberFavorites);
  }
}
