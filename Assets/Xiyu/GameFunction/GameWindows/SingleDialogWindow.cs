using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Xiyu.Constant;
using Xiyu.Expand;
using Xiyu.GameFunction.GeometricTransformations;
using Xiyu.LoggerSystem;

namespace Xiyu.GameFunction.GameWindows
{
    public class SingleDialogWindow : DialogWindow<float>
    {
        [SerializeField] private TextMeshProUGUI contentUGUI;
        [SerializeField] private Button okButton;

        private Property<float> _propertyAlpha;


        protected override void Init(bool autoClose)
        {
            base.Init(autoClose);
            Init();
        }

        protected override void Init(UnityAction<object> autoCloseAction)
        {
            base.Init(autoCloseAction);
            Init();
        }

        private void Init()
        {
            var buttonImage = okButton.GetComponent<Image>();
            var buttonText = okButton.GetComponentInChildren<TextMeshProUGUI>();
            _propertyAlpha = new Property<float>(() => basePanel.color.a, alpha =>
            {
                basePanel.color = basePanel.color.SetAlpha(alpha);
                buttonImage.color = buttonImage.color.SetAlpha(alpha);

                contentUGUI.alpha = alpha;
                titleUGUI.alpha = alpha;
                buttonText.alpha = alpha;
            });

            okButton.onClick.AddListener(() => SelectCompleteHandler?.Invoke(_timer));

            OnGetWindowHandler += () =>
            {
                _timer = 0F;
                _isTiming = true;
                _propertyAlpha.Member = 0;
                basePanel.gameObject.SetActive(true);
                UpDateUIContent(new SingleWindowParams(string.Empty, string.Empty));
            };

            OnReleaseWindowHandler += () =>
            {
                _propertyAlpha.Member = 1;
                basePanel.gameObject.SetActive(false);
            };
        }


        private bool _isTiming;
        private float _timer;

        private void Update()
        {
            if (_isTiming)
                _timer += Time.deltaTime;
        }


        protected override void UpDateUIContent(IDialogWindowParameters dialogWindowParameters)
        {
            var param = GetParams<SingleWindowParams>(dialogWindowParameters);
            titleUGUI.text = param.GetTitle();
            contentUGUI.text = param.Content;
        }


        public override Tween DoShow(float duration, Ease ease, Action onComplete = null)
        {
            return DOTween.To(() => _propertyAlpha.Member, value => _propertyAlpha.Member = value, 1, duration)
                .SetEase(ease)
                .OnComplete(() => onComplete?.Invoke());
        }

        public override Tween DoHide(float duration, Ease ease, Action onComplete = null)
        {
            return DOTween.To(() => _propertyAlpha.Member, value => _propertyAlpha.Member = value, 0, duration)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    _isTiming = false;
                    onComplete?.Invoke();
                });
        }


        public static string GetTypeName() => GameConstant.SingleDialogWindow;


        /// <summary>
        /// [自动回收] 弹出一个选择框，在窗口完成显示后选择任意按钮后隐藏 （可等待至窗口隐藏）
        /// </summary>
        /// <param name="result">选择结果</param>
        /// <param name="dialogWindowParameters">窗口参数</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static IEnumerator GetWindowWaitForClick(UnityAction<float> result, IDialogWindowParameters dialogWindowParameters, Action onComplete = null)
        {
            var window = (SingleDialogWindow)GetWindow(GetTypeName(), autoClose: true);
            // window.Init(autoClose: true);

            yield return window.DisplayWindow(result, dialogWindowParameters, onComplete).WaitForCompletion();
            while (window._isTiming /* && window.basePanel.gameObject.activeSelf == false*/)
            {
                // wait for one Frame
                yield return null;
            }
            // ReleaseWindow(window);
        }

        /// <summary>
        /// 弹出一个选择框，在窗口完成显示时 等待<see cref="closeFunc"/>成立然后隐藏 （可等待至窗口隐藏）
        /// </summary>
        /// <param name="result">选择结果</param>
        /// <param name="closeFunc">窗口关闭条件</param>
        /// <param name="dialogWindowParameters">窗口参数</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static IEnumerator GetWindowWaitForClick(UnityAction<float> result, Func<bool> closeFunc, IDialogWindowParameters dialogWindowParameters, Action onComplete = null)
        {
            var window = (SingleDialogWindow)GetWindow(GetTypeName(), autoClose: false);
            // window.Init(autoClose: false);

            yield return window.DisplayWindow(result, dialogWindowParameters, onComplete).WaitForCompletion();
            while (closeFunc.Invoke() == false)
            {
                yield return null;
            }

            yield return window.DoHide(window.HideTweenParams.duration, window.HideTweenParams.ease, onComplete).WaitForCompletion();
            window.Dispose();
        }
    }
}