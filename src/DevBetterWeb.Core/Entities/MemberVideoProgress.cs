using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class MemberVideoProgress : BaseEntity, IAggregateRoot
{
  public int MemberId { get; set; }
  public int ArchiveVideoId { get; set; }
  /// <summary>
  /// The last moment watched by this member in milliseconds
  /// Duration max is stored on ArchiveVideo
  /// </summary>
  public int CurrentDuration { get; private set; } = 0;

  // consider an enum with Not Watched / In Progress / Watched
  //public enum InProgress { get; set; }
  public MemberVideoProgress(int memberId, int archiveVideoId)
  {
    MemberId = memberId;
    ArchiveVideoId = archiveVideoId;
  }
}

public class MemberVideoService
{
  public MemberVideoService()
  {
    
  }

  public void GetMemberProgress(int memberId, int videoId)
  {
    // get the video
    // get the membervideoprogress

    //var result = 
  }
}
