using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injectable.Tests
{
    [Inject]
    internal class InjectConcreteInheritance { }
    internal class InjectConcreteInheritance1 : InjectConcreteInheritance { }
    internal class InjectConcreteInheritance2 : InjectConcreteInheritance1 { }
    internal class InjectConcreteInheritance3 : InjectConcreteInheritance2 { }
    internal class InjectConcreteInheritance4 : InjectConcreteInheritance3 { }
    internal class InjectConcreteInheritanceConcrete : InjectConcreteInheritance4 { }
}
