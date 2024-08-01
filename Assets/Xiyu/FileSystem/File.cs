using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Xiyu.Application;

namespace Xiyu.FileSystem
{
    public static class File
    {
        [CanBeNull]
        internal static string ReadAllText(string filePath)
        {
            filePath = RelativeToAbsolutePath(filePath);

            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath);
            }

            System.IO.File.Create(filePath).Close();

            return string.Empty;
        }


        [ItemCanBeNull]
        internal static Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default)
        {
            // 判断是否是相对路径
            if (filePath.StartsWith("./"))
            {
                // 相对路径要转为绝对路径
                filePath = $"{ApplicationData.MainDataDirectory}{Path.Combine(filePath.Substring(1, filePath.Length - 1))}";
            }

            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllTextAsync(filePath, cancellationToken);
            }

            System.IO.File.Create(filePath).Close();

            return Task.FromResult(string.Empty);
        }


        internal static void WriteAllText(string filePath, string content)
        {
            var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filePath)!);

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            System.IO.File.WriteAllText(RelativeToAbsolutePath(filePath), content);
        }

        internal static Task WriteAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default)
        {
            var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filePath)!);

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            return System.IO.File.WriteAllTextAsync(RelativeToAbsolutePath(filePath), content, cancellationToken);
        }


        internal static void AppendAllText(string filePath, string content)
        {
            var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filePath)!);

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            System.IO.File.AppendAllText(RelativeToAbsolutePath(filePath), content);
        }

        internal static Task AppendAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default)
        {
            var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filePath)!);

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            return System.IO.File.AppendAllTextAsync(RelativeToAbsolutePath(filePath), content, cancellationToken);
        }

        public static string RelativeToAbsolutePath(string path)
        {
            // 判断是否是相对路径
            if (path.StartsWith("./") || path.StartsWith(".\\"))
            {
                // 相对路径要转为绝对路径
                path = $"{Path.Combine(ApplicationData.MainDataDirectory, path.Substring(2, path.Length - 2))}";
            }

            return path;
        }
    }
}