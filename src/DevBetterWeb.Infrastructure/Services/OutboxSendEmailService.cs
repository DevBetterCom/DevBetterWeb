using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using MassTransit;
using NimblePros.OutboxServer.UseCases.EmailMessages.Send;

namespace DevBetterWeb.Infrastructure.Services;

public class OutboxSendEmailService : IEmailService
{
	private readonly IBus _bus;
	private readonly EmailSettings _emailSettings;

	public OutboxSendEmailService(IBus bus, EmailSettings emailSettings)
	{
		_bus = bus;
		_emailSettings = emailSettings;
	}

  public async Task SendEmailAsync(string email, string subject, string message)
  {
	  var msg = new SendMessageCommand(new List<string> { email }, _emailSettings.DefaultFromEmail, subject, message, _emailSettings.ApplicationName);
		await _bus.Publish(msg);
  }
}
