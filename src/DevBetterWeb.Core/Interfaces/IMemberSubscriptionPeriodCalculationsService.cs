using System;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMemberSubscriptionPeriodCalculationsService
  {
    bool GetHasCurrentSubscription(Member member);
    MemberSubscription GetCurrentSubscription(Member member);
    DateTime GetCurrentSubscriptionEndDate(Member member);
    DateTime GetGraduationDate(Member member);
  }
}
