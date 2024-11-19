using ozakboy.DirectoryTreeGenerator;
using ozakboy.NLOG;

namespace DirectoryTreeGeneratorConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            LOG.Info_Log($"建立 DirectoryTreeGenerator");
            var generator = new DirectoryTreeGenerator();
            LOG.Info_Log($"開始執行路徑掃描");
            generator.GenerateTree();
            LOG.Info_Log($"掃描的根目錄路徑:{generator.GetInPath()}");
            LOG.Info_Log($"輸出檔案路徑:{generator.GetOutPath()}");
            var Statistics = generator.GetStatistics();
            LOG.Info_Log($"");
            LOG.Info_Log($"### 目錄統計資訊");
            LOG.Info_Log($"- 總目錄數：{Statistics.TotalDirectories}");
            LOG.Info_Log($"- 總文件數：{Statistics.TotalFiles}");
            LOG.Info_Log($"- 總大小：{Statistics.TotalSize}");
            LOG.Info_Log($"- 最後更新：{Statistics.LastModified.ToString("yyyy-MM-dd HH:mm:ss")}");
            LOG.Info_Log($"- 最大文件：{Statistics.LargestFilePath}");
            LOG.Info_Log($"");
            LOG.Info_Log($"### 文件類型統計");
            foreach ( var stat in Statistics.ExtensionCounts)
            {
                LOG.Info_Log($"- {stat.Key}：{stat.Value} 個文件");
            }                  
            LOG.Info_Log($"完成路徑檔案掃描");
        }
    }
}
