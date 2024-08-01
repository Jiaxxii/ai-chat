using System.Threading;

namespace Xiyu.LoggerSystem
{
    public interface ILogger
    {
        void LogInfo(string message, bool stackTrace = false, CancellationToken cancellationToken = default);

        void LogWarning(string message, bool stackTrace = false, CancellationToken cancellationToken = default);

        void LogError(string message, bool stackTrace = false, CancellationToken cancellationToken = default);

        void ThrowFail(string message);


        void Log(LogLevel logLevel, string message, bool stackTrace = false, CancellationToken cancellationToken = default);
    }
}