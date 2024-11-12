// #define USER_DEBUG

using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using Xiyu.Constant;

namespace Xiyu.LoggerSystem
{
    public abstract class Logger : ILogger
    {
        public string Name { get; set; }
        public string PatternLayout { get; set; } = GameConstant.LoggerDefaultPatternLayout;

        protected Logger(string patternLayout)
        {
            PatternLayout = string.IsNullOrEmpty(patternLayout) ? GameConstant.LoggerDefaultPatternLayout : patternLayout;
        }

        protected Logger()
        {
        }


        private string _timeFormat;

        public string TimeFormat
        {
            get
            {
                if (string.IsNullOrEmpty(_timeFormat))
                {
                    _ = TryGetTimeFormat(PatternLayout, out _timeFormat);
                }

                return _timeFormat;
            }
        }

        public UniTask LogAsync(LogLevel logLevel, string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            var logMessage = GetLogMessage(logLevel, message, stackTrace);
            return SaveAsync(logMessage, cancellationToken);
        }

        public UniTaskVoid LogForget(LogLevel logLevel, string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            var logMessage = GetLogMessage(logLevel, message, stackTrace);
            return SaveForget(logMessage, cancellationToken);
        }

        public void Log(LogLevel logLevel, string message, bool stackTrace = false)
        {
            var logMessage = GetLogMessage(logLevel, message, stackTrace);
            Save(logMessage);
        }


        protected virtual string GetLogMessage(LogLevel logLevel, string message, bool stackTrace = false)
        {
            var stringBuilder = new StringBuilder(PatternLayout);
            stringBuilder.Replace($"%d{{{TimeFormat}}}", DateTime.Now.ToString(TimeFormat));
            var currentManagedThreadId = Environment.CurrentManagedThreadId;
            stringBuilder.Replace("%t", currentManagedThreadId == 1 ? "PlayerLoop" : currentManagedThreadId.ToString());
            stringBuilder.Replace("%level", logLevel.ToString().ToLowerInvariant());
            stringBuilder.Replace("%logger", Name);
            stringBuilder.Replace("%msg", message);

            return stackTrace ? stringBuilder.AppendLine(new StackTrace(true).ToString()).AppendLine().ToString() : stringBuilder.AppendLine().ToString();
        }


        protected abstract UniTask SaveAsync(string content, CancellationToken cancellationToken);
        protected abstract UniTaskVoid SaveForget(string content, CancellationToken cancellationToken);
        protected abstract void Save(string content);


        protected static bool TryGetTimeFormat(string content, out string format)
        {
            var match = Regex.Match(content, "%[dD]{(?<timeFormat>[^}]+)}");
            var timeFormatGroups = match.Groups["timeFormat"];

            if (timeFormatGroups is { Success: true } && !string.IsNullOrEmpty(timeFormatGroups.Value))
            {
                format = timeFormatGroups.Value;
                return true;
            }

            format = GameConstant.NullFormat;
            return false;
        }
    }
}