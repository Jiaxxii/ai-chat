using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Xiyu.GameFunction.GameWindows
{
    public abstract class DialogWindowBase : MonoBehaviour
    {
        public virtual Transform Parent { get; protected set; }

        [SerializeField] protected Image basePanel;
        [SerializeField] protected TextMeshProUGUI titleUGUI;

        public virtual (float duration, Ease ease) ShowTweenParams { get; set; } = (0.45F, Ease.Linear);
        public virtual (float duration, Ease ease) HideTweenParams { get; set; } = (0.45F, Ease.Linear);

        // public DialogWindowState WindowState { get; protected set; }

        private static readonly Stack<DialogWindowBase> ShowWindowStack = new();

        protected abstract void Init(bool autoClose);
        protected abstract void Init(UnityAction<object> autoCloseAction);

        protected static void PopWindow(DialogWindowBase dialogWindow)
        {
            ShowWindowStack.Push(dialogWindow);
            NotClickMaskPanel.Instance.SetAsChildAndLast(dialogWindow.transform);
        }

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

        public abstract Tween DisplayWindow(UnityAction<object> result, IDialogParameters dialogParameters);

        protected virtual void UpDateUIContent(IDialogParameters dialogParameters)
        {
            titleUGUI.text = dialogParameters.Title;
        }


        protected abstract Tween DoShow(float duration, Ease ease);

        protected Tween DoShow()
        {
            var showTweenParams = ShowTweenParams;
            return DoShow(showTweenParams.duration, showTweenParams.ease);
        }


        protected abstract Tween DoHide(float duration, Ease ease);

        protected Tween DoHide()
        {
            var hideTweenParams = HideTweenParams;
            return DoHide(hideTweenParams.duration, hideTweenParams.ease);
        }


        protected static TParams GetParams<TParams>(IDialogParameters dialogParameters) where TParams : class, IDialogParameters
        {
            return dialogParameters as TParams;
        }

        protected static bool TryGetParams<TParams>(IDialogParameters dialogParameters, out TParams instance) where TParams : class, IDialogParameters
        {
            if (dialogParameters is TParams parameters)
            {
                instance = parameters;
                return true;
            }

            instance = null;
            return false;
        }

        public static DialogWindowBase GetWindow<TResult>(string typeName, Transform parent)
        {
            return DialogWindow<TResult>.GetWindow(typeName, parent);
        }
    }
}