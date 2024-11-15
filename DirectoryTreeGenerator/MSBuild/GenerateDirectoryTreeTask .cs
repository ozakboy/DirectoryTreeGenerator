using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace ozakboy.DirectoryTreeGenerator.MSBuild
{
    /// <summary>
    /// MSBuild 任務類，用於生成目錄樹結構
    /// 當專案進行構建時會自動執行此任務
    /// </summary>
    public class GenerateDirectoryTreeTask : Task, IDisposable
    {
        private bool _disposed = false;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly int _timeoutMilliseconds = 30000; // 30 秒超時

        /// <summary>
        /// 必需的專案目錄路徑
        /// </summary>
        [Required]
        public string ProjectDir { get; set; } = string.Empty;

        /// <summary>
        /// 必需的配置文件路徑
        /// </summary>
        [Required]
        public string ConfigPath { get; set; } = string.Empty;

        public GenerateDirectoryTreeTask()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 執行目錄樹生成任務的主要方法
        /// </summary>
        public override bool Execute()
        {
            try
            {
                // 設置超時取消
                _cancellationTokenSource.CancelAfter(_timeoutMilliseconds);

                // 直接執行，不使用 Task
                return ExecuteInternal();
            }
            catch (OperationCanceledException)
            {
                Log.LogWarning("⚠️ 任務執行超時或被取消");
                return false;
            }
            catch (Exception ex)
            {
                Log.LogError($"❌ 致命錯誤: {ex.Message}");
                return false;
            }
            finally
            {
                // 確保資源被釋放
                Dispose();
            }
        }

        /// <summary>
        /// 內部執行方法
        /// </summary>
        private bool ExecuteInternal()
        {
            try
            {
                Log.LogMessage(MessageImportance.High, "開始執行 DirectoryTreeGenerator...");

                // 檢查是否已取消
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return false;
                }

                // 驗證輸入參數
                ValidateInputs();

                // 載入配置文件
                Log.LogMessage(MessageImportance.Normal, "正在載入配置檔案...");
                var config = LoadConfiguration();

                // 檢查是否已取消
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return false;
                }

                // 輸出配置資訊
                LogConfiguration(config);

                // 創建目錄樹生成器實例並執行生成
                Log.LogMessage(MessageImportance.Normal, "開始生成目錄樹...");
                var generator = new DirectoryTreeGenerator(config);

                generator.GenerateTree(ProjectDir, ProjectDir);

                // 計算輸出檔案的完整路徑
                string outputFilePath = Path.Combine(ProjectDir, config.OutputFileName);

                // 檢查檔案是否成功生成
                if (File.Exists(outputFilePath))
                {
                    Log.LogMessage(MessageImportance.High, $"✅ 目錄樹生成成功！");
                    Log.LogMessage(MessageImportance.High, $"📄 輸出檔案位置: {outputFilePath}");
                    return true;
                }

                Log.LogWarning("檔案似乎未成功生成，請檢查輸出路徑和權限設定。");
                return false;
            }
            catch (Exception ex)
            {
                Log.LogError($"❌ 生成目錄樹失敗: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Log.LogError($"詳細錯誤: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// 驗證輸入參數的有效性
        /// </summary>
        private void ValidateInputs()
        {
            if (string.IsNullOrEmpty(ProjectDir))
                throw new ArgumentException("專案目錄路徑不能為空");

            if (string.IsNullOrEmpty(ConfigPath))
                throw new ArgumentException("配置文件路徑不能為空");

            if (!Directory.Exists(ProjectDir))
                throw new DirectoryNotFoundException($"找不到專案目錄：{ProjectDir}");
        }

        /// <summary>
        /// 載入配置文件
        /// </summary>
        private GeneratorConfig LoadConfiguration()
        {
            if (!File.Exists(ConfigPath))
            {
                Log.LogMessage(MessageImportance.Normal, "⚠️ 找不到配置文件，將使用默認設定");
                return new GeneratorConfig();
            }

            try
            {
                string jsonContent = File.ReadAllText(ConfigPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var config = JsonSerializer.Deserialize<GeneratorConfig>(jsonContent, options);
                if (config == null)
                {
                    Log.LogWarning("配置檔案解析結果為空，將使用默認設定");
                    return new GeneratorConfig();
                }

                Log.LogMessage(MessageImportance.Normal, "✅ 配置檔案載入成功");
                return config;
            }
            catch (JsonException ex)
            {
                Log.LogWarning($"⚠️ 配置文件解析失敗：{ex.Message}，將使用默認設定");
                return new GeneratorConfig();
            }
        }

        /// <summary>
        /// 輸出當前配置資訊
        /// </summary>
        private void LogConfiguration(GeneratorConfig config)
        {
            Log.LogMessage(MessageImportance.Normal, "目前使用的配置：");
            Log.LogMessage(MessageImportance.Normal, $"- 輸出檔名：{config.OutputFileName}");
            Log.LogMessage(MessageImportance.Normal, $"- 包含檔案大小：{config.IncludeFileSize}");
            Log.LogMessage(MessageImportance.Normal, $"- 包含最後修改時間：{config.IncludeLastModified}");
            Log.LogMessage(MessageImportance.Normal, $"- 包含統計資訊：{config.IncludeStatistics}");
            Log.LogMessage(MessageImportance.Normal, $"- 目錄優先排序：{config.SortDirectoriesFirst}");

            if (config.IgnorePatterns.Length > 0)
            {
                Log.LogMessage(MessageImportance.Normal, "忽略的模式：");
                foreach (var pattern in config.IgnorePatterns)
                {
                    Log.LogMessage(MessageImportance.Normal, $"  - {pattern}");
                }
            }
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 釋放資源的保護方法
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 釋放託管資源
                    if (_cancellationTokenSource != null)
                    {
                        _cancellationTokenSource.Dispose();
                        _cancellationTokenSource = null;
                    }
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// 解構函數
        /// </summary>
        ~GenerateDirectoryTreeTask()
        {
            Dispose(false);
        }
    }
}