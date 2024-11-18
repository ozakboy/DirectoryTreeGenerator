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
        // 輸出文件名稱和路徑設定
        public string OutputFileName { get; set; } = "DirectoryStructure.md";
        public string OutputPath { get; set; } = string.Empty;

        // 忽略規則設定
        public string[] IgnorePatterns { get; set; } = Array.Empty<string>();    // 忽略的模式
        public string[] IgnoreDirectories { get; set; } = Array.Empty<string>(); // 忽略的目錄
        public string[] IgnoreFiles { get; set; } = Array.Empty<string>();       // 忽略的文件
        public string[] IgnoreExtensions { get; set; } = Array.Empty<string>();  // 忽略的副檔名

        // 顯示樣式設定
        public bool IncludeFileSize { get; set; } = false;        // 是否包含文件大小
        public bool IncludeLastModified { get; set; } = false;    // 是否包含最後修改時間
        public string DirectoryPrefix { get; set; } = "📁";       // 目錄前綴符號
        public string FilePrefix { get; set; } = "📄";           // 文件前綴符號
        public int IndentSpaces { get; set; } = 2;               // 縮排空格數


        // 額外功能設定
        public bool IncludeHeader { get; set; } = true;          // 是否包含標題
        public string HeaderText { get; set; } = "# Project Directory Structure";  // 標題文字
        public bool IncludeStatistics { get; set; } = false;     // 是否包含統計資訊
        public bool SortDirectoriesFirst { get; set; } = true;   // 是否目錄優先排序


        /// <summary>
        /// 建構函數，設置默認的忽略模式
        /// </summary>
        public GeneratorConfig()
        {
            IgnorePatterns = DirectoryTreeUtils.GetDefaultIgnorePatterns();
        }
    }
}