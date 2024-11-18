using System;
using System.Collections.Generic;
using ozakboy.DirectoryTreeGenerator.Code;

namespace ozakboy.DirectoryTreeGenerator.Configurations
{
    /// <summary>
    /// 目錄樹生成器的配置類
    /// 定義了生成目錄樹時的各種選項和設定
    /// </summary>
    public class GeneratorConfig
    {
        /// <summary>
        /// 輸出的 Markdown 文件名稱
        /// 預設值為 "DirectoryStructure.md"
        /// </summary>
        public string OutputFileName { get; set; } = "DirectoryStructure.md";

        /// <summary>
        /// 輸出文件的目錄路徑
        /// 若為空則使用當前目錄
        /// </summary>
        public string OutputPath { get; set; } = string.Empty;

        /// <summary>
        /// 要忽略的檔案或目錄的 Glob 模式陣列
        /// 例如: "**/bin/**", "**/obj/**", "**/.vs/**" 等
        /// </summary>
        public string[] IgnorePatterns { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 要忽略的目錄名稱陣列
        /// 不區分大小寫，例如: "node_modules", ".git" 等
        /// </summary>
        public string[] IgnoreDirectories { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 要忽略的檔案名稱陣列
        /// 不區分大小寫，例如: "package-lock.json", ".DS_Store" 等
        /// </summary>
        public string[] IgnoreFiles { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 要忽略的副檔名陣列
        /// 需包含點號，不區分大小寫，例如: ".tmp", ".cache" 等
        /// </summary>
        public string[] IgnoreExtensions { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 檔案副檔名對應的圖示字典
        /// Key 為副檔名（需包含點號），Value 為對應的圖示
        /// </summary>
        public Dictionary<string, string> FileExtensionIcons { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { ".cs", "📝" },
            { ".json", "📋" },
            { ".md", "📄" },
            { ".txt", "📃" },
            { ".xml", "📰" },
            { ".png", "🖼️" },
            { ".jpg", "🖼️" },
            { ".gif", "🖼️" },
            { ".pdf", "📚" },
            { ".zip", "📦" },
            { ".exe", "⚙️" },
            { ".dll", "🔧" }
        };

        /// <summary>
        /// 是否包含檔案大小資訊
        /// 若為 true，將在檔案名稱後顯示檔案大小
        /// </summary>
        public bool IncludeFileSize { get; set; } = false;

        /// <summary>
        /// 是否包含最後修改時間
        /// 若為 true，將在檔案名稱後顯示最後修改時間
        /// </summary>
        public bool IncludeLastModified { get; set; } = false;

        /// <summary>
        /// 目錄的預設圖示
        /// 可自訂更改，預設為資料夾圖示
        /// </summary>
        public string DirectoryPrefix { get; set; } = "📁";

        /// <summary>
        /// 未指定副檔名圖示的檔案的預設圖示
        /// 可自訂更改，預設為一般檔案圖示
        /// </summary>
        public string DefaultFilePrefix { get; set; } = "📄";

        /// <summary>
        /// 縮排空格數
        /// 控制目錄樹的階層視覺效果
        /// </summary>
        public int IndentSpaces { get; set; } = 2;

        /// <summary>
        /// 是否包含標題
        /// 若為 true，將在文件開頭加入標題
        /// </summary>
        public bool IncludeHeader { get; set; } = true;

        /// <summary>
        /// 標題文字
        /// 可自訂更改，預設為 "Project Directory Structure"
        /// </summary>
        public string HeaderText { get; set; } = "# Project Directory Structure";

        /// <summary>
        /// 是否包含統計資訊
        /// 若為 true，將在文件末尾加入目錄和檔案的統計資訊
        /// </summary>
        public bool IncludeStatistics { get; set; } = false;

        /// <summary>
        /// 是否目錄優先排序
        /// 若為 true，將先列出所有目錄，再列出檔案
        /// </summary>
        public bool SortDirectoriesFirst { get; set; } = true;

        /// <summary>
        /// 建構函數
        /// 初始化配置物件並設置預設的忽略模式
        /// </summary>
        public GeneratorConfig()
        {
            IgnorePatterns = DirectoryTreeUtils.GetDefaultIgnorePatterns();
        }

        /// <summary>
        /// 取得指定副檔名的圖示
        /// </summary>
        /// <param name="extension">檔案副檔名（需包含點號）</param>
        /// <returns>對應的圖示，若未定義則返回預設圖示</returns>
        public string GetFileIcon(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return DefaultFilePrefix;

            return FileExtensionIcons.TryGetValue(extension.ToLowerInvariant(), out var icon)
                ? icon
                : DefaultFilePrefix;
        }
    }
}