using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class NotifyNewRoleAddedHandler : IHandle<UserAddedToRoleEvent>
    {
        private readonly IRepository _repository;
        private readonly IEmailService _emailService;

        public NotifyNewRoleAddedHandler(IRepository repository,
            IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public async Task Handle(UserAddedToRoleEvent domainEvent)
        {
            var spec = new MemberByUserIdSpec(domainEvent.UserId);
            var member = await _repository.GetBySpecAsync(spec);

            string message = $"devBetter Member {member.UserFullName()} was added to role {domainEvent.RoleName}.";
            string subject = "[devBetter] Member added to role";
            string recipient = "steve@ardalis.com";

            await _emailService.SendEmailAsync(recipient, subject, message);
        }

        void IHandle<UserAddedToRoleEvent>.Handle(UserAddedToRoleEvent domainEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}
