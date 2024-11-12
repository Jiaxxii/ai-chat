using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Xiyu.CharacterIllustrationResource.Expand
{
    public static class AssetLoaderAudioClip
    {
        /// <summary>
        /// 异步加载指定类型的资源
        /// </summary>
        /// <param name="assetLoader">资源路径</param>
        /// <param name="audioClipName">资源路径</param>
        /// <returns></returns>
        public static async UniTask<AudioClip> LoadAssetAsync(this AssetLoader<AudioClip> assetLoader, string audioClipName)
        {
            return await assetLoader.LoadAssetAsync(audioClipName);
        }
    }
}