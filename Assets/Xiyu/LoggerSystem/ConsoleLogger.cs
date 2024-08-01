using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Xiyu.LoggerSystem
{
    public class ConsoleLogger : Logger
    {
        public event UnityAction<LogLevel, string> OnTriggerLogHandler;

        public LogLevel LogLevel { get; private set; }

        public ConsoleLogger(UnityAction<LogLevel, string> unityAction) : base() => OnTriggerLogHandler += unityAction;

        protected override string GetLogMessage(LogLevel logLevel, string message, bool stackTrace = false)
        {
            LogLevel = logLevel;
            return base.GetLogMessage(logLevel, message, stackTrace);
        }

        protected override Task SaveAsync(string content, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }

            OnTriggerLogHandler?.Invoke(LogLevel, content);
            return Task.CompletedTask;
        }
    }
}