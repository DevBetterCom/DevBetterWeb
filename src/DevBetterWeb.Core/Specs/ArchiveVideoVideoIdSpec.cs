using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class ArchiveVideoVideoIdSpec : Specification<ArchiveVideo>
{
  public ArchiveVideoVideoIdSpec()
  {
    Query
      .Where(s => !string.IsNullOrEmpty(s.VideoId));
  }
}
