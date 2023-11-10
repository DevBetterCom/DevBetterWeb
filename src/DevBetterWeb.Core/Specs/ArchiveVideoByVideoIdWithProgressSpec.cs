using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class ArchiveVideoByVideoIdWithProgressSpec : Specification<ArchiveVideo>, 
	ISingleResultSpecification<ArchiveVideo>
{
  public ArchiveVideoByVideoIdWithProgressSpec(string videoId)
  {
    Query
      .Where(x => x.VideoId == videoId)
      .Include(x => x.MembersVideoProgress);
  }
}
