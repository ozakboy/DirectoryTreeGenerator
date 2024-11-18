using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ozakboy.DirectoryTreeGenerator.Code
{
    /// <summary>
    /// 目錄樹統計資訊類
    /// 用於收集和生成目錄結構的統計資訊
    /// </summary>
    public class TreeStatistics
    {
        /// <summary>
        /// 總目錄數
        /// </summary>
        public int TotalDirectories { get; private set; }

        /// <summary>
        /// 總文件數
        /// </summary>
        public int TotalFiles { get; private set; }

        /// <summary>
        /// 總文件大小（位元組）
        /// </summary>
        public long TotalSize { get; private set; }

        /// <summary>
        /// 副檔名統計
        /// Key 為副檔名，Value 為該類型文件的數量
        /// </summary>
        public Dictionary<string, int> ExtensionCounts { get; }

        /// <summary>
        /// 最大文件大小（位元組）
        /// </summary>
        public long LargestFileSize { get; private set; }

        /// <summary>
        /// 最大文件的完整路徑
        /// </summary>
        public string LargestFilePath { get; private set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        public DateTime LastModified { get; private set; }

        /// <summary>
        /// 初始化一個新的統計實例
        /// </summary>
        public TreeStatistics()
        {
            ExtensionCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            LastModified = DateTime.MinValue;
            LargestFilePath = string.Empty;
        }

        /// <summary>
        /// 添加文件到統計資訊中
        /// </summary>
        /// <param name="file">文件資訊</param>
        /// <param name="relativePath">相對路徑</param>
        public void AddFile(FileInfo file, string relativePath)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            TotalFiles++;
            TotalSize += file.Length;

            // 更新最大文件資訊
            if (file.Length > LargestFileSize)
            {
                LargestFileSize = file.Length;
                LargestFilePath = relativePath ?? file.FullName;
            }

            // 更新最後修改時間
            if (file.LastWriteTime > LastModified)
            {
                LastModified = file.LastWriteTime;
            }

            // 更新副檔名統計
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
            report.AppendLine($"- 總大小：{FileUtils.FormatFileSize(TotalSize)}");
            report.AppendLine($"- 最後更新：{LastModified:yyyy-MM-dd HH:mm:ss}");

            if (LargestFileSize > 0)
            {
                report.AppendLine($"- 最大文件：{LargestFilePath} ({FileUtils.FormatFileSize(LargestFileSize)})");
            }

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

    }
}