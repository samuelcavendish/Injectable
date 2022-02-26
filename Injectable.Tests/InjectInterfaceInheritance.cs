using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injectable.Tests
{
    [Inject]
    internal interface IInjectInterfaceInheritance { }
    internal interface IInjectInterfaceInheritance1 : IInjectInterfaceInheritance { }
    internal interface IInjectInterfaceInheritance2 : IInjectInterfaceInheritance1 { }
    internal interface IInjectInterfaceInheritance3 : IInjectInterfaceInheritance2 { }
    internal interface IInjectInterfaceInheritance4 : IInjectInterfaceInheritance3 { }
    internal class InjectInterfaceInheritanceConcrete : IInjectInterfaceInheritance4 { }
}
