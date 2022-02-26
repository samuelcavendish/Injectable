using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injectable.Tests
{
    [Inject(InjectionType.Concrete)]
    internal class ConcreteConcreteInheritance { }
    internal class ConcreteConcreteInheritance1 : ConcreteConcreteInheritance { }
    internal class ConcreteConcreteInheritance2 : ConcreteConcreteInheritance1 { }
    internal class ConcreteConcreteInheritance3 : ConcreteConcreteInheritance2 { }
    internal class ConcreteConcreteInheritance4 : ConcreteConcreteInheritance3 { }
    internal class ConcreteConcreteInheritanceConcrete : ConcreteConcreteInheritance4 { }
}
