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
    public class InputDialogWindow : DialogWindow<string>
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI copyTitle;
        [SerializeField] private TextMeshProUGUI copyPanel;

        [SerializeField] private Button copyToButton;
        [SerializeField] private Button okButton;


        private Property<float> _alphaProperty;


        private static string _copyContent;

        public bool IsSuccess { get; private set; }


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
            var inputFieldPanel = inputField.GetComponent<Image>();
            var okButtonPanel = okButton.GetComponent<Image>();
            var copyToButtonPanel = copyToButton.GetComponent<Image>();

            var inputFieldPlaceholderTextComponent = inputField.placeholder.GetComponent<TextMeshProUGUI>();
            var okButtonTextComponentPanel = okButton.GetComponentInChildren<TextMeshProUGUI>();
            var copyToButtonTextComponentPanel = copyToButton.GetComponentInChildren<TextMeshProUGUI>();

            _alphaProperty = new Property<float>(() => basePanel.color.a, v =>
            {
                // v[0-1]
                basePanel.color = basePanel.color.SetAlpha(v);
                inputFieldPanel.color = inputFieldPanel.color.SetAlpha(v);
                okButtonPanel.color = okButtonPanel.color.SetAlpha(v);
                copyToButtonPanel.color = copyToButtonPanel.color.SetAlpha(v);

                titleUGUI.alpha = v;
                inputField.textComponent.alpha = v;
                inputFieldPlaceholderTextComponent.alpha = v;
                copyTitle.alpha = v;
                copyPanel.alpha = v;
                okButtonTextComponentPanel.alpha = v;
                copyToButtonTextComponentPanel.alpha = v;
            });


            okButton.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(inputField.text))
                {
                    return;
                }

                SelectCompleteHandler?.Invoke(inputField.text);
            });

            copyToButton.onClick.AddListener(() =>
            {
                {
                    var content = GUIUtility.systemCopyBuffer;
                    if (string.IsNullOrEmpty(content))
                    {
                        return;
                    }

                    copyPanel.text = inputField.text = _copyContent = GUIUtility.systemCopyBuffer;
                }
            });


            OnGetWindowHandler += () =>
            {
                inputField.text = string.Empty;
                copyPanel.text = _copyContent = GUIUtility.systemCopyBuffer;
                IsSuccess = false;
                _timer = 0F;
                UpDateUIContent(new InputWindowParams(string.Empty) /* or new DialogWindowParameters(string.Empty) */);

                _alphaProperty.Member = 0;
                basePanel.gameObject.SetActive(true);
            };

            OnReleaseWindowHandler += () =>
            {
                _alphaProperty.Member = 1;
                basePanel.gameObject.SetActive(false);
            };

            copyPanel.text = _copyContent = GUIUtility.systemCopyBuffer;
        }

        private float _timer;

        private void Update()
        {
            if (_timer >= GameConstant.ClipboardAccessHeartbeatSeconds && GUIUtility.systemCopyBuffer != _copyContent)
            {
                copyPanel.text = _copyContent = GUIUtility.systemCopyBuffer;
                _timer = 0F;
            }
            else _timer += Time.deltaTime;
        }


        protected override void UpDateUIContent(IDialogWindowParameters dialogWindowParameters)
        {
            base.UpDateUIContent(dialogWindowParameters);
            // TODO
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = dialogWindowParameters.Title;
        }


        public override Tween DoShow(float duration, Ease ease, Action onComplete = null)
        {
            // WindowState = DialogWindowState.DisplayShow;
            _alphaProperty.SetValue(0);
            basePanel.gameObject.SetActive(true);

            return DOTween.To(() => _alphaProperty.GetValue(), _alphaProperty.SetValue, 1, duration)
                .SetEase(ease);
        }

        public override Tween DoHide(float duration, Ease ease, Action onComplete = null)
        {
            // WindowState = DialogWindowState.DisplayHide;
            _alphaProperty.SetValue(1);
            basePanel.gameObject.SetActive(true);

            return DOTween.To(() => _alphaProperty.GetValue(), _alphaProperty.SetValue, 0, duration)
                .SetEase(ease).OnComplete(() =>
                {
                    IsSuccess = true;
                    onComplete?.Invoke();
                });
        }


        public static string GetTypeName() => GameConstant.InputDialogWindow;


        /// <summary>
        /// [自动回收] 弹出 文本输入窗口，在显示窗口后提交文本时隐藏窗口 （可等待至窗口隐藏）
        /// </summary>
        /// <param name="result">选择结果</param>
        /// <param name="dialogWindowParameters">窗口参数</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static IEnumerator GetWindowWaitForSubmit(UnityAction<string> result, IDialogWindowParameters dialogWindowParameters, Action onComplete = null)
        {
            var window = (InputDialogWindow)GetWindow(GetTypeName(), autoClose: true);
            // window.Init(autoClose: true);

            yield return window.DisplayWindow(result, dialogWindowParameters, onComplete).WaitForCompletion();
            while (window.IsSuccess == false)
            {
                yield return null;
            }
        }

        /// <summary>
        /// 弹出 文本输入窗口，在显示窗口后 等待<see cref="closeFunc"/>成立然后隐藏窗口 （可等待至窗口隐藏）
        /// </summary>
        /// <param name="result">选择结果</param>
        /// <param name="closeFunc"></param>
        /// <param name="dialogWindowParameters">窗口参数</param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static IEnumerator GetWindowWaitForSubmit(UnityAction<string> result, Func<bool> closeFunc, IDialogWindowParameters dialogWindowParameters, Action onComplete = null)
        {
            var window = (InputDialogWindow)GetWindow(GetTypeName(), autoClose: false);
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