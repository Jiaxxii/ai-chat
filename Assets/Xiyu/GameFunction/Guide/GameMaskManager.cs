using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Xiyu.Expand;
using Xiyu.Expand.Singleton;
using Xiyu.GameFunction.GeometricTransformations;
using Xiyu.GameFunction.UI;

namespace Xiyu.GameFunction.Guide
{
    public class GameMaskManager : TimelySingletonMono<GameMaskManager>
    {
        [SerializeField] private Image basePanel;

        [SerializeField] private Arrow arrow;


        private Property<float> _property;

        public Arrow Arrow => arrow;

        public Color MaskColor
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }

        protected override void Awake()
        {
            base.Awake();

            _property = new Property<float>(() => basePanel.color.a, alpha => { basePanel.color = basePanel.color.SetAlpha(alpha); });

            // if (basePanel.gameObject.activeSelf)
            // {
            //     WaitForMaskAlphaCompletion(0.3F, false, onComplete: () => basePanel.gameObject.SetActive(false));
            // }
        }

        public YieldInstruction WaitForMaskAlphaCompletion(float duration, bool isVisible, UnityAction onComplete = null, Ease ease = Ease.Linear)
        {
            (float Start, float End) mode = isVisible ? (0, 1) : (1, 0);

            _property.Member = mode.Start;
            basePanel.gameObject.SetActive(true);

            return DOTween.To(() => _property.Member, v => _property.Member = v, duration, mode.End)
                .SetEase(ease)
                .OnComplete(() => onComplete?.Invoke())
                .WaitForCompletion();
        }
    }
}