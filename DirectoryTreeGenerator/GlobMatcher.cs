using System.Text.RegularExpressions;

namespace ozakboy.DirectoryTreeGenerator
{
    public static class GlobMatcher
    {
        public static bool IsMatch(string path, string pattern)
        {
            string regex = GlobToRegex(pattern);
            return Regex.IsMatch(path, regex, RegexOptions.IgnoreCase);
        }

        private static string GlobToRegex(string glob)
        {
            // 轉換基本的 glob 模式為 regex
            var regex = Regex.Escape(glob)
                           .Replace(@"\*\*/", "(.*/)?") // **/ 匹配任意深度目錄
                           .Replace(@"\*", "[^/]*")     // * 匹配單層的任意字符
                           .Replace(@"\?", ".");        // ? 匹配單個字符

            return $"^{regex}$";
        }
    }
}
