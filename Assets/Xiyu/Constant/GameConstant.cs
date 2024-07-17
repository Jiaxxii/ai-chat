using System;

namespace Xiyu.Constant
{
    public static class GameConstant
    {
        public const string MainCanvasName = "Main Canvas";

        public const string Player = "西";

        public const string InputDialogWindow = "Input Dialog Window";

        public const string SelectDialogWindow = "Select Dialog Window";

        public const int MaxEffectiveDay = 7;

        /// <summary>
        /// 访问剪切板心跳间隔
        /// </summary>
        public const float ClipboardAccessHeartbeatSeconds = 1.5F;


        public static readonly DateTime UnixTimeOrigin = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static TimeSpan GetUnixTimeOriginToNow() => DateTime.UtcNow - UnixTimeOrigin;
    }
}