﻿using Ardalis.Specification;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Specs
{
  public class MemberByUserIdWithBooksReadAndSubscriptionsSpec : Specification<Member>
  {
    public MemberByUserIdWithBooksReadAndSubscriptionsSpec(string userId)
    {
      Query.Where(member => member.UserId == userId);
      Query.Include(member => member.BooksRead);
      Query.Include(member => member.Subscriptions);
    }
  }
}
