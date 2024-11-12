using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.Constant;

namespace Xiyu.LoggerSystem
{
    public class LoggerManager : MonoBehaviour, ILogger
    {
        private static readonly Lazy<LoggerManager> Lazy = new(FindObjectOfType<LoggerManager>);
        public static LoggerManager Instance => Lazy.Value;

        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TextMeshProUGUI logOutputUGUI;
        [SerializeField] private RectTransform basePanel;


        private bool _isRolling;


        private readonly List<Logger> _loggers = new()
        {
            new FileLogger
            {
                Name = GameConstant.LoggerDefaultFileLoggerName
            }
        };

        private readonly System.Text.StringBuilder _logContentStringBuilder = new(1_0_2_4 * 1_0);

        protected void Awake()
        {
            _loggers.Add(new ConsoleLogger(ConsoleLogSaveAsync)
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

            SetActive(false);
        }

        public void SetActive(bool value)
        {
            basePanel.gameObject.SetActive(value);
            if (value)
                basePanel.transform.SetAsLastSibling();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Escape))
            {
                SetActive(!basePanel.gameObject.activeSelf);
                if (basePanel.gameObject.activeSelf)
                {
                    ToFallRoll(1.75F);
                }
            }
        }


        private async UniTask ConsoleLogSaveAsync(string content, CancellationToken cancellationToken)
        {
            logOutputUGUI.text = _logContentStringBuilder.AppendLine(content).ToString();
            await UniTask.NextFrame();


            ToFallRoll(1);
        }


        private void ToFallRoll(float duration)
        {
            if (_isRolling)
            {
                return;
            }

            _isRolling = true;
            DOTween.To(() => scrollRect.verticalNormalizedPosition, v => scrollRect.verticalNormalizedPosition = v, 0, duration)
                .SetEase(Ease.OutQuint)
                .OnComplete(() => _isRolling = false);
        }


        public async UniTask LogAsync(LogLevel logLevel, string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            await UniTask.WhenAll(_loggers.Select(l => l.LogAsync(logLevel, message, stackTrace, cancellationToken)));
        }

        public async UniTaskVoid LogForget(LogLevel logLevel, string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            await LogAsync(logLevel, message, stackTrace, cancellationToken);
        }

        public void Log(LogLevel logLevel, string message, bool stackTrace = false)
        {
            LogForget(logLevel, message, stackTrace, CancellationToken.None).Forget();
        }

        #region Log

        public UniTask LogInfoAsync([NotNull] string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            return this.LogAsync(LogLevel.Info, message, stackTrace, cancellationToken);
        }

        public UniTask LogWarnAsync([NotNull] string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            return this.LogAsync(LogLevel.Warn, message, stackTrace, cancellationToken);
        }

        public UniTask LogErrorAsync([NotNull] string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            return this.LogAsync(LogLevel.Error, message, stackTrace, cancellationToken);
        }


        public UniTaskVoid LogInfoForget([NotNull] string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            return this.LogForget(LogLevel.Info, message, stackTrace, cancellationToken);
        }

        public UniTaskVoid LogWarnForget([NotNull] string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            return this.LogForget(LogLevel.Warn, message, stackTrace, cancellationToken);
        }

        public UniTaskVoid LogErrorForget([NotNull] string message, bool stackTrace = false, CancellationToken cancellationToken = default)
        {
            return this.LogForget(LogLevel.Error, message, stackTrace, cancellationToken);
        }


        public void LogInfo([NotNull] string message, bool stackTrace = false)
        {
            this.Log(LogLevel.Info, message, stackTrace);
        }

        public void LogWarn([NotNull] string message, bool stackTrace = false)
        {
            this.Log(LogLevel.Warn, message, stackTrace);
        }

        public void LogError([NotNull] string message, bool stackTrace = false)
        {
            this.Log(LogLevel.Error, message, stackTrace);
        }

        #endregion
    }
}