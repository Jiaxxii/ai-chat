using System;
using System.IO;
using Cysharp.Threading.Tasks;
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


        protected override void Save(string content)
        {
            SaveForget(content, System.Threading.CancellationToken.None).Forget();
        }

        protected override async UniTaskVoid SaveForget(string content, System.Threading.CancellationToken cancellationToken)
        {
            await SaveAsync(content, cancellationToken);
        }

        protected override async UniTask SaveAsync(string content, System.Threading.CancellationToken cancellationToken)
        {
            var saveFilePath = Path.Combine(ApplicationData.LoggerPath, FileName);

            if (File.Exists(saveFilePath))
            {
                await FileSystem.File.AppendAllTextAsync(saveFilePath, content, cancellationToken);
                return;
            }

            var suffix = Path.GetExtension(FileName);

            saveFilePath = Path.Combine(ApplicationData.LoggerPath,
                TryGetTimeFormat(DefaultFileName, out var format) ? $"{DateTime.Now.ToString(format)}{suffix}" : $"{DateTime.Now:yy-MM-dd}{suffix}");

            await FileSystem.File.AppendAllTextAsync(saveFilePath, content, cancellationToken);
        }
    }
}