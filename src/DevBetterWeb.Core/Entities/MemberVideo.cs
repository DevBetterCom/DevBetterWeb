using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities
{
  public class MemberVideo : BaseEntity, IAggregateRoot
  {
    public int MemberId { get; set; }
    public int ArchiveVideoId { get; set; }
    public int SecondWatched { get; set; }
    public bool IsCompleted { get; set; }
    public ArchiveVideo ArchiveVideo { get; set; } = new ArchiveVideo();

    public MemberVideo(int memberId, int archiveVideoId)
    {
      MemberId = memberId;
      ArchiveVideoId = archiveVideoId;      
    }

    public MemberVideo(int memberId, ArchiveVideo archiveVideo, int secondWatched)
    {
      MemberId = memberId;

      ArchiveVideo = archiveVideo;
      ArchiveVideoId = ArchiveVideo.Id;

      SecondWatched = secondWatched;
    }
  }
}
