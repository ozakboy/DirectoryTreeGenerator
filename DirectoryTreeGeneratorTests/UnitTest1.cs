using Microsoft.VisualStudio.TestTools.UnitTesting;
using ozakboy.DirectoryTreeGenerator;
using ozakboy.DirectoryTreeGenerator.Configurations;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DirectoryTreeGeneratorTests
{
    [TestClass]
    public class DirectoryTreeGeneratorTests
    {
        private string _testRootPath;
        private string _outputPath;
        private GeneratorConfig _config;
        private DirectoryTreeGenerator _generator;
        private static readonly string _permanentOutputPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "TestResults"
        );

        [TestInitialize]
        public void Setup()
        {
            // 設定測試根目錄
            _testRootPath = Path.Combine(
                Path.GetTempPath(),
                "DirectoryTreeGeneratorTests",
                Guid.NewGuid().ToString()
            );

            _outputPath = Path.Combine(_testRootPath, "output");

            // 確保目錄存在
            Directory.CreateDirectory(_testRootPath);
            Directory.CreateDirectory(_outputPath);
            Directory.CreateDirectory(_permanentOutputPath);

            // 創建基本配置
            _config = new GeneratorConfig
            {
                OutputFileName = "directory-structure.md",
                IncludeFileSize = true,
                IncludeLastModified = true,
                IncludeStatistics = true,
                SortDirectoriesFirst = true,
                HeaderText = "# Project Directory Structure\n\nGenerated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                DirectoryPrefix = "📁",
                DefaultFilePrefix = "📄"
            };

            _generator = new DirectoryTreeGenerator(_config);

            // 建立測試目錄結構
            TestDirectoryStructures.CreateAllTestStructures(_testRootPath);
        }

        [TestMethod]
        public void GenerateTree_WebApplication_CreatesExpectedStructure()
        {
            // Arrange
            var webPath = Path.Combine(_testRootPath, "WebApplication");
            var webOutputPath = Path.Combine(_permanentOutputPath, "WebApplication");
            Directory.CreateDirectory(webOutputPath);

            // Act
            _generator.GenerateTree(webPath, webOutputPath);

            // Assert
            string outputFilePath = Path.Combine(webOutputPath, _config.OutputFileName);
            Assert.IsTrue(File.Exists(outputFilePath), "Output file should be created");

            string content = File.ReadAllText(outputFilePath);

            // 验证目录结构
            Assert.IsTrue(content.Contains("📁 Controllers"), "Should contain Controllers directory");
            Assert.IsTrue(content.Contains("📁 Models"), "Should contain Models directory");
            Assert.IsTrue(content.Contains("📁 Views"), "Should contain Views directory");
            Assert.IsTrue(content.Contains("📁 wwwroot"), "Should contain wwwroot directory");

            // 验证文件
            Assert.IsTrue(content.Contains("Program.cs"), "Should contain Program.cs file");
            Assert.IsTrue(content.Contains("appsettings.json"), "Should contain appsettings.json file");
        }

        [TestMethod]
        public void GenerateTree_WinFormsApp_CreatesExpectedStructure()
        {
            // Arrange
            var formsPath = Path.Combine(_testRootPath, "WinFormsApp");
            var formsOutputPath = Path.Combine(_permanentOutputPath, "WinFormsApp");
            Directory.CreateDirectory(formsOutputPath);

            // Act
            _generator.GenerateTree(formsPath, formsOutputPath);

            // Assert
            string outputFilePath = Path.Combine(formsOutputPath, _config.OutputFileName);
            Assert.IsTrue(File.Exists(outputFilePath), "Output file should be created");

            string content = File.ReadAllText(outputFilePath);

            // 验证目录结构
            Assert.IsTrue(content.Contains("📁 Forms"), "Should contain Forms directory");
            Assert.IsTrue(content.Contains("📁 Models"), "Should contain Models directory");
            Assert.IsTrue(content.Contains("📁 Controls"), "Should contain Controls directory");
            Assert.IsTrue(content.Contains("📁 Services"), "Should contain Services directory");

            // 验证文件
            Assert.IsTrue(content.Contains("Program.cs"), "Should contain Program.cs file");
            Assert.IsTrue(content.Contains("MainForm.cs"), "Should contain MainForm.cs file");
            Assert.IsTrue(content.Contains("App.config"), "Should contain App.config file");
        }

        [TestMethod]
        public void GenerateTree_WithStatistics_IncludesCorrectCounts()
        {
            // Arrange
            var webPath = Path.Combine(_testRootPath, "WebApplication");

            // Act
            _generator.GenerateTree(webPath, _outputPath);
            var stats = _generator.GetStatistics();

            // Assert
            Assert.IsTrue(stats.TotalDirectories > 0, "Should have directories");
            Assert.IsTrue(stats.TotalFiles > 0, "Should have files");
            Assert.IsTrue(stats.TotalSize > 0, "Should have total size");
            Assert.IsTrue(stats.ExtensionCounts.Count > 0, "Should have extension counts");
        }

        [TestMethod]
        public void GenerateTree_WithCustomConfig_RespectsSettings()
        {
            // Arrange
            var customConfig = new GeneratorConfig
            {
                OutputFileName = "custom-structure.md",
                IncludeFileSize = false,
                IncludeLastModified = false,
                DirectoryPrefix = "📂",
                DefaultFilePrefix = "📄",
                IndentSpaces = 4,
                HeaderText = "# Custom Project Structure"
            };

            var generator = new DirectoryTreeGenerator(customConfig);
            var formsPath = Path.Combine(_testRootPath, "WinFormsApp");
            var customOutputPath = Path.Combine(_permanentOutputPath, "Custom-WinFormsApp");
            Directory.CreateDirectory(customOutputPath);

            // Act
            generator.GenerateTree(formsPath, customOutputPath);

            // Assert
            string outputFilePath = Path.Combine(customOutputPath, customConfig.OutputFileName);
            string content = File.ReadAllText(outputFilePath);

            Assert.IsTrue(content.Contains("📂"), "Should use custom directory prefix");
            Assert.IsFalse(Regex.IsMatch(content, @"\(\d+(\.\d+)?\s*(B|KB|MB|GB)\)"), "Should not include file sizes");
            Assert.IsFalse(Regex.IsMatch(content, @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}"), "Should not include timestamps");
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void GenerateTree_InvalidPath_ThrowsException()
        {
            // Arrange
            string invalidPath = Path.Combine(_testRootPath, "NonExistentDirectory");

            // Act
            _generator.GenerateTree(invalidPath, _outputPath);

            // Assert is handled by ExpectedException
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                // 只清理臨時測試目錄，保留永久輸出目錄
                if (Directory.Exists(_testRootPath))
                {
                    Directory.Delete(_testRootPath, true);
                }
            }
            catch (Exception)
            {
                // 忽略清理錯誤
            }
        }
    }
}