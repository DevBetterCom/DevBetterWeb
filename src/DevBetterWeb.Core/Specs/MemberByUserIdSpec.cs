using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class MemberByUserIdSpec : Specification<Member>, 
	ISingleResultSpecification<Member>
{
  public MemberByUserIdSpec(string userId)
  {
    Query
	    .Include(member => member.AddressHistory)
	    .Where(member => member.UserId == userId);
  }
}
