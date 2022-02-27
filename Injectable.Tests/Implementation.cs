using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injectable.Tests
{
    
    [Inject(InjectionType.Implementation)]
    internal class Implementation { }
    internal class ImplementationImplementation : Implementation { }
}
