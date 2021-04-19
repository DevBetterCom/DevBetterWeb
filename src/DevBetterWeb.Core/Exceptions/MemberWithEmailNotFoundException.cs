using System;

namespace DevBetterWeb.Core.Exceptions
{
  public class MemberWithEmailNotFoundException : Exception
  {
    public MemberWithEmailNotFoundException(string email) : base($"No member found for {email}.")
    {
    }
  }
}
