using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class MemberVideoProgress : BaseEntity, IAggregateRoot
{
  public int MemberId { get; set; }
  public int ArchiveVideoId { get; set; }
  public Member? Member { get; set; }
  public ArchiveVideo? Video { get; set; }

  /// <summary>
  /// The last moment watched by this member in milliseconds
  /// Duration max is stored on ArchiveVideo
  /// </summary>
  public int CurrentDuration { get; } = 0;

  public VideoWatchedStatus VideoWatchedStatus { get; private set; }
  public MemberVideoProgress(int memberId, int archiveVideoId, int currentDuration)
  {
    MemberId = memberId;
    ArchiveVideoId = archiveVideoId;
    CurrentDuration = currentDuration;
  }

  public void SetToWatched()
  {
	  VideoWatchedStatus = VideoWatchedStatus.Watched;
  }

  public void SetToUnwatched()
  {
	  VideoWatchedStatus = VideoWatchedStatus.Unwatched;
  }

  public void SetToInProgress()
  {
	  VideoWatchedStatus = VideoWatchedStatus.InProgress;
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
