namespace DirectoryTreeGeneratorTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.Text.Json;
    using ozakboy.DirectoryTreeGenerator;


    [TestClass]
    public class DirectoryTreeGeneratorTests
    {
        private string testRootPath;
        private GeneratorConfig defaultConfig;

        [TestInitialize]
        public void Setup()
        {
            // 建立測試用的臨時目錄結構
            testRootPath = Path.Combine(Path.GetTempPath(), "DirectoryTreeGeneratorTests");
            if (Directory.Exists(testRootPath))
                Directory.Delete(testRootPath, true);
            Directory.CreateDirectory(testRootPath);

            // 建立測試用的目錄結構
            CreateTestDirectoryStructure();

            // 設定預設配置
            defaultConfig = new GeneratorConfig
            {
                OutputFileName = "TestDirectoryStructure.md",
                IncludeFileSize = false,
                IncludeLastModified = false,
                IncludeStatistics = true,
                DirectoryPrefix = "📁",
                FilePrefix = "📄"
            };
        }

        // 增加一個輔助測試方法來更完整地設置測試環境
        private void CreateTestDirectoryStructure()
        {
            // 建立基本目錄結構
            Directory.CreateDirectory(Path.Combine(testRootPath, "src"));
            Directory.CreateDirectory(Path.Combine(testRootPath, "tests"));
            Directory.CreateDirectory(Path.Combine(testRootPath, "docs"));

            // 建立測試檔案並寫入固定大小的內容
            string testContent = new string('x', 100); // 固定 100 字節的內容

            // 在 src 目錄中建立文件
            File.WriteAllText(Path.Combine(testRootPath, "src", "test1.cs"), testContent);
            File.WriteAllText(Path.Combine(testRootPath, "src", "test2.cs"), testContent);

            // 在 docs 目錄中建立文件
            File.WriteAllText(Path.Combine(testRootPath, "docs", "readme.md"), testContent);

            // 等待一下以確保檔案時間戳不同
            System.Threading.Thread.Sleep(100);
        }

        [TestMethod]
        public void TestBasicDirectoryTreeGeneration()
        {
            // Arrange
            var generator = new DirectoryTreeGenerator(defaultConfig);

            // Act
            generator.GenerateTree(testRootPath, testRootPath);

            // Assert
            string outputPath = Path.Combine(testRootPath, defaultConfig.OutputFileName);
            Assert.IsTrue(File.Exists(outputPath), "Output file should be created");
            string content = File.ReadAllText(outputPath);
            Assert.IsTrue(content.Contains("src"), "Output should contain src directory");
            Assert.IsTrue(content.Contains("tests"), "Output should contain tests directory");
            Assert.IsTrue(content.Contains("docs"), "Output should contain docs directory");
        }

        [TestMethod]
        public void TestIgnorePatterns()
        {
            // Arrange
            defaultConfig.IgnorePatterns = new[] { "**/docs/**" };
            var generator = new DirectoryTreeGenerator(defaultConfig);

            // Act
            generator.GenerateTree(testRootPath, testRootPath);

            // Assert
            string outputPath = Path.Combine(testRootPath, defaultConfig.OutputFileName);
            string content = File.ReadAllText(outputPath);
            Assert.IsFalse(content.Contains("docs"), "Output should not contain ignored docs directory");
        }

        [TestMethod]
        public void TestIgnoreFiles()
        {
            // Arrange
            defaultConfig.IgnoreFiles = new[] { "test1.cs" };
            var generator = new DirectoryTreeGenerator(defaultConfig);

            // Act
            generator.GenerateTree(testRootPath, testRootPath);

            // Assert
            string outputPath = Path.Combine(testRootPath, defaultConfig.OutputFileName);
            string content = File.ReadAllText(outputPath);
            Assert.IsFalse(content.Contains("test1.cs"), "Output should not contain ignored file");
            Assert.IsTrue(content.Contains("test2.cs"), "Output should contain non-ignored file");
        }

        [TestMethod]
        public void TestIgnoreExtensions()
        {
            // Arrange
            defaultConfig.IgnoreExtensions = new[] { ".cs" };
            defaultConfig.IncludeFileSize = false;  // 簡化輸出以便測試
            defaultConfig.IncludeLastModified = false;
            var generator = new DirectoryTreeGenerator(defaultConfig);

            // Act
            generator.GenerateTree(testRootPath, testRootPath);

            // Assert
            string outputPath = Path.Combine(testRootPath, defaultConfig.OutputFileName);
            string content = File.ReadAllText(outputPath);

            // 檢查 .cs 檔案確實被忽略
            Assert.IsFalse(content.Contains("test1.cs"), "Output should not contain test1.cs");
            Assert.IsFalse(content.Contains("test2.cs"), "Output should not contain test2.cs");

            // 檢查 .md 檔案存在
            Assert.IsTrue(content.Contains("readme.md"), "Output should contain readme.md");

            // 檢查目錄結構仍然存在
            Assert.IsTrue(content.Contains("src"), "Output should still contain src directory");
            Assert.IsTrue(content.Contains("docs"), "Output should still contain docs directory");
        }

        [TestMethod]
        public void TestStatisticsGeneration()
        {
            // Arrange
            defaultConfig.IncludeStatistics = true;
            defaultConfig.IncludeFileSize = true;
            var generator = new DirectoryTreeGenerator(defaultConfig);

            // 新增一些額外的測試檔案以豐富統計資訊
            File.WriteAllText(Path.Combine(testRootPath, "src", "test3.txt"), "test content");
            File.WriteAllText(Path.Combine(testRootPath, "docs", "guide.md"), "test content");

            // Act
            generator.GenerateTree(testRootPath, testRootPath);

            // Assert
            string outputPath = Path.Combine(testRootPath, defaultConfig.OutputFileName);
            string content = File.ReadAllText(outputPath);

            // 檢查統計區段的存在
            Assert.IsTrue(content.Contains("## Directory Statistics"), "Output should contain statistics header");

            // 檢查基本統計資訊
            Assert.IsTrue(content.Contains("Total Directories: 3"), "Should show correct directory count");
            Assert.IsTrue(content.Contains("Total Files: 5"), "Should show correct file count");

            // 檢查副檔名統計
            Assert.IsTrue(content.Contains(".cs:"), "Should contain .cs extension statistics");
            Assert.IsTrue(content.Contains(".md:"), "Should contain .md extension statistics");
            Assert.IsTrue(content.Contains(".txt:"), "Should contain .txt extension statistics");

            // 檢查檔案大小統計
            Assert.IsTrue(content.Contains("Total Size:"), "Should contain total size information");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // 清理測試用的臨時目錄
            if (Directory.Exists(testRootPath))
                Directory.Delete(testRootPath, true);
        }
    }
}