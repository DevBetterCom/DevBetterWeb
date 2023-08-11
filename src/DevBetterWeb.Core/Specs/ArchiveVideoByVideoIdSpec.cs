using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class ArchiveVideoByVideoIdSpec : Specification<ArchiveVideo>, ISingleResultSpecification<ArchiveVideo>
{
  public ArchiveVideoByVideoIdSpec(string videoId)
  {
    Query
      .Where(x => x.VideoId == videoId);
  }
}
