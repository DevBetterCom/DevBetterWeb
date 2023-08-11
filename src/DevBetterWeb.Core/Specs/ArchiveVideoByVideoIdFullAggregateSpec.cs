using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class ArchiveVideoByVideoIdFullAggregateSpec : Specification<ArchiveVideo>, ISingleResultSpecification<ArchiveVideo>
{
  public ArchiveVideoByVideoIdFullAggregateSpec(string videoId)
  {
    Query
      .Where(x => x.VideoId == videoId)
      .Include(X => X.MemberFavorites)
      .Include(x => x.Comments)
        .ThenInclude(x => x.MemberWhoCreate)
      .Include(x => x.Comments)
        .ThenInclude(x => x.Replies)
      .Include(x => x.MembersVideoProgress);
  }
}
