using System;

namespace DevBetterWeb.Core.Exceptions;

public class MemberNotFoundException : Exception
{
  public MemberNotFoundException(int memberId) : base($"No member found with id {memberId}.")
  {
  }
  public MemberNotFoundException(string userId) : base($"No member found with userId {userId}.")
  {
  }
}
