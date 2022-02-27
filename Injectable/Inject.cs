namespace Injectable
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public class Inject : Attribute
    {
        public Inject(InjectionType injectionType = InjectionType.Decorated)
        {
            InjectionType = injectionType;
        }

        public InjectionType InjectionType { get; }
    }
}