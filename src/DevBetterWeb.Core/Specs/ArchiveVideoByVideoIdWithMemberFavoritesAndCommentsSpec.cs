using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class ArchiveVideoByVideoIdWithMemberFavoritesAndCommentsSpec : Specification<ArchiveVideo>, ISingleResultSpecification
{
  public ArchiveVideoByVideoIdWithMemberFavoritesAndCommentsSpec(string videoId)
  {
    Query
      .Where(x => x.VideoId == videoId)
      .Include(X => X.MemberFavorites)
      .Include(x => x.Comments)
        .ThenInclude(x => x.MemberWhoCreate)
      .Include(x => x.Comments)
        .ThenInclude(x => x.Replies);

	}
}
