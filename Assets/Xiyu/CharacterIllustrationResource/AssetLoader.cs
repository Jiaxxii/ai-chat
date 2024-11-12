using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;


namespace Xiyu.CharacterIllustrationResource
{
    public sealed class AssetLoader<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// 标识资源的地址，当该字段被初始化后不会额外添加或者减少
        /// </summary>
        private readonly Dictionary<string, UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> _pathMapToResourceLocation = new();

        /// <summary>
        /// 存储资源加载句柄，用于加载、缓存、释放资源
        /// </summary>
        private readonly ConcurrentDictionary<string, AssetBuffer> _operationHandles = new();


        /// <summary>
        /// 调用获取资源的次数
        /// </summary>
        public int TotalVisits { get; private set; }


        public AssetLoader(IEnumerable<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> resourceLocations)
        {
            foreach (var resourceLocation in resourceLocations)
            {
                _pathMapToResourceLocation.TryAdd(resourceLocation.PrimaryKey, resourceLocation);
            }
        }


        /// <summary>
        /// 释放加载的资源 （会删除引用的资源）
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="resetTotalVisits">是否将 <see cref="TotalVisits"/> 重置为0</param>
        /// <returns>如果资源不存在则返回false</returns>
        public bool Release(string path, bool resetTotalVisits = true)
        {
            if (!_operationHandles.TryRemove(path, out var handle))
            {
                return false;
            }

            if (resetTotalVisits)
            {
                TotalVisits = 0;
            }

            UnityEngine.AddressableAssets.Addressables.Release(handle.Handle);
            return true;
        }

        /// <summary>
        /// 清理使用资源 （会删除引用的资源）
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var handle in _operationHandles.Select(v => v.Value.Handle))
            {
                UnityEngine.AddressableAssets.Addressables.Release(handle);
            }

            _operationHandles.Clear();
            TotalVisits = 0;
        }


        // 如果并发执行 可能出现资源句柄在加载时第二次访问
        public async UniTask<T> LoadAssetAsync(string path)
        {
            // 判断是有缓存
            // // 不会出现资源句柄还在但资源未加载的情况，因为在创建句柄就加载了资源
            // if (TryGetBufferPoolAsset(path, out var asset))
            // {
            //     return asset;
            // }

            var bufferAsset = await TryGetBufferPoolAssetAsync(path);
            if (bufferAsset is not null)
                return bufferAsset;

            // 不在缓存池就进入加载流程
            // 先判断是否有这个资源的引用
            if (!_pathMapToResourceLocation.TryGetValue(path, out var resourceLocation))
            {
                throw new ResourceLoadFailedException($"加载的资源句柄不包含\"{path}\"！");
            }


            // 创建资源加载句柄
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(resourceLocation);

            // 等待资源加载完成
            await handle;

            // 检测状态
            if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                throw new ResourceLoadFailedException("资源加载失败！");
            }

            // 存储起来以便管理资源句柄以及引用次数
            var assetBuffer = new AssetBuffer(handle);
            _operationHandles.TryAdd(path, assetBuffer);

            return assetBuffer.Handle.Result;
        }

        // 线程不安全
        [System.Obsolete]
        private bool TryGetBufferPoolAsset(string key, out T asset)
        {
            // 判断是否包含句柄
            if (!_operationHandles.TryGetValue(key, out var buffer))
            {
                asset = null;
                return false;
            }

            TotalVisits++;
            buffer.GetCount++;

            asset = buffer.Handle.Result;
            return true;
        }

        private async UniTask<T> TryGetBufferPoolAssetAsync(string key)
        {
            // 判断是否包含句柄
            if (!_operationHandles.TryGetValue(key, out var buffer))
            {
                return null;
            }

            TotalVisits++;
            buffer.GetCount++;

            if (buffer.Handle.IsDone)
            {
                return buffer.Handle.Result;
            }

            return await buffer.Handle;
        }


        private struct AssetBuffer
        {
            public AssetBuffer(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<T> handle)
            {
                Handle = handle;
                GetCount = 1;
            }

            internal UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<T> Handle { get; }
            internal int GetCount { get; set; }
        }
    }
}