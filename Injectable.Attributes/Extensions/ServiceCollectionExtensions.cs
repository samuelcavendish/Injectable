using Injectable.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Injectable;

public static class ServiceCollectionExtensions
{
    // Add all injectables respecting their lifecycle from the attribute
    public static IServiceCollection AddByLifecycle(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes)
        {
            switch (type.Lifecycle)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(type.Service, type.Implementation);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(type.Service, type.Implementation);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(type.Service, type.Implementation);
                    break;
            }
        }
        return services;
    }

    public static IServiceCollection AddInjectables(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes)
        {
            switch (type.Lifecycle)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient(type.Service, type.Implementation);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(type.Service, type.Implementation);
                    break;
                case ServiceLifetime.Singleton:
                default:
                    services.AddSingleton(type.Service, type.Implementation);
                    break;
            }
        }
        return services;
    }

    public static IServiceCollection AddInjectableSingletons(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes.Where(x => x.Lifecycle == ServiceLifetime.Singleton))
            services.AddSingleton(type.Service, type.Implementation);
        return services;
    }

    public static IServiceCollection AddInjectableTransients(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes.Where(x => x.Lifecycle == ServiceLifetime.Transient))
            services.AddTransient(type.Service, type.Implementation);
        return services;
    }

    public static IServiceCollection AddInjectableScoped(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes.Where(x => x.Lifecycle == ServiceLifetime.Scoped))
            services.AddScoped(type.Service, type.Implementation);
        return services;
    }
}
