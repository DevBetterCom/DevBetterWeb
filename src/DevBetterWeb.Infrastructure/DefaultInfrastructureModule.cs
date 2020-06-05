using Autofac;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Infrastructure.DomainEvents;
using DevBetterWeb.Infrastructure.Services;
using Discord;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace DevBetterWeb.Infrastructure
{
    public class DefaultInfrastructureModule : Module
    {
        private bool _isDevelopment = false;
        public DefaultInfrastructureModule(bool isDevelopment)
        {
            _isDevelopment = isDevelopment;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_isDevelopment)
            {
                RegisterDevelopmentOnlyDependencies(builder);
            }
            else
            {
                RegisterProductionOnlyDependencies(builder);
            }
            RegisterCommonDependencies(builder);
        }

        private void RegisterCommonDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<DomainEventDispatcher>().InstancePerLifetimeScope();
            builder.RegisterType<Webhook>().InstancePerDependency();
            builder.RegisterType<DomainEventDispatcher>().As<IDomainEventDispatcher>();
            builder.RegisterType<MemberRegistrationService>().As<IMemberRegistrationService>();
            builder.RegisterType<DefaultEmailSender>().As<IEmailSender>();
            builder.RegisterType<AspNetCoreIdentityUserRoleMembershipService>()
                .As<IUserRoleMembershipService>();

            builder.RegisterDecorator<LoggerEmailServiceDecorator, IEmailService>();

            builder.RegisterAssemblyTypes(this.ThisAssembly)
                .AsClosedTypesOf(typeof(IHandle<>));
        }

        private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<LocalSmtpEmailService>().As<IEmailService>();
        }

        private void RegisterProductionOnlyDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<SendGridEmailService>().As<IEmailService>();
        }

    }
}
