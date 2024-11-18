# DirectoryTreeGenerator

[![NuGet](https://img.shields.io/nuget/v/ozakboy.DirectoryTreeGenerator.svg)](https://www.nuget.org/packages/ozakboy.DirectoryTreeGenerator/)

DirectoryTreeGenerator 是一個功能豐富的 .NET 函式庫，專門用於生成目錄結構的 Markdown 文檔。透過這個工具，您可以輕鬆地將專案或任意目錄的結構轉換成清晰易讀的樹狀圖文件，並支援自訂圖示、過濾規則和統計資訊。

## 核心功能

- 🎯 自動掃描並生成目錄結構的 Markdown 文件
- 📁 支援自訂目錄和檔案的圖示
- 🔍 彈性的忽略規則設定（支援檔案、目錄、副檔名和 glob 模式）
- 📊 詳細的統計資訊（總檔案數、大小、類型分布等）
- ⚡ 高效能的檔案系統處理
- 🛠️ 支援多個 .NET 平台（.NET Standard 2.0/2.1, .NET 6.0-8.0, .NET Framework 4.6.2）

## 快速開始

### 安裝套件

使用 NuGet Package Manager:
```bash
Install-Package ozakboy.DirectoryTreeGenerator
```

或使用 .NET CLI:
```bash
dotnet add package ozakboy.DirectoryTreeGenerator
```

### 基本使用

```csharp
// 建立預設配置的生成器
var generator = new DirectoryTreeGenerator();

// 生成目錄樹
generator.GenerateTree(
    "C:\\YourProject",     // 要掃描的根目錄
    "C:\\Output"          // 輸出目錄
);
```

### 使用配置檔案

1. 在專案根目錄建立 `directorytree.json`:

```json
{
  "outputFileName": "DirectoryStructure.md",
  "includeFileSize": true,
  "includeLastModified": true,
  "includeStatistics": true,
  "sortDirectoriesFirst": true,
  "ignorePatterns": [
    "**/bin/**",
    "**/obj/**",
    "**/.vs/**"
  ],
  "fileExtensionIcons": {
    ".cs": "📝",
    ".json": "📋",
    ".md": "📄"
  }
}
```

2. 使用配置檔案初始化生成器:

```csharp
// 使用指定的配置檔案路徑
var generator = new DirectoryTreeGenerator("path/to/directorytree.json");

// 或自動搜尋配置檔案
var generator = new DirectoryTreeGenerator();
```

## 配置選項

### GeneratorConfig 類別屬性

| 屬性 | 類型 | 預設值 | 說明 |
|------|------|--------|------|
| OutputFileName | string | "DirectoryStructure.md" | 輸出檔案名稱 |
| DirectoryPrefix | string | "📁" | 目錄前綴圖示 |
| DefaultFilePrefix | string | "📄" | 預設檔案前綴圖示 |
| IndentSpaces | int | 2 | 縮排空格數 |
| IncludeHeader | bool | true | 是否包含標題 |
| HeaderText | string | "# Project Directory Structure" | 標題文字 |
| IncludeFileSize | bool | false | 是否顯示檔案大小 |
| IncludeLastModified | bool | false | 是否顯示最後修改時間 |
| IncludeStatistics | bool | false | 是否包含統計資訊 |
| SortDirectoriesFirst | bool | true | 是否將目錄排在檔案前面 |

### 忽略規則設定

支援多種忽略規則類型：

```json
{
  "ignorePatterns": ["**/temp/**"],
  "ignoreDirectories": ["node_modules", "bin"],
  "ignoreFiles": ["thumbs.db"],
  "ignoreExtensions": [".log", ".tmp"]
}
```

## 輸出範例

```markdown
# Project Directory Structure

📁 src/
  📝 Program.cs (2.5 KB) - 2024-03-15 14:30:00
  📋 Config.json (1.2 KB) - 2024-03-15 14:25:00
📁 tests/
  📝 UnitTests.cs (3.8 KB) - 2024-03-15 14:35:00

## 目錄統計資訊
- 總目錄數：2
- 總檔案數：3
- 總大小：7.5 KB
- 最後更新：2024-03-15 14:35:00

### 檔案類型統計
- .cs：2 個檔案
- .json：1 個檔案
```

## 進階功能

### 自訂檔案圖示

可以為不同的副檔名設定專屬圖示：

```json
{
  "fileExtensionIcons": {
    ".cs": "📝",
    ".json": "📋",
    ".md": "📄",
    ".txt": "📃",
    ".xml": "📰",
    ".png": "🖼️",
    ".jpg": "🖼️",
    ".pdf": "📚",
    ".zip": "📦",
    ".exe": "⚙️",
    ".dll": "🔧"
  }
}
```

### 程式碼中配置

也可以透過程式碼動態設定配置：

```csharp
var config = new GeneratorConfig
{
    OutputFileName = "MyDirectoryTree.md",
    IncludeFileSize = true,
    IncludeLastModified = true,
    IncludeStatistics = true,
    SortDirectoriesFirst = true,
    IgnorePatterns = new[] { "**/bin/**", "**/obj/**" }
};

var generator = new DirectoryTreeGenerator(config);
```

## 支援的平台

- .NET Standard 2.0
- .NET Standard 2.1
- .NET 6.0
- .NET 7.0
- .NET 8.0
- .NET Framework 4.6.2
