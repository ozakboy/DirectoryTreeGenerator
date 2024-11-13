using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DirectoryTreeGeneratorTests
{
    /// <summary>
    /// 測試目錄結構生成器
    /// </summary>
    public class TestDirectoryStructures
    {
        /// <summary>
        /// 生成Vue3 + Vite專案結構
        /// </summary>
        public static void CreateVueViteProject(string basePath)
        {
            var projectPath = Path.Combine(basePath, "vue-vite-project");
            Directory.CreateDirectory(projectPath);

            // 建立目錄結構
            var directories = new[]
            {
                Path.Combine(projectPath, "src"),
                Path.Combine(projectPath, "src", "assets"),
                Path.Combine(projectPath, "src", "components"),
                Path.Combine(projectPath, "src", "views"),
                Path.Combine(projectPath, "src", "router"),
                Path.Combine(projectPath, "src", "stores"),
                Path.Combine(projectPath, "src", "styles"),
                Path.Combine(projectPath, "public"),
                Path.Combine(projectPath, "tests"),
                Path.Combine(projectPath, "tests", "unit"),
                Path.Combine(projectPath, "tests", "e2e"),
            };

            foreach (var dir in directories)
            {
                Directory.CreateDirectory(dir);
            }

            // 建立檔案
            var files = new Dictionary<string, string>
            {
                { Path.Combine(projectPath, "package.json"), "{ \"name\": \"vue-test-project\" }" },
                { Path.Combine(projectPath, "vite.config.js"), "export default {}" },
                { Path.Combine(projectPath, "index.html"), "<!DOCTYPE html>" },
                { Path.Combine(projectPath, ".gitignore"), "node_modules/" },
                { Path.Combine(projectPath, "src", "main.js"), "import { createApp } from 'vue'" },
                { Path.Combine(projectPath, "src", "App.vue"), "<template></template>" },
                { Path.Combine(projectPath, "src", "components", "HelloWorld.vue"), "<template></template>" },
                { Path.Combine(projectPath, "src", "views", "HomeView.vue"), "<template></template>" },
                { Path.Combine(projectPath, "src", "router", "index.js"), "import { createRouter } from 'vue-router'" },
                { Path.Combine(projectPath, "src", "stores", "counter.js"), "import { defineStore } from 'pinia'" },
                { Path.Combine(projectPath, "src", "assets", "logo.svg"), "<svg></svg>" },
                { Path.Combine(projectPath, "src", "styles", "main.css"), "/* styles */" },
                { Path.Combine(projectPath, "tests", "unit", "example.spec.js"), "describe('test')" },
                { Path.Combine(projectPath, "tests", "e2e", "test.cy.js"), "describe('test')" },
            };

            foreach (var file in files)
            {
                File.WriteAllText(file.Key, file.Value);
            }
        }

        /// <summary>
        /// 生成.NET Web專案結構
        /// </summary>
        public static void CreateDotNetWebProject(string basePath)
        {
            var projectPath = Path.Combine(basePath, "WebApplication");
            Directory.CreateDirectory(projectPath);

            // 建立目錄結構
            var directories = new[]
            {
                Path.Combine(projectPath, "Controllers"),
                Path.Combine(projectPath, "Models"),
                Path.Combine(projectPath, "Views"),
                Path.Combine(projectPath, "Views", "Home"),
                Path.Combine(projectPath, "Views", "Shared"),
                Path.Combine(projectPath, "Services"),
                Path.Combine(projectPath, "Data"),
                Path.Combine(projectPath, "wwwroot"),
                Path.Combine(projectPath, "wwwroot", "css"),
                Path.Combine(projectPath, "wwwroot", "js"),
                Path.Combine(projectPath, "wwwroot", "lib"),
                Path.Combine(projectPath, "Properties"),
            };

            foreach (var dir in directories)
            {
                Directory.CreateDirectory(dir);
            }

            // 建立檔案
            var files = new Dictionary<string, string>
            {
                { Path.Combine(projectPath, "WebApplication.csproj"), "<Project Sdk=\"Microsoft.NET.Sdk.Web\">" },
                { Path.Combine(projectPath, "Program.cs"), "var builder = WebApplication.CreateBuilder(args);" },
                { Path.Combine(projectPath, "appsettings.json"), "{ \"Logging\": { } }" },
                { Path.Combine(projectPath, "appsettings.Development.json"), "{ \"Logging\": { } }" },
                { Path.Combine(projectPath, "Controllers", "HomeController.cs"), "public class HomeController : Controller" },
                { Path.Combine(projectPath, "Models", "ErrorViewModel.cs"), "public class ErrorViewModel" },
                { Path.Combine(projectPath, "Views", "_ViewImports.cshtml"), "@using WebApplication" },
                { Path.Combine(projectPath, "Views", "_ViewStart.cshtml"), "@{ Layout = \"_Layout\"; }" },
                { Path.Combine(projectPath, "Views", "Shared", "_Layout.cshtml"), "<!DOCTYPE html>" },
                { Path.Combine(projectPath, "Views", "Home", "Index.cshtml"), "@{ ViewData[\"Title\"] = \"Home\"; }" },
                { Path.Combine(projectPath, "wwwroot", "css", "site.css"), "body { }" },
                { Path.Combine(projectPath, "wwwroot", "js", "site.js"), "// JavaScript" },
                { Path.Combine(projectPath, "Properties", "launchSettings.json"), "{ \"profiles\": { } }" },
            };

            foreach (var file in files)
            {
                File.WriteAllText(file.Key, file.Value);
            }
        }

        /// <summary>
        /// 生成.NET Windows Forms專案結構
        /// </summary>
        public static void CreateDotNetFormProject(string basePath)
        {
            var projectPath = Path.Combine(basePath, "WinFormsApp");
            Directory.CreateDirectory(projectPath);

            // 建立目錄結構
            var directories = new[]
            {
                Path.Combine(projectPath, "Forms"),
                Path.Combine(projectPath, "Models"),
                Path.Combine(projectPath, "Services"),
                Path.Combine(projectPath, "Properties"),
                Path.Combine(projectPath, "Resources"),
                Path.Combine(projectPath, "Controls"),
                Path.Combine(projectPath, "Utils"),
            };

            foreach (var dir in directories)
            {
                Directory.CreateDirectory(dir);
            }

            // 建立檔案
            var files = new Dictionary<string, string>
            {
                { Path.Combine(projectPath, "WinFormsApp.csproj"), "<Project Sdk=\"Microsoft.NET.Sdk\">" },
                { Path.Combine(projectPath, "Program.cs"), "static class Program { static void Main() { } }" },
                { Path.Combine(projectPath, "App.config"), "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" },
                { Path.Combine(projectPath, "Forms", "MainForm.cs"), "public partial class MainForm : Form" },
                { Path.Combine(projectPath, "Forms", "MainForm.Designer.cs"), "partial class MainForm" },
                { Path.Combine(projectPath, "Forms", "SettingsForm.cs"), "public partial class SettingsForm : Form" },
                { Path.Combine(projectPath, "Forms", "SettingsForm.Designer.cs"), "partial class SettingsForm" },
                { Path.Combine(projectPath, "Models", "AppSettings.cs"), "public class AppSettings" },
                { Path.Combine(projectPath, "Services", "DataService.cs"), "public class DataService" },
                { Path.Combine(projectPath, "Controls", "CustomControl.cs"), "public class CustomControl : UserControl" },
                { Path.Combine(projectPath, "Utils", "Helper.cs"), "public static class Helper" },
                { Path.Combine(projectPath, "Properties", "Settings.settings"), "<?xml version='1.0' encoding='utf-8'?>" },
                { Path.Combine(projectPath, "Resources", "app.ico"), "" },
            };

            foreach (var file in files)
            {
                File.WriteAllText(file.Key, file.Value);
            }
        }

        /// <summary>
        /// 生成完整的測試目錄結構
        /// </summary>
        public static void CreateAllTestStructures(string basePath)
        {
            // 清理並建立基礎目錄
            if (Directory.Exists(basePath))
                Directory.Delete(basePath, true);

            Directory.CreateDirectory(basePath);

            // 生成所有專案結構
            CreateVueViteProject(basePath);
            CreateDotNetWebProject(basePath);
            CreateDotNetFormProject(basePath);

            // 在根目錄添加一些通用檔案
            var rootFiles = new Dictionary<string, string>
            {
                { Path.Combine(basePath, ".gitignore"), "bin/\nobj/\nnode_modules/" },
                { Path.Combine(basePath, "README.md"), "# Test Projects" },
                { Path.Combine(basePath, "solution.sln"), "Microsoft Visual Studio Solution File" },
            };

            foreach (var file in rootFiles)
            {
                File.WriteAllText(file.Key, file.Value);
            }
        }
    }
}