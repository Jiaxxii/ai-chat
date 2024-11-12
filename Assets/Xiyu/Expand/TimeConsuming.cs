#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xiyu.LoggerSystem;
using Debug = UnityEngine.Debug;

namespace Xiyu.Expand
{
    public sealed class TimeConsuming : IDisposable
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private readonly Action<TimeSpan, IEnumerable<TimeSpan>?> _onTimeStop;

        private readonly Action<TimeSpan, IEnumerable<TimeSpan>?> _defaultAction = (elapsed, pauseElapsed) =>
           LoggerManager.Instance.LogInfo($"耗时:<color=red>{elapsed.TotalMilliseconds - (pauseElapsed?.Sum(v => v.TotalMilliseconds) ?? 0):F5}</color> ms");

        private Stopwatch? _pauseStopwatch;
        private readonly Queue<TimeSpan> _pauseElapsedQueue = new();

        private bool _isPause;
        
        // ctor
        public TimeConsuming(Action<TimeSpan, IEnumerable<TimeSpan>?>? stopwatchStop = null) => _onTimeStop = stopwatchStop ?? _defaultAction;


        public bool Pause()
        {
            if (_isPause)
            {
                return false;
            }

            _isPause = true;
            _pauseStopwatch = Stopwatch.StartNew();
            return true;
        }

        public bool Continue()
        {
            if (!_isPause)
            {
                return false;
            }

            _isPause = false;
            _pauseStopwatch!.Stop();
            _pauseElapsedQueue.Enqueue(_pauseStopwatch.Elapsed);

            return true;
        }


        public void Dispose()
        {
            if (_isPause)
            {
                Continue();
            }

            _stopwatch.Stop();
            _onTimeStop.Invoke(_stopwatch.Elapsed, _pauseElapsedQueue);
        }
    }
}