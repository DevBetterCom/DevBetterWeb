using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DevBetterWeb.Infrastructure.Services
{
    public class LoggerEmailServiceDecorator : IEmailService
    {
        private readonly IEmailService _actualEmailService;
        private readonly ILogger<LoggerEmailServiceDecorator> _logger;

        public LoggerEmailServiceDecorator(IEmailService actualEmailService,
            ILogger<LoggerEmailServiceDecorator> logger)
        {
            _actualEmailService = actualEmailService;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            _logger.LogInformation("Using email service implementation {0}", _actualEmailService.GetType());
            _logger.LogInformation("Sending email to {0} with subject {1} and message {2}", email, subject, message);
            await _actualEmailService.SendEmailAsync(email, subject, message);
        }
    }
}
