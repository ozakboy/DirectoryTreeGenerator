# DirectoryTreeGenerator

[![NuGet](https://img.shields.io/nuget/v/ozakboy.DirectoryTreeGenerator.svg)](https://www.nuget.org/packages/ozakboy.DirectoryTreeGenerator/)

DirectoryTreeGenerator 是一個強大的 .NET 函式庫，可以自動生成專案目錄結構的 Markdown 文檔。它能夠遍歷指定目錄，並生成一個包含圖示的目錄樹結構文件，幫助開發者更好地理解和展示專案結構。

## 特色功能

- 📁 自動生成帶有圖示的目錄樹結構
- 🔍 支援自訂忽略規則（檔案、目錄、副檔名）
- 📊 可選的統計資訊（檔案數量、大小等）
- ⚙️ 透過 JSON 配置文件靈活設定
- 🔄 支援 MSBuild 整合，可在專案建構時自動生成
- 🎯 支援多個 .NET 版本（.NET Standard 2.0/2.1, .NET 6.0+, .NET Framework 4.6.2）

## 安裝

### 使用 NuGet Package Manager

```bash
Install-Package ozakboy.DirectoryTreeGenerator
```

### 使用 .NET CLI

```bash
dotnet add package ozakboy.DirectoryTreeGenerator
```

## 基本使用

### 1. 配置文件設定

在專案根目錄創建 `directorytree.json` 文件：

```json
{
  "outputFileName": "DirectoryStructure.md",
  "ignorePatterns": [
    "**/bin/**",
    "**/obj/**",
    "**/.vs/**"
  ],
  "includeFileSize": true,
  "includeLastModified": true,
  "includeStatistics": true,
  "sortDirectoriesFirst": true
}
```

### 2. 專案檔整合

在專案檔 (.csproj) 中添加以下設定：

```xml
<PropertyGroup>
  <GenerateDirectoryTree>true</GenerateDirectoryTree>
  <DirectoryTreeConfigPath>$(MSBuildProjectDirectory)/directorytree.json</DirectoryTreeConfigPath>
</PropertyGroup>
```

### 3. 程式碼中使用

```csharp
using ozakboy.DirectoryTreeGenerator;

// 創建配置
var config = new GeneratorConfig
{
    OutputFileName = "DirectoryStructure.md",
    IncludeFileSize = true,
    IncludeStatistics = true
};

// 初始化生成器
var generator = new DirectoryTreeGenerator(config);

// 生成目錄樹
generator.GenerateTree("要掃描的目錄路徑", "輸出目錄路徑");
```

## 配置選項

### GeneratorConfig 類別屬性

| 屬性名稱 | 類型 | 預設值 | 說明 |
|----------|------|--------|------|
| OutputFileName | string | "DirectoryStructure.md" | 輸出文件名稱 |
| IgnorePatterns | string[] | ["**/bin/**", ...] | 要忽略的檔案/目錄模式 |
| IgnoreDirectories | string[] | [] | 要忽略的目錄名稱列表 |
| IgnoreFiles | string[] | [] | 要忽略的檔案名稱列表 |
| IgnoreExtensions | string[] | [] | 要忽略的副檔名列表 |
| IncludeFileSize | bool | false | 是否顯示檔案大小 |
| IncludeLastModified | bool | false | 是否顯示最後修改時間 |
| IncludeStatistics | bool | false | 是否包含統計資訊 |
| SortDirectoriesFirst | bool | true | 是否將目錄排在檔案前面 |

## 輸出範例

```markdown
# Project Directory Structure

📁 MyProject/
  📁 src/
    📄 Program.cs (2.5 KB)
    📄 Config.json (1.2 KB)
  📁 tests/
    📄 UnitTests.cs (3.8 KB)

## 目錄統計資訊
- 總目錄數：2
- 總文件數：3
- 總大小：7.5 KB

### 文件類型統計
- .cs：2 個文件
- .json：1 個文件
```

## 進階功能

### 自訂忽略規則

支援多種忽略規則設定：

```json
{
  "ignorePatterns": ["**/temp/**"],
  "ignoreDirectories": ["node_modules", "bin"],
  "ignoreFiles": ["thumbs.db"],
  "ignoreExtensions": [".log", ".tmp"]
}
```

### MSBuild 整合

當設定 `GenerateDirectoryTree` 為 `true` 時，目錄樹將在以下情況自動生成：
- 專案建構完成後
- 發布專案時
- 打包 NuGet 套件時

## 支援的平台

- .NET Standard 2.0
- .NET Standard 2.1
- .NET 6.0
- .NET 7.0
- .NET 8.0
- .NET Framework 4.6.2

## 貢獻

歡迎提交 Pull Request 或建立 Issue 來改善這個專案。

## 授權

本專案採用 MIT 授權條款 - 詳見 [LICENSE](LICENSE) 文件