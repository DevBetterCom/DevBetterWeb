using DevBetterWeb.Web.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevBetterWeb.Infrastructure.Identity.Data
{
    public static class IdentityExtensions
    {
        public static string UserFullName(this ApplicationUser user)
        {
            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
            {
                return "[No Name Provided]";
            }
            else
            {
                return $"{user.FirstName} {user.LastName}";
            }
        }
    }
}
