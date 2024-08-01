using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Xiyu.LoggerSystem;

namespace Xiyu.GameFunction.UI
{
    public class StopWatch : MonoBehaviour
    {
        // 长指针 短指针
        [SerializeField] private Image arrowL;
        [SerializeField] private Image arrowS;

        private float _t;

        public (float Start, float End) ArrowLong { get; set; } = (0, -360);
        public (float Start, float End) ArrowShort { get; set; } = (-40, -400);

        public Ease ArrowLongEase { get; set; } = Ease.Unset;
        public Ease ArrowShortEase { get; set; } = Ease.Unset;

        public bool IsLoading { get; private set; }


        private Coroutine _coroutine;

        private void Start()
        {
            ArrowLongEase = Ease.Linear;
            ArrowShortEase = Ease.OutBounce;

            LoadingCoroutine(3, () => Input.GetKeyDown(KeyCode.Space), null);
        }


        public Coroutine LoadingCoroutine(float duration, Func<bool> completeCondition, UnityAction onComplete)
        {
            if (!IsLoading) return _coroutine = StartCoroutine(LoadingCoroutineAsync(duration, completeCondition, onComplete));


            LoggerManager.Instance.LogWarning("loading......");
            return _coroutine;
        }

        private IEnumerator LoadingCoroutineAsync(float duration, Func<bool> completeCondition, UnityAction onComplete)
        {
            var tween = DoRotate(1, duration)
                .SetLoops(-1)
                .SetAutoKill(false);

            IsLoading = true;
            while (true)
            {
                if (completeCondition.Invoke() == true)
                {
                    break;
                }

                yield return null;
            }

            IsLoading = false;

            tween.Kill();
            onComplete?.Invoke();
            _coroutine = null;
        }


        private Sequence DoRotate(float endValue, float duration)
        {
            var sequence = DOTween.Sequence();

            sequence.Join(DOTween.To(() => _t, value =>
                {
                    var anglesLong = arrowL.rectTransform.eulerAngles;
                    arrowL.rectTransform.eulerAngles = new Vector3(anglesLong.x, anglesLong.y, Mathf.Lerp(ArrowLong.Start, ArrowLong.End, value));
                }, endValue, duration)
                .SetEase(ArrowLongEase));


            sequence.Join(DOTween.To(() => _t, value =>
                {
                    _t = value;

                    var anglesShort = arrowS.rectTransform.eulerAngles;
                    arrowS.rectTransform.eulerAngles = new Vector3(anglesShort.x, anglesShort.y, Mathf.Lerp(ArrowShort.Start, ArrowShort.End, value));
                }, endValue, duration)
                .SetEase(ArrowShortEase));

            return sequence;
        }
    }
}