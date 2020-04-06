using Autofac;
using Autofac.Extensions.DependencyInjection;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevBetterWeb.Infrastructure
{
    public static class ContainerSetup
    {
        public static void InitializeAutofac(this ContainerBuilder builder, Assembly webAssembly)
        {
            var coreAssembly = Assembly.GetAssembly(typeof(Question)) ?? throw new Exception("Assembly not found.");
            var infrastructureAssembly = Assembly.GetAssembly(typeof(EfRepository)) ?? throw new Exception("Assembly not found.");
            //var sharedKernelAssembly = Assembly.GetAssembly(typeof(IRepository));

            // Add Guard.AgainstNull for core and infra assemblies

            builder.RegisterAssemblyTypes(webAssembly, coreAssembly, infrastructureAssembly).AsImplementedInterfaces();

            // register specific types
            builder.RegisterType<DomainEventDispatcher>().InstancePerLifetimeScope();

            //setupAction?.Invoke(builder);
            //builder.Build();

        }
    }
}
