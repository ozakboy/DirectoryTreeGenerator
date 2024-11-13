// 為不同的.NET框架版本提供相容性擴展方法
#if NETSTANDARD2_0 || NET462
using System.Collections.Generic;
using System.Linq;

namespace ozakboy.DirectoryTreeGenerator
{
    /// <summary>
    /// 框架相容性擴展方法
    /// 為舊版本的.NET框架提供新版本的功能
    /// </summary>
    internal static class FrameworkExtensions
    {
        /// <summary>
        /// 將IEnumerable<string>轉換為陣列的擴展方法
        /// </summary>
        public static string[] ToArray(this IEnumerable<string> source)
        {
            return source?.ToArray() ?? Array.Empty<string>();
        }

        /// <summary>
        /// 獲取相對路徑的擴展方法
        /// 模擬新版本.NET中的Path.GetRelativePath功能
        /// </summary>
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