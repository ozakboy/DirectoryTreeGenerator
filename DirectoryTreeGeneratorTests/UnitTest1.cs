using ozakboy.DirectoryTreeGenerator;

namespace DirectoryTreeGeneratorTests
{
    [TestClass]
    public class DirectoryTreeGeneratorTests
    {
        private string testRootPath;
        private GeneratorConfig defaultConfig;

        [TestInitialize]
        public void Setup()
        {
            // 設定測試根目錄
            testRootPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "TestData"
            );

            // 建立測試目錄結構
            TestDirectoryStructures.CreateAllTestStructures(testRootPath);

            // 設定預設配置
            defaultConfig = new GeneratorConfig
            {
                OutputFileName = "TestDirectoryStructure.md",
                IncludeFileSize = true,
                IncludeLastModified = true,
                IncludeStatistics = true,
                DirectoryPrefix = "📁",
                FilePrefix = "📄",
                IgnorePatterns = new[]
                {
                    "**/bin/**",
                    "**/obj/**",
                    "**/.vs/**",
                    "**/node_modules/**"
                }
            };
        }

        [TestMethod]
        public void TestVueProjectStructure()
        {
            // Arrange
            var generator = new DirectoryTreeGenerator(defaultConfig);
            var vuePath = Path.Combine(testRootPath, "vue-vite-project");

            // Act
            generator.GenerateTree(vuePath, vuePath);

            // Assert
            string outputPath = Path.Combine(vuePath, defaultConfig.OutputFileName);
            Assert.IsTrue(File.Exists(outputPath), "Output file should be created");

            string content = File.ReadAllText(outputPath);
            Assert.IsTrue(content.Contains("src"), "Should contain src directory");
            Assert.IsTrue(content.Contains("components"), "Should contain components directory");
            Assert.IsTrue(content.Contains("App.vue"), "Should contain App.vue file");
        }

        [TestMethod]
        public void TestDotNetWebProjectStructure()
        {
            // Arrange
            var generator = new DirectoryTreeGenerator(defaultConfig);
            var webPath = Path.Combine(testRootPath, "WebApplication");

            // Act
            generator.GenerateTree(webPath, webPath);

            // Assert
            string outputPath = Path.Combine(webPath, defaultConfig.OutputFileName);
            Assert.IsTrue(File.Exists(outputPath), "Output file should be created");

            string content = File.ReadAllText(outputPath);
            Assert.IsTrue(content.Contains("Controllers"), "Should contain Controllers directory");
            Assert.IsTrue(content.Contains("Views"), "Should contain Views directory");
            Assert.IsTrue(content.Contains("wwwroot"), "Should contain wwwroot directory");
        }

        [TestMethod]
        public void TestDotNetFormProjectStructure()
        {
            // Arrange
            var generator = new DirectoryTreeGenerator(defaultConfig);
            var formPath = Path.Combine(testRootPath, "WinFormsApp");

            // Act
            generator.GenerateTree(formPath, formPath);

            // Assert
            string outputPath = Path.Combine(formPath, defaultConfig.OutputFileName);
            Assert.IsTrue(File.Exists(outputPath), "Output file should be created");

            string content = File.ReadAllText(outputPath);
            Assert.IsTrue(content.Contains("Forms"), "Should contain Forms directory");
            Assert.IsTrue(content.Contains("Controls"), "Should contain Controls directory");
            Assert.IsTrue(content.Contains("MainForm.cs"), "Should contain MainForm.cs file");
        }

        [TestCleanup]
        public void Cleanup()
        {
        
        }
    }
}