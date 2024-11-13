using System;
using System.Collections.Generic;

namespace ozakboy.DirectoryTreeGenerator
{
    public class GeneratorConfig
    {
        // 基本配置
        public string OutputFileName { get; set; } = "DirectoryStructure.md";
        public string OutputPath { get; set; } = string.Empty;

        // 忽略規則
        public string[] IgnorePatterns { get; set; } = Array.Empty<string>();
        public string[] IgnoreDirectories { get; set; } = Array.Empty<string>();
        public string[] IgnoreFiles { get; set; } = Array.Empty<string>();
        public string[] IgnoreExtensions { get; set; } = Array.Empty<string>();

        // 樣式配置
        public bool IncludeFileSize { get; set; } = false;
        public bool IncludeLastModified { get; set; } = false;
        public string DirectoryPrefix { get; set; } = "📁";
        public string FilePrefix { get; set; } = "📄";
        public int IndentSpaces { get; set; } = 2;

        // 額外功能配置
        public bool IncludeHeader { get; set; } = true;
        public string HeaderText { get; set; } = "# Project Directory Structure";
        public bool IncludeStatistics { get; set; } = false;
        public bool SortDirectoriesFirst { get; set; } = true;

        public GeneratorConfig()
        {
            // 設置默認的忽略模式
            IgnorePatterns = DirectoryTreeUtils.GetDefaultIgnorePatterns();
        }
    }
}