using Injectable.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Injectable
{
    public static class InjectableListExtensions
    {
        public static IEnumerable<InjectableType> AddInjectables(this IEnumerable<InjectableType> injectableTypes, ServiceCollection services)
        {
            services.AddInjectables(injectableTypes);
            return injectableTypes;
        }

        public static IEnumerable<InjectableType> AddInjectableSingletons(this IEnumerable<InjectableType> injectableTypes, ServiceCollection services)
        {
            services.AddInjectableSingletons(injectableTypes);
            return injectableTypes;
        }

        public static IEnumerable<InjectableType> AddInjectableTransients(this IEnumerable<InjectableType> injectableTypes, ServiceCollection services)
        {
            services.AddInjectableTransients(injectableTypes);
            return injectableTypes;
        }

        public static IEnumerable<InjectableType> AddInjectableScopes(this IEnumerable<InjectableType> injectableTypes, ServiceCollection services)
        {
            services.AddInjectableScopes(injectableTypes);
            return injectableTypes;
        }
    }
}
