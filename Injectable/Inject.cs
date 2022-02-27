namespace Injectable
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public class Inject : Attribute
    {
        public Inject(InjectionType injectionType = InjectionType.Decorated, bool includeDecoratedType = true)
        {
            InjectionType = injectionType;
            IncludeDecoratedType = includeDecoratedType;
        }

        public InjectionType InjectionType { get; }
        public bool IncludeDecoratedType { get; }
    }
}