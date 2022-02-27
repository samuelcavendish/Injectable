namespace Injectable.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsOfType<T>(this Type type)
        {
            return type == typeof(T);
        }
    }
}
