using Autofac;
using Autofac.Extensions.DependencyInjection;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DevBetterWeb.Infrastructure
{
    public static class ContainerSetup
    {
        public static IServiceProvider InitializeWeb(Assembly webAssembly, IServiceCollection services) =>
            new AutofacServiceProvider(BaseAutofacInitialization(setupAction =>
            {
                setupAction.Populate(services);
                setupAction.RegisterAssemblyTypes(webAssembly).AsImplementedInterfaces();
            }));

        public static IContainer BaseAutofacInitialization(Action<ContainerBuilder> setupAction = null)
        {
            var builder = new ContainerBuilder();

            var coreAssembly = Assembly.GetAssembly(typeof(Question));
            var infrastructureAssembly = Assembly.GetAssembly(typeof(EfRepository));
            //var sharedKernelAssembly = Assembly.GetAssembly(typeof(IRepository));
            builder.RegisterAssemblyTypes(coreAssembly, infrastructureAssembly).AsImplementedInterfaces();

            setupAction?.Invoke(builder);
            return builder.Build();
        }

        public static void InitializeAutofac(this ContainerBuilder builder, Assembly webAssembly)
        {
            var coreAssembly = Assembly.GetAssembly(typeof(Question));
            var infrastructureAssembly = Assembly.GetAssembly(typeof(EfRepository));
            //var sharedKernelAssembly = Assembly.GetAssembly(typeof(IRepository));
            builder.RegisterAssemblyTypes(webAssembly, coreAssembly, infrastructureAssembly).AsImplementedInterfaces();

            //setupAction?.Invoke(builder);
            builder.Build();

        }
    }
}
