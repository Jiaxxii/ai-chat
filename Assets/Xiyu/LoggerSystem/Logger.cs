// #define USER_DEBUG
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xiyu.Constant;

namespace Xiyu.LoggerSystem
{
    public abstract class Logger : ILogger
    {
        public string Name { get; set; }
        public string PatternLayout { get; set; }

        protected Logger(string patternLayout)
        {
            PatternLayout = string.IsNullOrEmpty(patternLayout) ? GameConstant.LoggerDefaultPatternLayout : patternLayout;
        }

        protected Logger()
        {
            PatternLayout = GameConstant.LoggerDefaultPatternLayout;
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


        public void LogInfo(string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            Log(LogLevel.Info, message, stackTrace, cancellationToken);
        }

        public void LogWarning(string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            Log(LogLevel.Warn, message, stackTrace, cancellationToken);
        }

        public void LogError(string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            Log(LogLevel.Error, message, stackTrace, cancellationToken);
        }

        public async void ThrowFail(string message)
        {
            var logMessage = GetLogMessage(LogLevel.Fail, message, true);
            await SaveAsync(logMessage);
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning("game quit");
            UnityEngine.Debug.Break();
#else
            UnityEngine.Application.Quit(-1);
#endif
        }


        public async void Log(LogLevel logLevel, string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR && USER_DEBUG
            switch (logLevel)
            {
                case LogLevel.Info:
                    UnityEngine.Debug.Log(message);
                    break;
                case LogLevel.Warn:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case LogLevel.Error or LogLevel.Fail:
                    UnityEngine.Debug.LogError(message);
                    break;
                default:
                    UnityEngine.Debug.LogError($"type:{logLevel} {message}");
                    break;
            }
#endif
            var logMessage = GetLogMessage(logLevel, message, stackTrace);
            await SaveAsync(logMessage, cancellationToken);
        }


        protected virtual string GetLogMessage(LogLevel logLevel, string message, bool stackTrace = false)
        {
            var stringBuilder = new StringBuilder(PatternLayout);
            stringBuilder.Replace($"%d{{{TimeFormat}}}", DateTime.Now.ToString(TimeFormat));
            stringBuilder.Replace("%t", Environment.CurrentManagedThreadId.ToString());
            stringBuilder.Replace("%level", logLevel.ToString().ToUpperInvariant());
            stringBuilder.Replace("%logger", Name);
            stringBuilder.Replace("%msg", message);

            return stackTrace ? stringBuilder.AppendLine(new StackTrace(true).ToString()).AppendLine().ToString() : stringBuilder.AppendLine().ToString();
        }

        protected abstract Task SaveAsync(string content, CancellationToken cancellationToken = default);


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