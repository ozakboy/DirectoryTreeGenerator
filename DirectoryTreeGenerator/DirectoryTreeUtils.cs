
namespace ozakboy.DirectoryTreeGenerator
{
    /// <summary>
    /// 目錄樹工具類
    /// 提供處理路徑和預設忽略模式的公用方法
    /// </summary>
    public static class DirectoryTreeUtils
    {
        /// <summary>
        /// 標準化路徑
        /// 將絕對路徑轉換為相對路徑
        /// </summary>
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

        /// <summary>
        /// 獲取預設的忽略模式
        /// </summary>
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