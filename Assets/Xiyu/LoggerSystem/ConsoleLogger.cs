#nullable enable
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Xiyu.LoggerSystem
{
    public class ConsoleLogger : Logger
    {
        public event Func<string, CancellationToken, UniTask> OnSaveMethodAsync;

        public LogLevel LogLevel { get; private set; }

        private ConsoleLogger()
        {
            throw new NotImplementedException($"成员\"{nameof(OnSaveMethodAsync)}\"未初始化！");
        }

        public ConsoleLogger(Func<string, CancellationToken, UniTask> saveMethodAsync)
        {
            OnSaveMethodAsync = saveMethodAsync;
        }


        protected override string GetLogMessage(LogLevel logLevel, string message, bool stackTrace = false)
        {
            LogLevel = logLevel;
            var stringBuilder = new StringBuilder(PatternLayout);
            stringBuilder.Replace($"%d{{{TimeFormat}}}", $"<color=#8bfff5>{DateTime.Now.ToString(TimeFormat)}</color>");

            var currentManagedThreadId = Environment.CurrentManagedThreadId;
            stringBuilder.Replace("%t", $"<color=#b400ff>{(currentManagedThreadId == 1 ? "PlayerLoop" : currentManagedThreadId.ToString())}</color>");
            stringBuilder.Replace("%level", logLevel switch
            {
                LogLevel.Error or LogLevel.Fail => "<color=#ff0000>error</color>",
                LogLevel.Info => "<color=#00ff78>info</color>",
                LogLevel.Debug or LogLevel.Warn => $"<color=#f7ff15>{logLevel.ToString().ToLowerInvariant()}</color>",
                _ => "<color=#808080>undefined</color>"
            });
            stringBuilder.Replace("%logger", $"<color=#ff005a>{Name}</color>");
            stringBuilder.Replace("%msg", $"<color=#00ff60>{message}</color>");

            return stackTrace
                ? stringBuilder.AppendLine().Append("<color=#949695>").Append(new StackTrace(true)).Append("</color>").ToString()
                : stringBuilder.AppendLine().ToString();
        }

        protected override async UniTask SaveAsync(string content, CancellationToken cancellationToken)
        {
            await OnSaveMethodAsync.Invoke(content, cancellationToken);
        }

        protected override async UniTaskVoid SaveForget(string content, CancellationToken cancellationToken)
        {
            await SaveAsync(content, cancellationToken);
        }

        protected override void Save(string content)
        {
            SaveForget(content, CancellationToken.None).Forget();
        }
    }
}