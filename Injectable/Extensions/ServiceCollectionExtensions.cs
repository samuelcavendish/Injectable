using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Injectable.Extensions;

public class InjectableType
{
    public Inject? Attribute { get; set; }
    public Type Implementation { get; set; } = null!;
    public Type Service { get; set; } = null!;
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInjectableServices(this IServiceCollection services, Assembly assembly)
    {
        var allTypes = assembly.GetTypes()
            .Where(x => x is { IsAbstract: false, IsClass: true, IsInterface: false });

        var injectableInstances = allTypes.SelectMany(x => GetInjectablesFromType(x));

        foreach (var instance in injectableInstances.Reverse().DistinctBy(x => x.Service))
        {
            Action<Type, Type> add = instance.Attribute.InjectionScope switch
            {
                InjectionScope.Singleton =>
                    (serviceType, implementationType) => services.AddSingleton(serviceType, implementationType),
                InjectionScope.Transient =>
                    (serviceType, implementationType) => services.AddTransient(serviceType, implementationType),
                InjectionScope.Scoped =>
                    (serviceType, implementationType) => services.AddScoped(serviceType, implementationType),
                _ => throw new NotImplementedException()
            };
            if (instance.Attribute.InjectionType.HasFlag(InjectionType.Inject))
                add(instance.Service, instance.Implementation);
            if (instance.Attribute.InjectionType.HasFlag(InjectionType.Concrete))
                add(instance.Implementation, instance.Implementation);
        }

        return services;
    }

    private static IEnumerable<InjectableType> GetInjectablesFromType(Type implementation)
    {
        IEnumerable<InjectableType> allBaseClasses(Type type)
        {
            yield return new InjectableType { Attribute = type.GetCustomAttribute<Inject>(), Implementation = implementation, Service = type };

            if (type.BaseType is not null)
            {
                foreach (var baseType in allBaseClasses(type.BaseType))
                    yield return baseType;
            }
        }
        var baseClasses = allBaseClasses(implementation);

        IEnumerable<InjectableType> allInterfacesOnBaseClasses(Type type)
        {
            yield return new InjectableType { Attribute = type.GetCustomAttribute<Inject>(), Implementation = implementation, Service = type };
            foreach (var interfaceType in type.GetInterfaces())
            {
                foreach (var interfaceInjectableType in allInterfacesOnBaseClasses(interfaceType))
                    yield return interfaceInjectableType;
            }
        }

        var interfaceClasses = baseClasses.SelectMany(x => allInterfacesOnBaseClasses(x.Service));
        var injectables = baseClasses.Concat(interfaceClasses).Where(x => x.Attribute is not null);

        return injectables;
    }
}
