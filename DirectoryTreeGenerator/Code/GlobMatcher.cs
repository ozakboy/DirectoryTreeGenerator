using System.Text.RegularExpressions;

namespace ozakboy.DirectoryTreeGenerator.Code
{
    /// <summary>
    /// Glob模式匹配器
    /// 用於檢查文件路徑是否符合指定的glob模式
    /// </summary>
    public static class GlobMatcher
    {
        /// <summary>
        /// 檢查路徑是否匹配指定的glob模式
        /// </summary>
        public static bool IsMatch(string path, string pattern)
        {
            string regex = GlobToRegex(pattern);
            return Regex.IsMatch(path, regex, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 將glob模式轉換為正則表達式
        /// </summary>
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
