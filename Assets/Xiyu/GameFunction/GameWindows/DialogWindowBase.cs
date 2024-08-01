using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Xiyu.GameFunction.GameWindows
{
    public abstract class DialogWindowBase : MonoBehaviour, IDisposable
    {
        #region Unity序列化    protected

        [SerializeField] protected Image basePanel;
        [SerializeField] protected TextMeshProUGUI titleUGUI;

        #endregion

        #region 属性

        public virtual Transform Parent { get; protected set; }
        public virtual (float duration, Ease ease) ShowTweenParams { get; set; } = (0.45F, Ease.Linear);
        public virtual (float duration, Ease ease) HideTweenParams { get; set; } = (0.45F, Ease.Linear);

        #endregion

        #region 窗口托管栈   private

        private static readonly Stack<DialogWindowBase> ShowWindowStack = new();

        #endregion

        #region 初始化器    protected

        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoClose">为 true 时，在回收窗口时需要手动调用 <see cref="Dispose"/> 方法</param>
        protected abstract void Init(bool autoClose);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoCloseAction">定义窗口回收规则</param>
        protected abstract void Init(UnityAction<object> autoCloseAction);

        #endregion

        #region 将多个弹窗按栈顺序弹窗/回收  protected

        /// <summary>
        /// 入栈窗口，并且将窗口设置为顶层
        /// </summary>
        /// <param name="dialogWindow"></param>
        protected static void PopWindow(DialogWindowBase dialogWindow)
        {
            ShowWindowStack.Push(dialogWindow);
            NotClickMaskPanel.Instance.SetAsChildAndLast(dialogWindow.transform);
        }

        /// <summary>
        /// 出栈一个窗口
        /// </summary>
        /// <param name="dialogWindow"></param>
        protected static void RecoveryWindow(DialogWindowBase dialogWindow)
        {
            if (ShowWindowStack.Count != 0)
            {
                var window = ShowWindowStack.Pop();
                NotClickMaskPanel.Instance.SetAsChildAndLast(window.transform);
            }
            else
            {
                NotClickMaskPanel.Instance.ClearParent(dialogWindow.transform);
                NotClickMaskPanel.Instance.SetActive(false);
            }
        }

        #endregion

        #region 显示窗口

        public abstract Tween DisplayWindow(UnityAction<object> result, IDialogWindowParameters dialogWindowParameters, Action onComplete = null);

        #endregion

        #region 更新UI    protected

        protected virtual void UpDateUIContent(IDialogWindowParameters dialogWindowParameters)
        {
            titleUGUI.text = dialogWindowParameters.Title;
        }

        #endregion

        #region 淡入窗口    protected

        public abstract Tween DoShow(float duration, Ease ease, Action onComplete = null);

        public Tween DoShow(Action onComplete)
        {
            var showTweenParams = ShowTweenParams;
            return DoShow(showTweenParams.duration, showTweenParams.ease, onComplete);
        }

        #endregion

        #region 淡出窗口    protected

        public abstract Tween DoHide(float duration, Ease ease, Action onComplete = null);

        public Tween DoHide(Action onComplete)
        {
            var hideTweenParams = HideTweenParams;
            return DoHide(hideTweenParams.duration, hideTweenParams.ease, onComplete);
        }

        #endregion
        
        #region 将 IDialogParameters 转换为具体类  protected static

        protected static TParams GetParams<TParams>(IDialogWindowParameters dialogWindowParameters) where TParams : class, IDialogWindowParameters => dialogWindowParameters as TParams;
        protected static bool TryGetParams<TParams>(IDialogWindowParameters dialogWindowParameters, out TParams instance) where TParams : class, IDialogWindowParameters
        {
            if (dialogWindowParameters is TParams parameters)
            {
                instance = parameters;
                return true;
            }

            instance = null;
            return false;
        }

        #endregion

        #region 回收窗口

        public abstract void Dispose();

        #endregion
        
        // public static DialogWindowBase GetWindow<TResult>(string typeName, Transform parent)
        // {
        //     return DialogWindow<TResult>.GetWindow(typeName, parent);
        // }
    }
}