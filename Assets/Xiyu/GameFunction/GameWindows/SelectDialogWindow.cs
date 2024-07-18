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

namespace Xiyu.GameFunction.GameWindows
{
    public class SelectDialogWindow : DialogWindow<bool>
    {
        public class Parameters : IDialogParameters
        {
            public Parameters(string title, string content)
            {
                Title = title;
                Content = content;
            }

            public string Title { get; }
            public string Content { get; }

            public (float duration, Ease ease)? ShowTweenParams { get; set; }
            public (float duration, Ease ease)? HideTweenParams { get; set; }
        }


        [SerializeField] private TextMeshProUGUI contentTex;
        [SerializeField] private Button oK, cancel;

        private Property<float> _alphaProperty;

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
            oK.onClick.AddListener(() => SelectCompleteHandler?.Invoke(true));
            cancel.onClick.AddListener(() => SelectCompleteHandler?.Invoke(false));

            var image = basePanel.GetComponent(typeof(Image)) as Image ?? throw new NullReferenceException();

            var okButtonPanel = oK.GetComponent<Image>();
            var cancelButtonPanel = cancel.GetComponent<Image>();

            _alphaProperty = new Property<float>(() => image.color.a, v =>
            {
                // v[0-1]
                image.color = image.color.SetAlpha(v);
                okButtonPanel.color = okButtonPanel.color.SetAlpha(v);
                cancelButtonPanel.color = cancelButtonPanel.color.SetAlpha(v);
                titleUGUI.alpha = v;
                contentTex.alpha = v;
            });

            OnGetWindowHandler += () =>
            {
                IsSelect = false;
                UpDateUIContent(new Parameters(string.Empty, string.Empty));
                ButtonActive(true);

                _alphaProperty.Member = 0;
                basePanel.gameObject.SetActive(true);
            };

            OnReleaseWindowHandler += () =>
            {
                // IsSelect = false;
                _alphaProperty.Member = 1;
                basePanel.gameObject.SetActive(false);
            };
        }
        

        public bool IsSelect { get; private set; }

        protected override void UpDateUIContent(IDialogParameters dialogParameters)
        {
            base.UpDateUIContent(dialogParameters);
            var parameters = GetParams<Parameters>(dialogParameters);
            contentTex.text = parameters.Content;
        }


        protected override Tween DoShow(float duration, Ease ease, Action onComplete = null)
        {
            // WindowState = DialogWindowState.DisplayShow;
            ButtonActive(false);
            _alphaProperty.SetValue(0);
            basePanel.gameObject.SetActive(true);

            return DOTween.To(() => _alphaProperty.GetValue(), _alphaProperty.SetValue, 1, duration)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    ButtonActive(true);
                    onComplete?.Invoke();
                });
        }

        protected override Tween DoHide(float duration, Ease ease, Action onComplete = null)
        {
            // WindowState = DialogWindowState.DisplayHide;
            ButtonActive(false);

            return DOTween.To(() => _alphaProperty.GetValue(), _alphaProperty.SetValue, 0, duration)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    ButtonActive(true);
                    IsSelect = true;
                    onComplete?.Invoke();
                });
        }


        private void ButtonActive(bool value)
        {
            // 关闭按钮的文字
            oK.transform.GetChild(0).gameObject.SetActive(value);
            cancel.transform.GetChild(0).gameObject.SetActive(value);

            // 关闭按钮组件的激活状态 (不然调整 panel 的 alpha 是无效的)
            oK.enabled = value;
            cancel.enabled = value;
        }

        public static string GetTypeName() => GameConstant.SelectDialogWindow;


        /// <summary>
        /// [自动回收] 弹出一个选择框，在窗口完成显示后选择任意按钮后隐藏 （可等待至窗口隐藏）
        /// </summary>
        /// <param name="result">选择结果</param>
        /// <param name="dialogParameters">窗口参数</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static IEnumerator GetWindowWaitForSelect(UnityAction<bool> result, IDialogParameters dialogParameters, Action onComplete = null)
        {
            var window = (SelectDialogWindow)GetWindow(GetTypeName(), autoClose: true);
            // window.Init(autoClose: true);

            yield return window.DisplayWindow(result, dialogParameters, onComplete).WaitForCompletion();
            while (window.IsSelect == false /* && window.basePanel.gameObject.activeSelf == false*/)
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
        /// <param name="dialogParameters">窗口参数</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static IEnumerator GetWindowWaitForSelect(UnityAction<bool> result, Func<bool> closeFunc, IDialogParameters dialogParameters, Action onComplete = null)
        {
            var window = (SelectDialogWindow)GetWindow(GetTypeName(), autoClose: false);
            // window.Init(autoClose: false);

            yield return window.DisplayWindow(result, dialogParameters, onComplete).WaitForCompletion();
            while (closeFunc.Invoke() == false)
            {
                // wait for one Frame
                yield return null;
            }

            yield return window.DoHide(window.HideTweenParams.duration, window.HideTweenParams.ease, onComplete).WaitForCompletion();
            window.Dispose();
        }
    }
}