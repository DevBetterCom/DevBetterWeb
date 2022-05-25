namespace DevBetterWeb.Core.ValueObjects;

public class MemberFavoriteArchiveVideo
{
	public int MemberId { get; }
	public int ArchiveVideoId { get; }

	public MemberFavoriteArchiveVideo(int memberId, int archiveVideoId)
	{
		MemberId = memberId;
		ArchiveVideoId = archiveVideoId;
	}

	// EF
	public MemberFavoriteArchiveVideo()
	{
	}
}
