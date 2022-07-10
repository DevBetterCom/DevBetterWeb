using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class VideoCommentByVideoIdSpec : Specification<VideoComment>
{
  public VideoCommentByVideoIdSpec(string videoId)
  {
	  var videoIdToSearchBy = long.Parse(videoId);
	  Query
		  .Where(x => x.VideoId == videoIdToSearchBy)
		  .Include(x => x.Replies)
		  .OrderByDescending(x => x.CreatedAt);
  }
}
