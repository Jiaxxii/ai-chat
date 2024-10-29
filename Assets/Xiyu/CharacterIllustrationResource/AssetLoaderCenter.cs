using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Xiyu.CharacterIllustrationResource
{
    public static class AssetLoaderCenter<T> where T : UnityEngine.Object
    {
        private static readonly Dictionary<string, AssetLoader<T>> Asset = new();

        
        public static IReadOnlyDictionary<string, AssetLoader<T>> AssetLoaders => Asset;

        
        public static async UniTask<AssetLoader<T>> LoadResourceLocations(string addressableLabel)
        {
            var handle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync(addressableLabel, typeof(T));
            await handle;

            var assetLoader = new AssetLoader<T>(handle.Result);
            if (!Asset.TryAdd(addressableLabel, assetLoader))
            {
                throw new ResourceLoadFailedException($"\"{addressableLabel}\"资源句柄已经加载！");
            }

            return assetLoader;
        }
    }
}