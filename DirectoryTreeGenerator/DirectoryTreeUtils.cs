
namespace ozakboy.DirectoryTreeGenerator
{
    public static class DirectoryTreeUtils
    {
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

#if NET6_0_OR_GREATER || NETSTANDARD2_1
            return Path.GetRelativePath(Directory.GetCurrentDirectory(), path);
#else
            return FrameworkExtensions.GetRelativePath(
                Path.GetFullPath(Directory.GetCurrentDirectory()),
                Path.GetFullPath(path));
#endif
        }

        public static string[] GetDefaultIgnorePatterns()
        {
            return new[]
            {
                "**/bin/**",
                "**/obj/**",
                "**/.vs/**",
                "**/.git/**",
                "**/node_modules/**",
                "**/*.user",
                "**/*.suo"
            };
        }
    }
}