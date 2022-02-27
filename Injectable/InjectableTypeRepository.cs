using System.Reflection;

namespace Injectable;

public class InjectableTypeRepository
{
    private class InjectableTypeInternal
    {
        public Inject? Attribute { get; set; }
        public Type Implementation { get; set; } = null!;
        public Type Service { get; set; } = null!;
    }

    public static IEnumerable<InjectableType> GetAssemblyInjectables(Assembly assembly)
    {
        IEnumerable<InjectableType> GetAssemblyInjectablesInternal()
        {
            var injectableImplementations = assembly.GetTypes()
                .Where(x => x is { IsAbstract: false, IsClass: true, IsInterface: false });

            IEnumerable<InjectableTypeInternal> recursiveBaseTypes(Type implementation, Type type)
            {
                yield return new InjectableTypeInternal { Attribute = type.GetCustomAttribute<Inject>(), Implementation = implementation, Service = type };

                if (type.BaseType is not null)
                {
                    foreach (var baseType in recursiveBaseTypes(implementation, type.BaseType))
                        yield return baseType;
                }
            }

            IEnumerable<InjectableTypeInternal> recursiveInterfaces(Type implementation, Type type)
            {
                yield return new InjectableTypeInternal { Attribute = type.GetCustomAttribute<Inject>(), Implementation = implementation, Service = type };
                foreach (var interfaceType in type.GetInterfaces())
                {
                    foreach (var interfaceInjectableType in recursiveInterfaces(implementation, interfaceType))
                        yield return interfaceInjectableType;
                }
            }

            var implementationBaseClasses = injectableImplementations.SelectMany(x => recursiveBaseTypes(x, x));
            var interfaceClasses = implementationBaseClasses.SelectMany(x => recursiveInterfaces(x.Implementation, x.Service));
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

        //var duplicatesForDecoratedAndImplementation

        // Return in reverse so the "lowest" class ancestor wins
        return GetAssemblyInjectablesInternal().Reverse();
    }
}
