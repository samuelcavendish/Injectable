using System.Reflection;

namespace Injectable;

public class InjectableTypeRepository
{
    private class InjectableTypeInternal
    {
        public Inject? Attribute { get; set; }
        public Type Implementation { get; set; } = null!;
        public Type Service { get; set; } = null!;

        public override string ToString()
        {
            return $"{Service.Name}:{Implementation.Name}:{Attribute?.InjectionType.ToString() ?? "Null"}";
        }
    }

    public static IEnumerable<InjectableType> GetAssemblyInjectables(Assembly assembly)
    {
        IEnumerable<InjectableType> GetAssemblyInjectablesInternal()
        {
            var injectableImplementations = assembly.GetTypes()
                .Where(x => x is { IsAbstract: false, IsClass: true, IsInterface: false }
                       && (x.GetCustomAttribute<Inject>() is null or { IncludeDecoratedType: true })
                );

            var implementationBaseClasses = injectableImplementations.SelectMany(x => typeWithRecursiveBaseTypes(x));
            var interfaceClasses = implementationBaseClasses.SelectMany(x => recursiveInterfaces(x.Implementation));
            foreach (var injectable in implementationBaseClasses.Concat(interfaceClasses))
            {
                if (injectable.Attribute is null)
                    continue;

                yield return new InjectableType
                {
                    Attribute = injectable.Attribute,
                    Implementation = injectable.Implementation,
                    Service = injectable.Service
                };
            }
        }

        // Return in reverse so the "lowest" class ancestor wins
        // Brute force to remove duplicates, can we do this better?
        return GetAssemblyInjectablesInternal().Reverse().DistinctBy(x => new { x.Implementation, x.Service });
    }



    static IEnumerable<InjectableTypeInternal> typeWithRecursiveBaseTypes(Type implementation)
    {
        IEnumerable<InjectableTypeInternal> getBaseTypesRecursive(Type type)
        {
            if (type.BaseType is not null)
            {
                yield return new InjectableTypeInternal { Attribute = type.BaseType.GetCustomAttribute<Inject>(), Implementation = implementation, Service = type.BaseType };
                foreach (var baseType in getBaseTypesRecursive(type.BaseType))
                {
                    yield return baseType;
                }
            }
        }

        yield return new InjectableTypeInternal { Attribute = implementation.GetCustomAttribute<Inject>(), Implementation = implementation, Service = implementation };

        foreach (var baseType in getBaseTypesRecursive(implementation))
        {
            yield return baseType;
        }
    }


    static IEnumerable<InjectableTypeInternal> recursiveInterfaces(Type implementation)
    {
        IEnumerable<InjectableTypeInternal> getInterfacesRecursive(Type type)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                yield return new InjectableTypeInternal { Attribute = interfaceType.GetCustomAttribute<Inject>(), Implementation = implementation, Service = interfaceType };
                foreach (var parentInterface in getInterfacesRecursive(interfaceType))
                {
                    yield return parentInterface;
                }
            }
        }

        if (implementation.IsInterface)
            yield return new InjectableTypeInternal { Attribute = implementation.GetCustomAttribute<Inject>(), Implementation = implementation, Service = implementation };

        foreach (var parentInterface in getInterfacesRecursive(implementation))
        {
            yield return parentInterface;
        }
    }
}
