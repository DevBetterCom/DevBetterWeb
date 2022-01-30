using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class VideoAddedEvent : BaseDomainEvent
{
  public VideoAddedEvent(ArchiveVideo video)
  {
    Video = video;
  }

  public ArchiveVideo Video { get; }
}
