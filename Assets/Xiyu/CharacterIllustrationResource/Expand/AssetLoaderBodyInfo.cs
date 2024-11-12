using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Xiyu.CharacterIllustrationResource.Expand
{
    public static class AssetLoaderBodyInfo
    {
        /// <summary>
        /// 异步加载指定类型的资源
        /// </summary>
        /// <param name="assetLoader">资源路径</param>
        /// <param name="dataItem">资源路径</param>
        /// <returns></returns>
        public static async UniTask<Sprite> LoadAssetAsync(this AssetLoader<Sprite> assetLoader, DataItem dataItem)
        {
            return await assetLoader.LoadAssetAsync(dataItem.Path);
        }

        /// <summary>
        /// 异步加载指定类型的资源组 (并发加载)
        /// </summary>
        /// <param name="assetLoader">资源路径</param>
        /// <param name="dataItems">资源路径组</param>
        /// <returns></returns>
        public static async UniTask<IEnumerable<Sprite>> LoadAssetsAsync(this AssetLoader<Sprite> assetLoader, IEnumerable<DataItem> dataItems)
        {
            var assetTasks = new List<UniTask<Sprite>>(dataItems.Select(assetLoader.LoadAssetAsync));

            return await UniTask.WhenAll(assetTasks);
        }

        /// <summary>
        /// 异步加载指定类型的资源组 (并发加载)
        /// </summary>
        /// <param name="assetLoader">资源路径</param>
        /// <param name="bodyInfo">资源信息</param>
        /// <returns></returns>
        public static async UniTask<IEnumerable<Sprite>> LoadAssetAsync(this AssetLoader<Sprite> assetLoader, BodyInfo bodyInfo)
        {
            return await assetLoader.LoadAssetsAsync(bodyInfo.Data);
        }
    }
}