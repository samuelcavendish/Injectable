using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injectable.Extensions
{
    public static class InjectableTypeExtensions
    {
        public static IEnumerable<InjectableType> OfServiceType<T>(this IEnumerable<InjectableType> injectableTypes) 
            => injectableTypes.Where(x => x.Service.IsOfType<T>());
    }
}
