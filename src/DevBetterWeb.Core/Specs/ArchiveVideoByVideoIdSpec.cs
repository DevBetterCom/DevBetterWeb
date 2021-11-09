using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class ArchiveVideoByVideoIdSpec : Specification<ArchiveVideo>, ISingleResultSpecification
{
  public ArchiveVideoByVideoIdSpec(string videoId)
  {
    Query
      .Where(x => x.VideoId == videoId);
  }
}
