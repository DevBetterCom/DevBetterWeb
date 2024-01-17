using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public class MemberByUserIdWithFavoriteArchiveVideosSpec : Specification<Member>, 
	ISingleResultSpecification<Member>
{
  public MemberByUserIdWithFavoriteArchiveVideosSpec(string userId)
  {
    Query.Where(member => member.UserId == userId)
      .Include(member => member.FavoriteArchiveVideos);
  }
}
