using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace DevBetterWeb.Infrastructure;

public static class DomainEventHandlerRegistrationExtensions
{
	public static IServiceCollection AddDomainEventHandlers(this IServiceCollection services, params Assembly[] assembliesToScan)
	{
		var handlerInterfaceType = typeof(IHandle<>);

		foreach (var assembly in assembliesToScan)
		{
			var concreteTypes = assembly.GetTypes()
				.Where(t => t is { IsAbstract: false, IsInterface: false })
				.ToList();

			foreach (var type in concreteTypes)
			{
				var handlerInterfaces = type.GetInterfaces()
					.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType);

				foreach (var @interface in handlerInterfaces)
				{
					services.AddScoped(@interface, type);
				}
			}
		}

		return services;
	}
}
