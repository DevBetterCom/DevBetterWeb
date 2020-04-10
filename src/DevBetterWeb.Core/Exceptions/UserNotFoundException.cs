using System;

namespace DevBetterWeb.Core.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string userId) : base($"{userId} not found")
        {
        }
    }
}
