using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ozakboy.DirectoryTreeGenerator.Configurations;

namespace ozakboy.DirectoryTreeGenerator
{
    /// <summary>
    /// 目錄樹生成器
    /// 用於生成專案目錄結構的 Markdown 文檔
    /// </summary>
    public class DirectoryTreeGenerator
    {
        /// <summary>
        /// 生成器配置
        /// </summary>
        private readonly GeneratorConfig _config;

        /// <summary>
        /// 用於構建目錄樹字符串的建構器
        /// </summary>
        private readonly StringBuilder _treeBuilder;

        /// <summary>
        /// 當前處理的目錄層級（用於控制縮進）
        /// </summary>
        private int _indentLevel;

        /// <summary>
        /// 初始化目錄樹生成器
        /// </summary>
        /// <param name="config">生成器配置對象</param>
        /// <exception cref="ArgumentNullException">當配置為null時拋出</exception>
        public DirectoryTreeGenerator(GeneratorConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _treeBuilder = new StringBuilder();
            _indentLevel = 0;
        }

        /// <summary>
        /// 生成目錄樹結構文檔
        /// </summary>
        /// <param name="rootPath">要掃描的根目錄路徑</param>
        /// <param name="outputPath">輸出文件的目錄路徑</param>
        /// <exception cref="DirectoryNotFoundException">當根目錄不存在時拋出</exception>
        public void GenerateTree(string rootPath, string outputPath)
        {
            // 驗證目錄是否存在
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"找不到目錄：{rootPath}");

            // 清空之前的內容
            _treeBuilder.Clear();

            // 添加標題
            _treeBuilder.AppendLine("# Project Directory Structure");
            _treeBuilder.AppendLine();

            // 處理目錄內容
            ProcessDirectory(new DirectoryInfo(rootPath));

            // 寫入文件
            File.WriteAllText(Path.Combine(outputPath, _config.OutputFileName), _treeBuilder.ToString());
        }

        /// <summary>
        /// 處理目錄內容
        /// </summary>
        /// <param name="directory">要處理的目錄信息</param>
        private void ProcessDirectory(DirectoryInfo directory)
        {
            // 檢查是否需要忽略該目錄
            if (ShouldIgnoreDirectory(directory.Name))
                return;

            // 添加目錄名稱到輸出
            AppendLine($"📁 {directory.Name}/");

            // 增加縮進層級
            _indentLevel++;

            // 先處理文件
            foreach (var file in directory.GetFiles().OrderBy(f => f.Name))
            {
                if (!ShouldIgnoreFile(file.Name))
                {
                    AppendLine($"📄 {file.Name}");
                }
            }

            // 再處理子目錄
            foreach (var dir in directory.GetDirectories().OrderBy(d => d.Name))
            {
                ProcessDirectory(dir);
            }

            // 減少縮進層級
            _indentLevel--;
        }

        /// <summary>
        /// 檢查是否應該忽略指定目錄
        /// </summary>
        /// <param name="directoryName">目錄名稱</param>
        /// <returns>是否應該忽略</returns>
        private bool ShouldIgnoreDirectory(string directoryName)
        {
            return _config.IgnoreDirectories.Contains(directoryName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 檢查是否應該忽略指定文件
        /// </summary>
        /// <param name="fileName">文件名稱</param>
        /// <returns>是否應該忽略</returns>
        private bool ShouldIgnoreFile(string fileName)
        {
            // 檢查文件名是否在忽略列表中
            if (_config.IgnoreFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                return true;

            // 檢查副檔名是否在忽略列表中
            string extension = Path.GetExtension(fileName);
            return _config.IgnoreExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 添加一行到輸出，並處理縮進
        /// </summary>
        /// <param name="content">要添加的內容</param>
        private void AppendLine(string content)
        {
            _treeBuilder.AppendLine($"{new string(' ', _indentLevel * 2)}{content}");
        }
    }

    /// <summary>
    /// 目錄樹統計資訊類
    /// 用於收集和生成目錄結構的統計資訊
    /// </summary>
    public class TreeStatistics
    {
        // 統計數據
        public int TotalDirectories { get; private set; }         // 總目錄數
        public int TotalFiles { get; private set; }              // 總文件數
        public long TotalSize { get; private set; }              // 總大小
        public Dictionary<string, int> ExtensionCounts { get; private set; }  // 副檔名統計

        /// <summary>
        /// 建構函數，初始化統計資料
        /// </summary>
        public TreeStatistics()
        {
            ExtensionCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 添加文件到統計資訊中
        /// </summary>
        public void AddFile(FileInfo file)
        {
            TotalFiles++;
            TotalSize += file.Length;

            string ext = file.Extension.ToLowerInvariant();
            if (!string.IsNullOrEmpty(ext))
            {
                if (!ExtensionCounts.ContainsKey(ext))
                    ExtensionCounts[ext] = 0;
                ExtensionCounts[ext]++;
            }
        }

        /// <summary>
        /// 添加目錄到統計資訊中
        /// </summary>
        public void AddDirectory()
        {
            TotalDirectories++;
        }

        /// <summary>
        /// 生成統計報告
        /// </summary>
        /// <returns>格式化的統計報告字符串</returns>
        public string GenerateReport()
        {
            var report = new StringBuilder();
            report.AppendLine("\n## 目錄統計資訊");
            report.AppendLine($"- 總目錄數：{TotalDirectories:N0}");
            report.AppendLine($"- 總文件數：{TotalFiles:N0}");
            report.AppendLine($"- 總大小：{FormatFileSize(TotalSize)}");

            if (ExtensionCounts.Any())
            {
                report.AppendLine("\n### 文件類型統計");
                foreach (var ext in ExtensionCounts.OrderByDescending(x => x.Value))
                {
                    report.AppendLine($"- {ext.Key}：{ext.Value:N0} 個文件");
                }
            }

            return report.ToString();
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        /// <param name="bytes">文件大小（位元組）</param>
        /// <returns>格式化後的文件大小字符串</returns>
        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }
}