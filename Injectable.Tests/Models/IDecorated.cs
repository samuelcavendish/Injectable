using Injectable.Abstractions;

namespace Injectable.Tests.Models;

[Inject]
internal interface IDecorated { }
internal class DecoratedImplementation : IDecorated { }
