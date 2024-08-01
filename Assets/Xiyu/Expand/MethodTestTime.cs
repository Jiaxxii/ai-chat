using System;
using System.Diagnostics;
using Xiyu.LoggerSystem;

namespace Xiyu.Expand
{
    public sealed class MethodTestTime : IDisposable
    {
        public event Action<TimeSpan> OnStopWatchElapsedComplete;

        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        public MethodTestTime(Action<TimeSpan> onStopWatchElapsedComplete)
        {
            OnStopWatchElapsedComplete += onStopWatchElapsedComplete;
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            OnStopWatchElapsedComplete?.Invoke(_stopwatch.Elapsed);
        }

        public static MethodTestTime CreateDebugTimeTest(Action<TimeSpan> onStopWatchElapsedComplete = null)
        {
            var methodTestTime = new MethodTestTime(onStopWatchElapsedComplete);

            methodTestTime.OnStopWatchElapsedComplete += timeSpan => LoggerManager.Instance.LogInfo($"消耗时间:<color=red>{timeSpan.TotalMilliseconds}</color> ms");

            return methodTestTime;
        }
    }
}