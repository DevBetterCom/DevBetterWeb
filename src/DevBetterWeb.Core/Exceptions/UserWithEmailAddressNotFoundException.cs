using System;

namespace DevBetterWeb.Core.Exceptions;

public class UserWithEmailAddressNotFoundException : Exception
{
  public UserWithEmailAddressNotFoundException(string emailAddress) : base($"No user with email {emailAddress} was found")
  {
  }
}
