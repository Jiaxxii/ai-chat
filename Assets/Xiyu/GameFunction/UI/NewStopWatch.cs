using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.Expand;

namespace Xiyu.GameFunction.UI
{
    public class NewStopWatch : MonoBehaviour
    {
        [SerializeField] private Image arrowHour;

        [SerializeField] private Image arrowMinute;

        [SerializeField] private Image arrowSeconds;

        private float _arrowHourT;
        private float _arrowMinuteT;
        private float _arrowSecondsT;

        private bool _isStop;

        private Coroutine _coroutine;

        /// <summary>
        /// 设置“时针”指针的弹性动画
        /// </summary>
        public Ease ArrowHourEase { get; set; } = Ease.OutBounce;

        /// <summary>
        /// 设置“分针”指针的弹性动画
        /// </summary>
        public Ease ArrowMinuteEase { get; set; } = Ease.OutBounce;

        /// <summary>
        /// 设置“秒针”指针的弹性动画
        /// </summary>
        public Ease ArrowSecondsEase { get; set; } = Ease.OutBounce;

        /// <summary>
        /// 设置指针移到到目标位置的最小秒数
        /// </summary>
        public float MainArrowMoveDuration { get; set; } = 0.5F;

        /// <summary>
        /// 设置指针移到到目标位置的最大秒数
        /// </summary>
        public float MaxArrowMoveDuration { get; set; } = 3F;

        /// <summary>
        /// 设置指针完成移动后下一次移动间隔
        /// </summary>
        public float NextMoveSeconds { get; set; } = 3F;


        private TimeSpan LastLoadingTimeSpan
        {
            get
            {
                var timeString = PlayerPrefs.GetString(nameof(LastLoadingTimeSpan), DateTime.Now.ToString("hh:mm:ss")).Split(':');


                if (int.TryParse(timeString[0], out var hours) && int.TryParse(timeString[1], out var minute) && int.TryParse(timeString[2], out var seconds))
                {
                    return new TimeSpan(hours, minute, seconds);
                }

                var now = DateTime.Now;

                return new TimeSpan(now.Hour, now.Minute, now.Second);
            }
            set => PlayerPrefs.SetString(nameof(LastLoadingTimeSpan), $"{value.Hours:00}:{value.Minutes:00}:{value.Seconds:00}");
        }

        private void OnEnable()
        {
            // 启用时先设置到上一次时间
            var lastTimeSpan = LastLoadingTimeSpan;
            SetArrowPosition(arrowHour.rectTransform, ((float)lastTimeSpan.Hours).MapFloat(0, 12, 0, 1));
            SetArrowPosition(arrowMinute.rectTransform, ((float)lastTimeSpan.Hours).MapFloat(0, 59, 0, 1));
            SetArrowPosition(arrowSeconds.rectTransform, ((float)lastTimeSpan.Hours).MapFloat(0, 59, 0, 1));

            _isStop = false;
            if (_coroutine != null)
            {
                return;
            }

            _coroutine = StartCoroutine(RunTimeCoroutine());
        }

        private void OnDisable()
        {
            // 关闭时 保存当前时间
            var now = DateTime.Now;
            LastLoadingTimeSpan = new TimeSpan(now.Hour, now.Minute, now.Second);

            _isStop = true;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }


        private IEnumerator RunTimeCoroutine()
        {
            while (!_isStop)
            {
                yield return CreateArrowPositionSequence(DateTime.Now).WaitForCompletion();
                yield return new WaitForSeconds(NextMoveSeconds);
            }
        }

        public void Stop() => OnDisable();


        private Sequence CreateArrowPositionSequence(DateTime now)
        {
            return CreateArrowPositionSequence(now.Hour, now.Minute, now.Second);
        }

        private Sequence CreateArrowPositionSequence(TimeSpan timeSpan)
        {
            return CreateArrowPositionSequence(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        private Sequence CreateArrowPositionSequence(int hour, int minute, int second)
        {
            var sequence = DOTween.Sequence();

            // 小时
            var hourTarget = (hour > 12 ? hour - 12f : hour).MapFloat(0, 12, 0, 1);
            var hourDuration = Mathf.Lerp(MainArrowMoveDuration, MaxArrowMoveDuration, hourTarget);

            sequence.Join(
                DOTween.To(() => _arrowHourT, t =>
                    {
                        _arrowHourT = t;

                        var eulerAngles = arrowHour.rectTransform.eulerAngles;
                        arrowHour.rectTransform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, -Mathf.Lerp(0, 360, t));
                    }, hourTarget, hourDuration)
                    .SetEase(ArrowHourEase)
            );

            // 分钟
            var minuteTarget = ((float)minute).MapFloat(0, 59, 0, 1);
            var minuteDuration = Mathf.Lerp(MainArrowMoveDuration, MaxArrowMoveDuration, minuteTarget);

            sequence.Join(
                DOTween.To(() => _arrowMinuteT, t =>
                    {
                        _arrowMinuteT = t;

                        var eulerAngles = arrowMinute.rectTransform.eulerAngles;
                        arrowMinute.rectTransform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, -Mathf.Lerp(0, 360, t));
                    }, minuteTarget, minuteDuration)
                    .SetEase(ArrowMinuteEase)
            );

            // 秒
            var secondTarget = ((float)second).MapFloat(0, 59, 0, 1);
            var secondDuration = Mathf.Lerp(MainArrowMoveDuration, MaxArrowMoveDuration, secondTarget);

            sequence.Join(
                DOTween.To(() => _arrowSecondsT, t =>
                    {
                        _arrowSecondsT = t;

                        var eulerAngles = arrowSeconds.rectTransform.eulerAngles;
                        arrowSeconds.rectTransform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, -Mathf.Lerp(0, 360, t));
                    }, secondTarget, secondDuration)
                    .SetEase(ArrowSecondsEase)
            );

            return sequence;
        }

        private static void SetArrowPosition(Transform arrow, float timeClamp01)
        {
            var eulerAngles = arrow.eulerAngles;
            var t = Mathf.Lerp(0, -360, timeClamp01);
            arrow.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, t);
        }
    }
}