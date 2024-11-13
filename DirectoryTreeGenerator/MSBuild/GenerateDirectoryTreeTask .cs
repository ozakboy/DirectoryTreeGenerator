using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace ozakboy.DirectoryTreeGenerator.MSBuild
{
    /// <summary>
    /// MSBuild 任務類，用於生成目錄樹結構
    /// 當專案進行構建時會自動執行此任務
    /// </summary>
    public class GenerateDirectoryTreeTask : Microsoft.Build.Utilities.Task
    {
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

        /// <summary>
        /// 執行目錄樹生成任務的主要方法
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            try
            {
                // 驗證輸入參數
                ValidateInputs();
                // 載入配置文件
                var config = LoadConfiguration();
                // 創建目錄樹生成器實例並執行生成
                var generator = new DirectoryTreeGenerator(config);
                generator.GenerateTree(ProjectDir, ProjectDir);
                // 記錄成功訊息
                Log.LogMessage(MessageImportance.High,
                    $"Successfully generated directory tree at {Path.Combine(ProjectDir, config.OutputFileName)}");

                return true;
            }
            catch (Exception ex)
            {
                Log.LogError($"Failed to generate directory tree: {ex.Message}");
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
        /// 如果配置文件不存在或解析失敗，則使用默認配置
        /// </summary>
        private GeneratorConfig LoadConfiguration()
        {
            if (!File.Exists(ConfigPath))
            {
                Log.LogMessage(MessageImportance.Normal,
                    "找不到配置文件，使用默認設定");
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
                return config ?? new GeneratorConfig();
            }
            catch (JsonException ex)
            {
                Log.LogWarning($"配置文件解析失敗：{ex.Message}，使用默認設定");
                return new GeneratorConfig();
            }
        }
    }
}