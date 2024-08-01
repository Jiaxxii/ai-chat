using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Xiyu.Expand;
using Xiyu.Expand.Singleton;
using Xiyu.GameFunction.GeometricTransformations;

namespace Xiyu.GameFunction.Guide
{
    public class GameMaskManager : TimelySingletonMono<GameMaskManager>
    {
        [SerializeField] private Image basePanel;

        private Property<float> _property;

        public Color MaskColor
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }

        protected override void Awake()
        {
            base.Awake();

            _property = new Property<float>(() => basePanel.color.a, alpha => { basePanel.color = basePanel.color.SetAlpha(alpha); });
        }

        public YieldInstruction WaitForMaskCompletion(float duration, bool isVisible, UnityAction onComplete = null, Ease ease = Ease.Linear)
        {
            (float Start, float End) mode = isVisible ? (0, 1) : (1, 0);

            _property.Member = mode.Start;

            return DOTween.To(() => _property.Member, v => _property.Member = v, duration, mode.End)
                .SetEase(ease)
                .OnComplete(() => onComplete?.Invoke())
                .WaitForCompletion();
        }
    }
}