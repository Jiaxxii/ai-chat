using System;
using System.Collections.Generic;
using Xiyu.LoggerSystem;

namespace Xiyu.Desktop.FiniteStateMachine
{
    public class StateMachine
    {
        private readonly Logger _logger = new ConsoleLogger((level, msg) =>
        {
            switch (level)
            {
                case LogLevel.Info:
                    UnityEngine.Debug.Log(msg);
                    break;
                case LogLevel.Warn or LogLevel.Debug:
                    UnityEngine.Debug.LogWarning(msg);
                    break;
                case LogLevel.Error or LogLevel.Fail:
                    UnityEngine.Debug.LogError(msg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        });

        private readonly Dictionary<PointerModel, IPointerState> _stateMap = new();

        private IPointerState _current;


        public bool Add(IPointerState pointerState)
        {
            return _stateMap.TryAdd(pointerState.Model, pointerState);
        }


        public void ChangeState(PointerModel pointerModel)
        {
            if (pointerModel == _current.Model)
            {
                _logger.LogWarning($"切换的状态是当前状态！({pointerModel})");
                return;
            }

            if (!_stateMap.TryGetValue(pointerModel, out var nextPointerState))
            {
                _logger.LogError($"未定义的状态:{pointerModel}");
                return;
            }

            // _current?.OnClickExit();
            // (_current = nextPointerState).OnClickEnter();
        }
    }
}