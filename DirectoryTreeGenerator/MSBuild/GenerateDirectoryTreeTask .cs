using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace ozakboy.DirectoryTreeGenerator.MSBuild
{
    public class GenerateDirectoryTreeTask : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string ProjectDir { get; set; } = string.Empty;

        [Required]
        public string ConfigPath { get; set; } = string.Empty;

        public override bool Execute()
        {
            try
            {
                ValidateInputs();
                var config = LoadConfiguration();
                var generator = new DirectoryTreeGenerator(config);
                generator.GenerateTree(ProjectDir, ProjectDir);

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

        private void ValidateInputs()
        {
            if (string.IsNullOrEmpty(ProjectDir))
                throw new ArgumentException("ProjectDir cannot be null or empty");

            if (string.IsNullOrEmpty(ConfigPath))
                throw new ArgumentException("ConfigPath cannot be null or empty");

            if (!Directory.Exists(ProjectDir))
                throw new DirectoryNotFoundException($"Project directory not found: {ProjectDir}");
        }

        private GeneratorConfig LoadConfiguration()
        {
            if (!File.Exists(ConfigPath))
            {
                Log.LogMessage(MessageImportance.Normal,
                    "Configuration file not found. Using default settings.");
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
                Log.LogWarning($"Failed to parse configuration file: {ex.Message}. Using default settings.");
                return new GeneratorConfig();
            }
        }
    }
}