using System;
using UnityEngine;

namespace Xiyu.Constant
{
    public static class GameConstant
    {
        public const string MainCanvasName = "直播画面栏";

        public const string Player = "西";

        public const string InputDialogWindow = "Input Dialog Window";

        public const string SelectDialogWindow = "Select Dialog Window";

        public const string SingleDialogWindow = "Single Dialog Window";
        public const int MaxEffectiveDay = 7;

        public const string LoggerDefaultPatternLayout = "[%d{yyyy-MM-dd HH:mm:ss}] [ThreadID:%t] [%level] <%logger> : %msg";

        public const string LoggerDefaultFileLoggerName = "文件日志";
        public const string LoggerDefaultConsoleLoggerName = "内部控制台";


        public const int MaxDanmuMessageLength = 20 * 4;
        public static readonly Func<string, bool> DefaultDanmuMessageSendBoxSubmitCheck = msg => !string.IsNullOrEmpty(msg) && msg.Length <= MaxDanmuMessageLength;

        // public static readonly string DefaultNavigationPage = "https://www.xiyuVL.com";

        // public static readonly string WebIndexPage = "https://www.xiyu.index.com";

        /// <summary>
        /// 访问剪切板心跳间隔
        /// </summary>
        public const float ClipboardAccessHeartbeatSeconds = 1.5F;


        public const string NullFormat = "NULL_FORMAT";

        public static readonly DateTime UnixTimeOrigin = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static TimeSpan GetUnixTimeOriginToNow() => DateTime.UtcNow - UnixTimeOrigin;
    }
}