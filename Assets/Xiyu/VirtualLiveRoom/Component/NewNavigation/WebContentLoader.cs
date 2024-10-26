using System;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom.Component.NewNavigation
{
    [Serializable]
    public class WebContentLoader
    {
        [SerializeField] private string url;
        [SerializeField] private UnityEngine.AddressableAssets.AssetReferenceGameObject preformViewContent;

        public string Url => url;
        public UnityEngine.AddressableAssets.AssetReferenceGameObject AssetReferenceGameObject => preformViewContent;


        #region MyRegion

        // public GameObject Asset { get; private set; }
        //
        // public void Release()
        // {
        //     if (!preformViewContent.IsDone)
        //     {
        //         return;
        //     }
        //
        //     UnityEngine.AddressableAssets.Addressables.Release(preformViewContent.OperationHandle);
        // }
        //
        //
        // /// <summary>
        // /// 加载实例化对象并且获取指定组件
        // /// </summary>
        // /// <param name="parent"></param>
        // /// <param name="onComplete"></param>
        // /// <typeparam name="T"></typeparam>
        // /// <returns></returns>
        // /// <exception cref="NullReferenceException"></exception>
        // public IEnumerator LoadAndComponentAsync<T>(Transform parent, Action<T> onComplete = null) where T : UnityEngine.Component
        // {
        //     var handle = preformViewContent.LoadAssetAsync();
        //     yield return handle;
        //
        //     if (handle.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         Asset = handle.Result;
        //         var asset = parent == null ? UnityEngine.Object.Instantiate(handle.Result) : UnityEngine.Object.Instantiate(handle.Result, parent: parent);
        //
        //         if (!asset.TryGetComponent(typeof(T), out var component))
        //         {
        //             UnityEngine.Object.Destroy(asset);
        //         }
        //
        //         onComplete?.Invoke((T)component);
        //     }
        //     else
        //     {
        //         throw new NullReferenceException();
        //     }
        // }
        //
        // /// <summary>
        // /// 加载预制体对象
        // /// </summary>
        // /// <param name="onComplete"></param>
        // /// <returns></returns>
        // /// <exception cref="NullReferenceException"></exception>
        // public IEnumerator LoadAsync(Action<GameObject> onComplete = null)
        // {
        //     var handle = preformViewContent.LoadAssetAsync();
        //     yield return handle;
        //
        //     if (handle.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         onComplete?.Invoke(Asset = handle.Result);
        //     }
        //     else
        //     {
        //         throw new NullReferenceException();
        //     }
        // }
        //
        // /// <summary>
        // /// 加载或创建预制体对象 （取决于<see cref="parent"/>是否为空）
        // /// </summary>
        // /// <param name="parent">指定实例化预制体的父级，如果为空则只加载预制体（不实例化）</param>
        // /// <param name="onComplete"></param>
        // /// <returns></returns>
        // /// <exception cref="NullReferenceException"></exception>
        // public IEnumerator LoadAsync(Transform parent, Action<GameObject> onComplete = null)
        // {
        //     var handle = preformViewContent.LoadAssetAsync();
        //     yield return handle;
        //
        //     if (handle.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         Asset = handle.Result;
        //         var asset = parent == null ? handle.Result : UnityEngine.Object.Instantiate(handle.Result, parent);
        //         onComplete?.Invoke(asset);
        //     }
        //     else
        //     {
        //         throw new NullReferenceException();
        //     }
        // }

        #endregion
    }
}