namespace Injectable.Extensions
{
    public static class TypeExtensions
    {
        public static bool OfType<T>(this Type type)
        {
            return type == typeof(T);
        }
    }
}
