using Injectable;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletons(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes)
            services.AddSingleton(type.Service, type.Implementation);
        return services;
    }

    public static IServiceCollection AddTransients(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes)
            services.AddTransient(type.Service, type.Implementation);
        return services;
    }

    public static IServiceCollection AddScopes(this IServiceCollection services, IEnumerable<InjectableType> injectableTypes)
    {
        foreach (var type in injectableTypes)
            services.AddScoped(type.Service, type.Implementation);
        return services;
    }
}
