namespace Injectable
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public class Inject : Attribute
    {
        public Inject(InjectionType injectionType = InjectionType.Inject, InjectionScope injectionScope = InjectionScope.Singleton)
        {
            InjectionType = injectionType;
            InjectionScope = injectionScope;
        }

        public InjectionType InjectionType { get; }
        public InjectionScope InjectionScope { get; }
    }

    [Flags]
    public enum InjectionType
    {
        Inject = 1,
        Concrete = 2,
        FirstGeneric = 4
    }

    public enum InjectionScope
    {
        Singleton = 1,
        Transient = 2,
        Scoped = 3
    }
}