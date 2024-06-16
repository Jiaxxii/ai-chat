using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Xiyu.CharacterIllustration
{
    public class SpriteAssetLoader
    {
        // 
        public SpriteAssetLoader(string type, IEnumerable<KeyValuePair<string, SpriteAsset>> collection)
        {
            Type = type;
            _typeFindAtlasInitializationTransformationInfoMap = new Dictionary<string, SpriteAsset>(collection);
        }


        private readonly Dictionary<string, SpriteAsset> _typeFindAtlasInitializationTransformationInfoMap;

        private readonly Dictionary<string, SpriteResourceLoader> _codeFindSpriteResourceLoaderMap = new();

        public RefState State { get; private set; }

        public string Type { get; }

        public SpriteAsset GetSpriteAsset(string type = null)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrWhiteSpace(type))
            {
                type = Type;
            }

            return _typeFindAtlasInitializationTransformationInfoMap[type];
        }

        public SpriteResourceLoader GetResourceLoader(string code) => _codeFindSpriteResourceLoaderMap[code];

        /// <summary>
        /// <para>  *必须调用</para>
        /// 异步加载资源类型为 <see cref="Sprite"/> 并且标签为 <see cref="label"/>
        /// <para>(不指定时使用 <see cref="Type"/>) 作为 <see cref="label"/></para>
        /// </summary>
        /// <param name="label">资源标签名称</param>
        /// <returns>此方法需要配合协程使用</returns>
        public IEnumerator LoadRefAssetsAsync(string label = null)
        {
            State = RefState.None;
            if (string.IsNullOrEmpty(label) || string.IsNullOrWhiteSpace(label))
            {
                label = Type;
            }

            var refsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(Sprite));

            State = RefState.Loading;
            yield return refsHandle;

            if (refsHandle.Status != AsyncOperationStatus.Succeeded)
            {
                State = RefState.Fail;
            }

            foreach (var assetRef in refsHandle.Result)
            {
                // assetRef.PrimaryKey 返回图片的源名称
                // (前提需要图片命名遵循本项目的命名规范)
                _codeFindSpriteResourceLoaderMap.Add(assetRef.PrimaryKey, new SpriteResourceLoader(assetRef));
            }

            State = RefState.Ok;
            Addressables.Release(refsHandle);
        }

        /// <summary>
        /// 将引用的资源(图集)加载到内存中
        /// </summary>
        /// <param name="typeCode">图集代码</param>
        /// <param name="onCompletion">在加载完所有图集时</param>
        /// <returns>此方法需要配合协程使用</returns>
        public IEnumerator LoadAssetAsync(string typeCode, Action<Sprite[]> onCompletion)
        {
            if (string.IsNullOrEmpty(typeCode) || string.IsNullOrWhiteSpace(typeCode))
            {
                Debug.LogError("空的\"typeCode\"");
                yield break;
            }

            if (!_typeFindAtlasInitializationTransformationInfoMap.TryGetValue(typeCode, out var spriteAsset))
            {
                Debug.LogError($"不存在的资源名称\"{typeCode}\"");
                yield break;
            }


            if (State == RefState.None)
            {
                Debug.LogWarning("资源引用未加载,尝试加载中......");
                yield return LoadRefAssetsAsync(Type);
            }

            if (State == RefState.Loading)
            {
                Debug.LogWarning("资源引用未加载完成,尝试等待中......");
                yield return new WaitUntil(() => State != RefState.Loading);
            }

            if (State == RefState.Fail)
            {
                Debug.LogError("资源加载失败!");
                yield break;
            }

            var transformInfoData = spriteAsset.Data.TransformInfoData;
            var sprites = new Sprite[transformInfoData.Length];

            // 这里获取的就是这个"body(faces) code"对应的精灵立绘信息
            for (var i = 0; i < sprites.Length; i++)
            {
                var resource = _codeFindSpriteResourceLoaderMap[transformInfoData[i].Path];
                var index = i;
                yield return resource.GetAsync(sprite => sprites[index] = sprite);
            }

            onCompletion?.Invoke(sprites);
        }

        // /// <summary>
        // /// 将引用的资源(图集)加载到内存中
        // /// </summary>
        // /// <param name="typeCode">图集代码</param>
        // /// <param name="onItemCompletion">在加载完成一个图集时</param>
        // /// <returns>此方法需要配合协程使用</returns>
        // public IEnumerator LoadAssetAsync(string typeCode, Action<Sprite> onItemCompletion)
        // {
        //     if (string.IsNullOrEmpty(typeCode) || string.IsNullOrWhiteSpace(typeCode))
        //     {
        //         Debug.LogError("空的\"typeCode\"");
        //         yield break;
        //     }
        //
        //     if (!_typeFindAtlasInitializationTransformationInfoMap.TryGetValue(typeCode, out var spriteAsset))
        //     {
        //         Debug.LogError($"不存在的资源名称\"{typeCode}\"");
        //         yield break;
        //     }
        //
        //     // 这里获取的就是这个"body(faces) code"对应的精灵立绘信息
        //     foreach (var data in spriteAsset.Data.TransformInfoData)
        //     {
        //         var resource = _codeFindSpriteResourceLoaderMap[data.Path];
        //         yield return resource.GetAsync(sprite => onItemCompletion?.Invoke(sprite));
        //     }
        // }
    }
}