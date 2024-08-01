using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xiyu.Application;

namespace Xiyu.LoggerSystem
{
    public class FileLogger : Logger
    {
        public const string DefaultFileName = "%d{yy-MM-dd}.log";

        public string FileName { get; }

        public FileLogger() : base() => FileName = DefaultFileName;

        public FileLogger(string fileName) : base()
        {
            FileName = fileName;
        }

        protected override Task SaveAsync(string content, CancellationToken cancellationToken = default)
        {
            var saveFilePath = Path.Combine(ApplicationData.LoggerPath, FileName);

            if (File.Exists(saveFilePath)) return FileSystem.File.AppendAllTextAsync(saveFilePath, content, cancellationToken);

            var suffix = Path.GetExtension(FileName);

            saveFilePath = Path.Combine(ApplicationData.LoggerPath,
                TryGetTimeFormat(DefaultFileName, out var format) ? $"{DateTime.Now.ToString(format)}{suffix}" : $"{DateTime.Now:yy-MM-dd}{suffix}");

            return FileSystem.File.AppendAllTextAsync(saveFilePath, content, cancellationToken);
        }
    }
}