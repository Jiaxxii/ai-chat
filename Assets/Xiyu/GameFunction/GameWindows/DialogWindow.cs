using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Xiyu.GameFunction.GameWindows
{
    public enum DialogWindowState
    {
        InGet,
        InRelease,
        DisplayShow,
        DisplayHide
    }

    public abstract class DialogWindow<TResult> : DialogWindowBase  , IDisposable
    {
        private static ObjectPool<DialogWindow<TResult>> _objectPool;


        /// <summary>
        /// 
        /// </summary>
        protected readonly Queue<UnityAction<TResult>> WaitForResultDelegateQueue = new();


        protected UnityAction<TResult> SelectCompleteHandler;

        public event UnityAction<TResult> SelectComplete
        {
            add => SelectCompleteHandler = value;
            remove => SelectCompleteHandler -= value;
        }


        protected override void Init(bool autoClose)
        {
            Init();
            if (autoClose)
            {
                SelectCompleteHandler += _ => AutoClose();
            }
        }

        protected override void Init(UnityAction<object> autoCloseAction)
        {
            Init();
            if (autoCloseAction.Target is not TResult)
            {
                throw new InvalidConnectionException($"无法将{typeof(object).FullName}转换为{typeof(TResult).FullName}");
            }

            SelectCompleteHandler += new UnityAction<TResult>(autoCloseAction);
        }

        private void Init()
        {
            // WindowState = DialogWindowState.InGet;
            SelectCompleteHandler += result =>
            {
                foreach (var unityAction in WaitForResultDelegateQueue)
                {
                    unityAction?.Invoke(result);
                }


                while (WaitForResultDelegateQueue.Count != 0)
                {
                    SelectCompleteHandler -= WaitForResultDelegateQueue.Dequeue();
                }
            };

            SelectCompleteHandler += _ => RecoveryWindow(this);

            Parent = DialogWindowManager.Instance.Parent;
        }


        protected virtual void AutoClose()
        {
            var hideTweenParams = HideTweenParams;
            DoHide(hideTweenParams.duration, hideTweenParams.ease)
                .OnComplete(() =>
                {
                    basePanel.gameObject.SetActive(false);
                    _objectPool.Release(this);
                });
        }


        public Tween DisplayWindow(UnityAction<TResult> result, IDialogParameters dialogParameters)
        {
            WaitForResultDelegateQueue.Enqueue(result);
            UpDateUIContent(dialogParameters);

            if (dialogParameters.HideTweenParams.HasValue)
            {
                HideTweenParams = dialogParameters.HideTweenParams.Value;
            }

            if (dialogParameters.ShowTweenParams.HasValue)
            {
                ShowTweenParams = dialogParameters.ShowTweenParams.Value;
            }

            var showTweenParams = dialogParameters.ShowTweenParams ?? ShowTweenParams;
            return DoShow(showTweenParams.duration, showTweenParams.ease);
        }

        public sealed override Tween DisplayWindow(UnityAction<object> result, IDialogParameters dialogParameters)
        {
            if (result.Target is not TResult)
            {
                throw new InvalidConnectionException($"无法将{typeof(object).FullName}转换为{typeof(TResult).FullName}");
            }

            return DisplayWindow(result as UnityAction<TResult>, dialogParameters);
        }

        public static DialogWindow<TResult> GetWindow(string typeName, Transform parent = null)
        {
            _objectPool ??= new ObjectPool<DialogWindow<TResult>>(() => CreateFunc(typeName, parent), OnGetWindow, OnReleaseWindow);

            DialogWindow<TResult> window = _objectPool.Get();
            return window;
        }

        private static DialogWindow<TResult> CreateFunc(string typeName, Transform parent = null)
        {
            var preform = CharacterComponent.CharacterContentRoot.PreformScriptableObject.Table[typeName].Preform;

            var dialogWindow = Instantiate(preform, parent == null ? DialogWindowManager.Instance.Parent : parent).GetComponent<DialogWindow<TResult>>();

            dialogWindow.Init();

            // dialogWindow.WindowState = DialogWindowState.InRelease;

            return dialogWindow;
        }


        private static void OnGetWindow(DialogWindow<TResult> window)
        {
            window.gameObject.SetActive(true);
            PopWindow(window);
        }

        protected static void OnReleaseWindow(DialogWindow<TResult> window)
        {
            // TODO 可能重复设置状态
            window.gameObject.SetActive(false);
            RecoveryWindow(window);
        }

        public abstract void Dispose();
    }
}