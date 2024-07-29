using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using MassTransit;
using NimblePros.OutboxServer.UseCases.EmailMessages.Send;

namespace DevBetterWeb.Infrastructure.Services;

public class OutboxSendEmailService : IEmailService
{
	private readonly IBus _bus;

	public OutboxSendEmailService(IBus bus)
  {
	  _bus = bus;
  }

  public async Task SendEmailAsync(string email, string subject, string message)
  {
	  var msg = new SendMessageCommand(new List<string> { email }, "donotreply@devbetter.com", subject, message, "DevBetter");
		await _bus.Publish(msg);
  }
}
