using System;
using System.IO;
using System.Linq;
using System.Text;
using ozakboy.DirectoryTreeGenerator.Code;
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
        /// 統計資訊對象
        /// </summary>
        private TreeStatistics _statistics;

        /// <summary>
        /// 當前處理的目錄層級（用於控制縮進）
        /// </summary>
        private int _indentLevel;

        /// <summary>
        /// 根目錄的完整路徑
        /// </summary>
        private string _rootPath;

        /// <summary>
        /// 初始化目錄樹生成器
        /// </summary>
        public DirectoryTreeGenerator() : this((GeneratorConfig)null)
        {
        }


        /// <summary>
        /// 使用指定的配置文件初始化目錄樹生成器
        /// </summary>
        /// <param name="configPath">配置文件路徑</param>
        public DirectoryTreeGenerator(string configPath)
        {
            _config = ConfigurationLoader.LoadConfiguration(configPath);
            _treeBuilder = new StringBuilder();
            _statistics = new TreeStatistics();
            _indentLevel = 0;
        }

        /// <summary>
        /// 使用指定的配置對象初始化目錄樹生成器
        /// </summary>
        /// <param name="config">生成器配置對象，如果為null則載入預設配置</param>
        public DirectoryTreeGenerator(GeneratorConfig config)
        {
            _config = config ?? ConfigurationLoader.LoadConfiguration();
            _treeBuilder = new StringBuilder();
            _statistics = new TreeStatistics();
            _indentLevel = 0;
        }


        /// <summary>
        /// 生成目錄樹結構文檔
        /// </summary>
        /// <param name="rootPath">要掃描的根目錄路徑</param>
        /// <param name="outputPath">輸出文件的目錄路徑</param>
        /// <exception cref="DirectoryNotFoundException">當根目錄不存在時拋出</exception>
        /// <exception cref="ArgumentException">當路徑無效時拋出</exception>
        public void GenerateTree(string rootPath, string outputPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("根目錄路徑不能為空", nameof(rootPath));

            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("輸出路徑不能為空", nameof(outputPath));

            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"找不到目錄：{rootPath}");

            // 重置狀態
            _rootPath = Path.GetFullPath(rootPath);
            _treeBuilder.Clear();
            _statistics = new TreeStatistics();
            _indentLevel = 0;

            try
            {
                // 添加標題
                if (_config.IncludeHeader)
                {
                    _treeBuilder.AppendLine(_config.HeaderText);
                    _treeBuilder.AppendLine();
                }

                // 處理目錄內容
                ProcessDirectory(new DirectoryInfo(_rootPath));

                // 添加統計資訊
                if (_config.IncludeStatistics)
                {
                    _treeBuilder.Append(_statistics.GenerateReport());
                }

                // 確保輸出目錄存在
                Directory.CreateDirectory(outputPath);

                // 寫入文件
                string outputFilePath = Path.Combine(outputPath, _config.OutputFileName);
                File.WriteAllText(outputFilePath, _treeBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"生成目錄樹時發生錯誤：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 處理目錄內容
        /// </summary>
        /// <param name="directory">要處理的目錄信息</param>
        private void ProcessDirectory(DirectoryInfo directory)
        {
            // 檢查是否需要忽略該目錄
            if (ShouldIgnoreDirectory(directory))
                return;

            // 統計目錄
            _statistics.AddDirectory();

            // 取得相對路徑
            string relativePath = DirectoryTreeUtils.NormalizePath(directory.FullName);

            // 添加目錄名稱到輸出
            if (_indentLevel > 0) // 不顯示根目錄
            {
                AppendLine($"{_config.DirectoryPrefix} {directory.Name}/");
            }

            // 增加縮進層級
            _indentLevel++;

            try
            {
                // 獲取所有文件和目錄
                var entries = GetSortedDirectoryEntries(directory);

                // 處理所有條目
                foreach (var entry in entries)
                {
                    if (entry is DirectoryInfo dir)
                    {
                        ProcessDirectory(dir);
                    }
                    else if (entry is FileInfo file)
                    {
                        ProcessFile(file);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                AppendLine("⚠️ 存取被拒絕");
            }
            catch (Exception ex)
            {
                AppendLine($"⚠️ 錯誤：{ex.Message}");
            }

            // 減少縮進層級
            _indentLevel--;
        }


        /// <summary>
        /// 處理檔案
        /// </summary>
        /// <param name="file">要處理的檔案資訊</param>
        private void ProcessFile(FileInfo file)
        {
            if (ShouldIgnoreFile(file))
                return;

            string relativePath = DirectoryTreeUtils.NormalizePath(file.FullName);
            _statistics.AddFile(file, relativePath);

            // 構建檔案描述
            var fileDescription = new StringBuilder();

            // 添加圖示和檔案名
            fileDescription.Append($"{_config.GetFileIcon(file.Extension)} {file.Name}");

            // 添加檔案大小
            if (_config.IncludeFileSize)
            {
                fileDescription.Append($" ({FileUtils.FormatFileSize(file.Length)})");
            }

            // 添加最後修改時間
            if (_config.IncludeLastModified)
            {
                fileDescription.Append($" - {file.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
            }

            AppendLine(fileDescription.ToString());
        }

        /// <summary>
        /// 獲取排序後的目錄條目
        /// </summary>
        /// <param name="directory">目錄資訊</param>
        /// <returns>排序後的檔案系統條目集合</returns>
        private FileSystemInfo[] GetSortedDirectoryEntries(DirectoryInfo directory)
        {
            try
            {
                var entries = directory.GetFileSystemInfos();

                if (_config.SortDirectoriesFirst)
                {
                    return entries.OrderByDescending(e => e is DirectoryInfo)
                                .ThenBy(e => e.Name)
                                .ToArray();
                }

                return entries.OrderBy(e => e.Name).ToArray();
            }
            catch (Exception)
            {
                return Array.Empty<FileSystemInfo>();
            }
        }

        /// <summary>
        /// 檢查是否應該忽略指定目錄
        /// </summary>
        /// <param name="directory">目錄資訊</param>
        /// <returns>是否應該忽略</returns>
        private bool ShouldIgnoreDirectory(DirectoryInfo directory)
        {
            // 檢查目錄名稱
            if (_config.IgnoreDirectories.Contains(directory.Name, StringComparer.OrdinalIgnoreCase))
                return true;

            // 檢查相對路徑是否匹配任何忽略模式
            string relativePath = DirectoryTreeUtils.NormalizePath(directory.FullName);
            return _config.IgnorePatterns.Any(pattern => GlobMatcher.IsMatch(relativePath, pattern));
        }

        /// <summary>
        /// 檢查是否應該忽略指定文件
        /// </summary>
        /// <param name="file">文件資訊</param>
        /// <returns>是否應該忽略</returns>
        private bool ShouldIgnoreFile(FileInfo file)
        {
            // 檢查文件名
            if (_config.IgnoreFiles.Contains(file.Name, StringComparer.OrdinalIgnoreCase))
                return true;

            // 檢查副檔名
            if (_config.IgnoreExtensions.Contains(file.Extension, StringComparer.OrdinalIgnoreCase))
                return true;

            // 檢查相對路徑是否匹配任何忽略模式
            string relativePath = DirectoryTreeUtils.NormalizePath(file.FullName);
            return _config.IgnorePatterns.Any(pattern => GlobMatcher.IsMatch(relativePath, pattern));
        }

        /// <summary>
        /// 添加一行到輸出，並處理縮進
        /// </summary>
        /// <param name="content">要添加的內容</param>
        private void AppendLine(string content)
        {
            _treeBuilder.AppendLine($"{new string(' ', _indentLevel * _config.IndentSpaces)}{content}");
        }

        /// <summary>
        /// 取得統計資訊
        /// </summary>
        /// <returns>目錄樹的統計資訊</returns>
        public TreeStatistics GetStatistics()
        {
            return _statistics;
        }

        /// <summary>
        /// 重置生成器狀態
        /// </summary>
        public void Reset()
        {
            _treeBuilder.Clear();
            _statistics = new TreeStatistics();
            _indentLevel = 0;
            _rootPath = string.Empty;
        }

        /// <summary>
        /// 取得當前的目錄樹內容
        /// </summary>
        /// <returns>目錄樹的字符串表示</returns>
        public override string ToString()
        {
            return _treeBuilder.ToString();
        }
    }
}