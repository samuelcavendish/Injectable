using Injectable.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletons(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes.Where(x => x.Lifetime == ServiceLifetime.Singleton))
            services.AddSingleton(type.Service, type.Implementation);
        return services;
    }

    public static IServiceCollection AddTransients(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes.Where(x => x.Lifetime == ServiceLifetime.Transient))
            services.AddTransient(type.Service, type.Implementation);
        return services;
    }

    public static IServiceCollection AddScopes(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes.Where(x => x.Lifetime == ServiceLifetime.Scoped))
            services.AddScoped(type.Service, type.Implementation);
        return services;
    }
}
