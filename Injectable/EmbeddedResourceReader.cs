using System.IO;
using System.Reflection;

namespace Injectable
{
    public static class EmbeddedResourceReader
    {
        public static string ReadAsString(Assembly assembly, string resourceName)
        {
            using Stream resource = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new(resource);
            return reader.ReadToEnd();
        }
    }
}
