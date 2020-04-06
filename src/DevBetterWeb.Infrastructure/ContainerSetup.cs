using Autofac;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DomainEvents;
using System;
using System.Reflection;

namespace DevBetterWeb.Infrastructure
{
    public static class ContainerSetup
    {
        public static void InitializeAutofac(this ContainerBuilder builder, Assembly webAssembly)
        {
            var coreAssembly = Assembly.GetAssembly(typeof(Question)) ?? throw new Exception("Assembly not found.");
            var infrastructureAssembly = Assembly.GetAssembly(typeof(EfRepository)) ?? throw new Exception("Assembly not found.");
            //var sharedKernelAssembly = Assembly.GetAssembly(typeof(IRepository));

            builder.RegisterAssemblyTypes(webAssembly, coreAssembly, infrastructureAssembly).AsImplementedInterfaces();

            // register specific types
            builder.RegisterType<DomainEventDispatcher>().InstancePerLifetimeScope();
        }
    }
}
