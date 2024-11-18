using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ozakboy.DirectoryTreeGenerator.Configurations
{
    /// <summary>
    /// 配置檔案載入器
    /// 用於載入和解析 directorytree.json 配置檔案
    /// </summary>
    public static class ConfigurationLoader
    {
        /// <summary>
        /// 預設配置檔案名稱
        /// </summary>
        public const string DefaultConfigFileName = "directorytree.json";

        /// <summary>
        /// 載入配置檔案，若找不到配置檔案則返回預設配置
        /// </summary>
        /// <param name="configPath">配置檔案路徑，若為 null 則搜尋預設位置</param>
        /// <returns>生成器配置對象</returns>
        public static GeneratorConfig LoadConfiguration(string configPath = null)
        {
            try
            {
                // 如果指定了配置文件路徑且文件存在
                if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
                {
                    return LoadConfigurationFromFile(configPath);
                }

                // 如果未指定路徑，嘗試搜尋預設位置
                if (string.IsNullOrEmpty(configPath))
                {
                    string foundConfigPath = FindConfigFile();
                    if (File.Exists(foundConfigPath))
                    {
                        return LoadConfigurationFromFile(foundConfigPath);
                    }
                }

                // 如果找不到配置文件，返回預設配置
                return new GeneratorConfig();
            }
            catch (Exception)
            {
                // 如果載入過程中發生任何錯誤，返回預設配置
                return new GeneratorConfig();
            }
        }

        /// <summary>
        /// 從文件載入配置
        /// </summary>
        /// <param name="configPath">配置文件路徑</param>
        /// <returns>配置對象</returns>
        private static GeneratorConfig LoadConfigurationFromFile(string configPath)
        {
            try
            {
                string jsonContent = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                };

                var config = JsonSerializer.Deserialize<GeneratorConfig>(jsonContent, options);
                return config ?? new GeneratorConfig();
            }
            catch (Exception)
            {
                // 如果解析失敗，返回預設配置
                return new GeneratorConfig();
            }
        }

        /// <summary>
        /// 搜尋配置文件
        /// </summary>
        /// <returns>配置文件的完整路徑</returns>
        private static string FindConfigFile()
        {
            // 搜尋順序：
            // 1. 當前目錄
            // 2. 專案根目錄
            // 3. 執行檔所在目錄
            var searchPaths = new[]
            {
                Directory.GetCurrentDirectory(),
                GetProjectRootDirectory(),
                AppContext.BaseDirectory
            };

            foreach (var path in searchPaths)
            {
                if (string.IsNullOrEmpty(path))
                    continue;

                var configPath = Path.Combine(path, DefaultConfigFileName);
                if (File.Exists(configPath))
                {
                    return configPath;
                }
            }

            // 如果都找不到，返回當前目錄的預設配置文件路徑
            return Path.Combine(Directory.GetCurrentDirectory(), DefaultConfigFileName);
        }

        /// <summary>
        /// 取得專案根目錄
        /// </summary>
        /// <returns>專案根目錄路徑</returns>
        private static string GetProjectRootDirectory()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null)
            {
                // 檢查是否存在專案文件
                if (directory.GetFiles("*.csproj").Length > 0 ||
                    directory.GetFiles("*.sln").Length > 0)
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            return null;
        }

        /// <summary>
        /// 保存配置到文件
        /// </summary>
        /// <param name="config">配置對象</param>
        /// <param name="configPath">配置文件路徑</param>
        public static void SaveConfiguration(GeneratorConfig config, string configPath)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrEmpty(configPath))
                configPath = Path.Combine(Directory.GetCurrentDirectory(), DefaultConfigFileName);

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    }
                };

                string jsonContent = JsonSerializer.Serialize(config, options);
                File.WriteAllText(configPath, jsonContent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"保存配置文件時發生錯誤: {ex.Message}", ex);
            }
        }
    }
}