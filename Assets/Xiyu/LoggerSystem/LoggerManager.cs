using System;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.Constant;
using Xiyu.Expand;

namespace Xiyu.LoggerSystem
{
    public class LoggerManager : Singleton<LoggerManager>, ILogger
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TextMeshProUGUI logOutputUGUI;
        [SerializeField] private RectTransform basePanel;

        private float _startLogContentSizeY;

        private bool _isRolling;


        private readonly List<Logger> _loggers = new()
        {
            new FileLogger
            {
                Name = GameConstant.LoggerDefaultFileLoggerName
            }
        };

        private readonly System.Text.StringBuilder _logContentStringBuilder = new(1_0_2_4 * 1_0);

        protected override void Awake()
        {
            base.Awake();

            _loggers.Add(new ConsoleLogger(ConsoleLog)
            {
                Name = GameConstant.LoggerDefaultConsoleLoggerName
            });
            UnityEngine.Application.logMessageReceived += (condition, _, type) =>
            {
                var logLevel = type switch
                {
                    LogType.Error or LogType.Assert or LogType.Exception => LogLevel.Error,
                    LogType.Warning => LogLevel.Warn,
                    _ => LogLevel.Fail
                };

                Log(logLevel, condition);
            };

            _startLogContentSizeY = scrollRect.content.sizeDelta.y;
        }

        public void SetActive(bool value)
        {
            basePanel.gameObject.SetActive(value);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                SetActive(!basePanel.gameObject.activeSelf);
            }
        }


        private void ConsoleLog(LogLevel logLevel, string msg)
        {
            var logLevelString = logLevel switch
            {
                LogLevel.Debug or LogLevel.Error or LogLevel.Fail => $"<color=red>{logLevel.ToString()}</color>",
                LogLevel.Info => logLevel.ToString(),
                LogLevel.Warn => $"<color=yellow>{logLevel.ToString()}</color>",
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
            };

            logOutputUGUI.text = _logContentStringBuilder.AppendLine($"{logLevelString} {msg}{Environment.NewLine}").ToString();

            var height = logOutputUGUI.preferredHeight;

            if (height <= _startLogContentSizeY) return;

            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, height);
            _startLogContentSizeY = height;

            if (_isRolling)
            {
                return;
            }

            _isRolling = true;
            DOTween.To(() => scrollRect.verticalNormalizedPosition, v => scrollRect.verticalNormalizedPosition = v, 0, 1)
                .SetEase(Ease.OutQuint)
                .OnComplete(() => _isRolling = false);
        }


        public void LogInfo(string message, bool stackTrace = false, CancellationToken cancellationToken = default) =>
            Log(LogLevel.Info, message, stackTrace, cancellationToken);

        public void LogWarning(string message, bool stackTrace = false, CancellationToken cancellationToken = default) =>
            Log(LogLevel.Warn, message, stackTrace, cancellationToken);

        public void LogError(string message, bool stackTrace = false, CancellationToken cancellationToken = default) =>
            Log(LogLevel.Error, message, stackTrace, cancellationToken);

        public void ThrowFail(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.ThrowFail(message);
            }
        }

        public void Log(LogLevel logLevel, string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            SetActive(true);
            foreach (var logger in _loggers)
            {
                logger.Log(logLevel, message, stackTrace, cancellationToken);
            }
        }
    }
}