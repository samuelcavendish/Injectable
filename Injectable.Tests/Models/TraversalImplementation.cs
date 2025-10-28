using Injectable.Abstractions;
namespace Injectable.Tests.Models;

[Inject(InjectionType.DecoratedAndImplementation)]
internal interface ITraversalInterface { }
internal interface ITraversalInterface1 : ITraversalInterface { }
internal interface ITraversalInterface2 : ITraversalInterface1 { }
internal interface ITraversalInterface3 : ITraversalInterface2 { }
internal class TraversalInterfaceImplementation3 : ITraversalInterface3 { }
internal class TraversalInterfaceImplementation : TraversalInterfaceImplementation3 { }
