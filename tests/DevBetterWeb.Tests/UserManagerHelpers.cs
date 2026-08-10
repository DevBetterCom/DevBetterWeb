using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace DevBetterWeb.Tests;

public static class UserManagerHelpers
{
  public static UserManager<ApplicationUser> CreateSubstitute()
  {
    var store = Substitute.For<IUserStore<ApplicationUser>>();
    return Substitute.For<UserManager<ApplicationUser>>(store, null!, null!, null!, null!, null!, null!, null!, null!);
  }
}
