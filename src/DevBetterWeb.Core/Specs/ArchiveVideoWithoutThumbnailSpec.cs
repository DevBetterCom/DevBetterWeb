using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class ArchiveVideoWithoutThumbnailSpec : Specification<ArchiveVideo>
{
  public ArchiveVideoWithoutThumbnailSpec()
  {
    Query
      .Where(s => string.IsNullOrEmpty(s.AnimatedThumbnailUri));
  }
}
