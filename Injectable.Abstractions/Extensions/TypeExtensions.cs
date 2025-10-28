using System;

namespace Injectable;

public static class TypeExtensions
{
    public static bool IsOfType<T>(this Type type)
    {
        return type == typeof(T);
    }
}
