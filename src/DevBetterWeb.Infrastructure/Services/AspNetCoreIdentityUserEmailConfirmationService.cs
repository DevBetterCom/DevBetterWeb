using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Infrastructure.Services;

public class AspNetCoreIdentityUserEmailConfirmationService : IUserEmailConfirmationService
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IDomainEventDispatcher _dispatcher;

  public AspNetCoreIdentityUserEmailConfirmationService(UserManager<ApplicationUser> userManager,
      IDomainEventDispatcher dispatcher)
  {
    _userManager = userManager;
    _dispatcher = dispatcher;
  }

  public async Task UpdateUserEmailConfirmationAsync(string userId, bool isEmailConfirmed)
  {
    var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
    if (user == null) throw new UserNotFoundException(userId);

    user.EmailConfirmed = isEmailConfirmed;

    await _userManager.UpdateAsync(user);

    var userEmailConfirmedChangedEvent = new UserEmailConfirmedChangedEvent(user.Email, isEmailConfirmed);
    await _dispatcher.Dispatch(userEmailConfirmedChangedEvent);
  }
}
