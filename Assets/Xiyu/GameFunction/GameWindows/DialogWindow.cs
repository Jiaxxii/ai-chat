using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Xiyu.GameFunction.GameWindows
{
    public abstract class DialogWindow<TResult> : DialogWindowBase
    {
        
        private static ObjectPool<DialogWindow<TResult>> _objectPool;


        private readonly Queue<UnityAction<TResult>> _waitForResultDelegateQueue = new();


        protected UnityAction<TResult> SelectCompleteHandler;

        public event UnityAction<TResult> SelectComplete
        {
            add => SelectCompleteHandler += value;
            remove => SelectCompleteHandler -= value;
        }

        protected event UnityAction OnGetWindowHandler;
        protected event UnityAction OnReleaseWindowHandler;


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

            SelectCompleteHandler += autoCloseAction as UnityAction<TResult>;
        }

        private void Init()
        {
            // WindowState = DialogWindowState.InGet;
            SelectCompleteHandler += result =>
            {
                foreach (var unityAction in _waitForResultDelegateQueue)
                {
                    unityAction?.Invoke(result);
                }


                while (_waitForResultDelegateQueue.Count != 0)
                {
                    SelectCompleteHandler -= _waitForResultDelegateQueue.Dequeue();
                }
            };

            SelectCompleteHandler += _ => RecoveryWindow(this);

            Parent = DialogWindowManager.Instance.Parent;
        }


        protected virtual void AutoClose()
        {
            var hideTweenParams = HideTweenParams;
            DoHide(hideTweenParams.duration, hideTweenParams.ease, this.Dispose);
        }


        public Tween DisplayWindow(UnityAction<TResult> result, IDialogParameters dialogParameters, Action onComplete = null)
        {
            _waitForResultDelegateQueue.Enqueue(result);
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
            return DoShow(showTweenParams.duration, showTweenParams.ease, onComplete);
        }

        public sealed override Tween DisplayWindow(UnityAction<object> result, IDialogParameters dialogParameters, Action onComplete = null)
        {
            if (result.Target is not TResult)
            {
                throw new InvalidConnectionException($"无法将{typeof(object).FullName}转换为{typeof(TResult).FullName}");
            }

            return DisplayWindow(result as UnityAction<TResult>, dialogParameters, onComplete);
        }

        public static DialogWindow<TResult> GetWindow(string typeName, bool autoClose, Transform parent = null)
        {
            _objectPool ??= new ObjectPool<DialogWindow<TResult>>(() => CreateFunc(typeName, autoClose, parent), GetWindow, ReleaseWindow);

            DialogWindow<TResult> window = _objectPool.Get();

            return window;
        }

        public static DialogWindow<TResult> GetWindow(string typeName, UnityAction<object> autoCloseAction, Transform parent = null)
        {
            _objectPool ??= new ObjectPool<DialogWindow<TResult>>(() => CreateFunc(typeName, autoCloseAction, parent), GetWindow, ReleaseWindow);

            DialogWindow<TResult> window = _objectPool.Get();

            return window;
        }


        private static DialogWindow<TResult> CreateFunc(string typeName, bool autoClose, Transform parent = null)
        {
            var preform = CharacterComponent.CharacterContentRoot.PreformScriptableObject.Table[typeName].Preform;

            var dialogWindow = Instantiate(preform, parent == null ? DialogWindowManager.Instance.Parent : parent).GetComponent<DialogWindow<TResult>>();

            dialogWindow.Init(autoClose);


            return dialogWindow;
        }

        private static DialogWindow<TResult> CreateFunc(string typeName, UnityAction<object> autoCloseAction, Transform parent = null)
        {
            var preform = CharacterComponent.CharacterContentRoot.PreformScriptableObject.Table[typeName].Preform;

            var dialogWindow = Instantiate(preform, parent == null ? DialogWindowManager.Instance.Parent : parent).GetComponent<DialogWindow<TResult>>();

            dialogWindow.Init(autoCloseAction);


            return dialogWindow;
        }


        private static void GetWindow(DialogWindow<TResult> window)
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (window.OnGetWindowHandler is null)
            {
                window.OnGetWindowHandler += () => window.gameObject.SetActive(true);
            }

            window.OnGetWindowHandler.Invoke();
            PopWindow(window);
        }

        private static void ReleaseWindow(DialogWindow<TResult> window)
        {
            // // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            // if (window.OnReleaseWindowHandler is null)
            // {
            //     window.OnReleaseWindowHandler += () => window.gameObject.SetActive(false);
            // }

            window.OnReleaseWindowHandler?.Invoke();
            RecoveryWindow(window);
        }


        public override void Dispose()
        {
            basePanel.gameObject.SetActive(false);
            _objectPool.Release(this);
            ReleaseWindow(this);
        }
    }
}