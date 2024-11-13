using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace  ozakboy.DirectoryTreeGenerator
{

    public class DirectoryTreeGenerator
    {
        private readonly GeneratorConfig _config;
        private readonly StringBuilder _treeBuilder;
        private int _indentLevel;

        public DirectoryTreeGenerator(GeneratorConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _treeBuilder = new StringBuilder();
            _indentLevel = 0;
        }

        public void GenerateTree(string rootPath, string outputPath)
        {
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"Directory not found: {rootPath}");

            _treeBuilder.Clear();
            _treeBuilder.AppendLine("# Project Directory Structure");
            _treeBuilder.AppendLine();

            ProcessDirectory(new DirectoryInfo(rootPath));

            File.WriteAllText(Path.Combine(outputPath, _config.OutputFileName), _treeBuilder.ToString());
        }

        private void ProcessDirectory(DirectoryInfo directory)
        {
            if (ShouldIgnoreDirectory(directory.Name))
                return;

            AppendLine($"📁 {directory.Name}/");

            _indentLevel++;

            // Process all files first
            foreach (var file in directory.GetFiles().OrderBy(f => f.Name))
            {
                if (!ShouldIgnoreFile(file.Name))
                {
                    AppendLine($"📄 {file.Name}");
                }
            }

            // Then process all directories
            foreach (var dir in directory.GetDirectories().OrderBy(d => d.Name))
            {
                ProcessDirectory(dir);
            }

            _indentLevel--;
        }

        private bool ShouldIgnoreDirectory(string directoryName)
        {
            return _config.IgnoreDirectories.Contains(directoryName, StringComparer.OrdinalIgnoreCase);
        }

        private bool ShouldIgnoreFile(string fileName)
        {
            if (_config.IgnoreFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                return true;

            string extension = Path.GetExtension(fileName);
            return _config.IgnoreExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        private void AppendLine(string content)
        {
            _treeBuilder.AppendLine($"{new string(' ', _indentLevel * 2)}{content}");
        }
    }
    public class TreeStatistics
    {
        public int TotalDirectories { get; private set; }
        public int TotalFiles { get; private set; }
        public long TotalSize { get; private set; }
        public Dictionary<string, int> ExtensionCounts { get; private set; }

        public TreeStatistics()
        {
            ExtensionCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddFile(FileInfo file)
        {
            TotalFiles++;
            TotalSize += file.Length;

            string ext = file.Extension.ToLowerInvariant();
            if (!string.IsNullOrEmpty(ext))
            {
                if (!ExtensionCounts.ContainsKey(ext))
                    ExtensionCounts[ext] = 0;
                ExtensionCounts[ext]++;
            }
        }

        public void AddDirectory()
        {
            TotalDirectories++;
        }

        public string GenerateReport()
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("\n## Directory Statistics");
            report.AppendLine($"- Total Directories: {TotalDirectories:N0}");
            report.AppendLine($"- Total Files: {TotalFiles:N0}");
            report.AppendLine($"- Total Size: {FormatFileSize(TotalSize)}");

            if (ExtensionCounts.Any())
            {
                report.AppendLine("\n### File Extensions");
                foreach (var ext in ExtensionCounts.OrderByDescending(x => x.Value))
                {
                    report.AppendLine($"- {ext.Key}: {ext.Value:N0} files");
                }
            }

            return report.ToString();
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }
}