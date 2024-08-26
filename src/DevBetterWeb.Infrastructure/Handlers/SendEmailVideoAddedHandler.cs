using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.Interfaces;

namespace DevBetterWeb.Core.Handlers;

public class SendEmailVideoAddedHandler : IHandle<VideoAddedEvent>
{
	private readonly IGetUsersHaveRolesService _getUsersHaveRolesService;
	private readonly IEmailService _emailService;

	public SendEmailVideoAddedHandler(IGetUsersHaveRolesService getUsersHaveRolesService, IEmailService emailService)
	{
		_getUsersHaveRolesService = getUsersHaveRolesService;
		_emailService = emailService;
	}

  public static string ReturnMessageString(VideoAddedEvent domainEvent)
  {
    return $"Video {domainEvent.Video.Title} is added! " +
        $"Check out the video here: https://devbetter.com/Videos/Details/{domainEvent.Video.VideoId}.";
  }

  public async Task Handle(VideoAddedEvent domainEvent)
  {
		var message = ReturnMessageString(domainEvent);
    var activeUsers = await _getUsersHaveRolesService.ExecuteAsync();

    foreach (var activeUser in activeUsers)
    {
			await _emailService.SendEmailAsync(activeUser.UserName!, "DevBetter New Video", message);
		}
  }
}
