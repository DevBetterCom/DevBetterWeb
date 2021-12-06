using System;

namespace DevBetterWeb.Core.Exceptions;

public class RoleNotFoundException : Exception
{
  public RoleNotFoundException(string roleId) : base($"{roleId} not found")
  {
  }
}
