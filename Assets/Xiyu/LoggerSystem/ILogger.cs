using System.Threading;
using Cysharp.Threading.Tasks;

namespace Xiyu.LoggerSystem
{
    public interface ILogger
    {
        UniTask LogAsync(LogLevel logLevel, string message, bool stackTrace = false, CancellationToken cancellationToken = default);
        UniTaskVoid LogForget(LogLevel logLevel, string message, bool stackTrace = false, CancellationToken cancellationToken = default);
        void Log(LogLevel logLevel, string message, bool stackTrace = false);
    }
}