#if NETSTANDARD2_0 || NET462
using System.Collections.Generic;
using System.Linq;

namespace ozakboy.DirectoryTreeGenerator
{
    internal static class FrameworkExtensions
    {
        public static string[] ToArray(this IEnumerable<string> source)
        {
            return source?.ToArray() ?? Array.Empty<string>();
        }

        public static string GetRelativePath(string relativeTo, string path)
        {
            var relativeToUri = new Uri(relativeTo);
            var pathUri = new Uri(path);
            var relativeUri = relativeToUri.MakeRelativeUri(pathUri);
            return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
#endif