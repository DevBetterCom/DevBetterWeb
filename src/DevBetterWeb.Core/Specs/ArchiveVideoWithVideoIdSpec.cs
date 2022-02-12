using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class ArchiveVideoWithVideoIdSpec : Specification<ArchiveVideo>
{
  public ArchiveVideoWithVideoIdSpec()
  {
    Query
      .Where(s => !string.IsNullOrEmpty(s.VideoId));
  }
}
