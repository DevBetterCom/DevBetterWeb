using System.Collections.Generic;
using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs;

public sealed class MembersNonSubscriptionSpec : Specification<Member>
{
  public MembersNonSubscriptionSpec(List<string> usersIdWithoutMemberRole)
  {
    Query
      .Where(member => usersIdWithoutMemberRole.Contains(member.UserId));
  }
}

