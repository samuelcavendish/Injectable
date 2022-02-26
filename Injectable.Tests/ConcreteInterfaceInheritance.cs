using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injectable.Tests
{
    [Inject(InjectionType.Concrete)]
    internal class ConcreteInterfaceInheritance { }
    internal class ConcreteInterfaceInheritance1 : ConcreteInterfaceInheritance { }
    internal class ConcreteInterfaceInheritance2 : ConcreteInterfaceInheritance1 { }
    internal class ConcreteInterfaceInheritance3 : ConcreteInterfaceInheritance2 { }
    internal class ConcreteInterfaceInheritance4 : ConcreteInterfaceInheritance3 { }
    internal class ConcreteInterfaceInheritanceConcrete : ConcreteInterfaceInheritance4 { }
}
