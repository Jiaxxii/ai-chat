using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Xiyu.GameFunction.GeometricTransformations;

namespace Xiyu
{
    public class AsyncCancellationToken : MonoBehaviour
    {
        [SerializeField] private RectTransform imageRect;

        private CancellationTokenSource _cancellationTokenSource;

        private async void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(6));
            await MoveAsync(new Property<float>(() => imageRect.anchoredPosition.x, v => imageRect.anchoredPosition = new Vector2(v, 0))
                , 0
                , 100
                , _cancellationTokenSource.Token);

            await MoveAsync(new Property<float>(() => imageRect.anchoredPosition.x, v => imageRect.anchoredPosition = new Vector2(v, 0))
                , 0
                , 100
                , _cancellationTokenSource.Token);
        }

        private async UniTask MoveAsync(Property<float> moveObj, float startValue, float endValue, CancellationToken cancellationToken)
        {
            moveObj.Member = startValue;

            var tween = DOTween.To(() => moveObj.Member, v => moveObj.Member = v, endValue, 5);
            await tween.OnUpdate(() =>
            {
                if (!cancellationToken.IsCancellationRequested) return;

                tween.Complete(true);
            }).AsyncWaitForCompletion().AsUniTask();
        }
    }
}