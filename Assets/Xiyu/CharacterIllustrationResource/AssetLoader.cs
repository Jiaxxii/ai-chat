using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Xiyu.Expand;


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
        private readonly Dictionary<string, AssetBuffer> _operationHandles = new();

        private int _clearTriggerCount = 50;

        public int ClearTriggerCount
        {
            get => _clearTriggerCount;
            set => _clearTriggerCount = UnityEngine.Mathf.Clamp(value, LowClearTriggerNumber + 1, int.MaxValue);
        }

        /// <summary>
        /// 调用获取资源的次数
        /// </summary>
        public int TotalVisits { get; private set; }

        private int _lowClearTriggerNumber = 10;

        /// <summary>
        /// 在加载资源时进行检查，如果资源项调用次数小于该值者会被释放 (范围：[1-1024] = 10)
        /// </summary>
        public int LowClearTriggerNumber
        {
            get => _lowClearTriggerNumber;
            set => _lowClearTriggerNumber = UnityEngine.Mathf.Clamp(value, 1, 1024);
        }


        public AssetLoader(IEnumerable<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> resourceLocations)
        {
            foreach (var resourceLocation in resourceLocations)
            {
                _pathMapToResourceLocation.Add(resourceLocation.PrimaryKey, resourceLocation);
            }
        }


        /// <summary>
        /// 异步加载指定类型的资源
        /// </summary>
        /// <param name="dataItem">资源路径</param>
        /// <returns></returns>
        public async UniTask<T> LoadAssetAsync(DataItem dataItem)
        {
            return await LoadAssetAsync(dataItem.Path);
        }

        /// <summary>
        /// 异步加载指定类型的资源组 (并发加载)
        /// </summary>
        /// <param name="dataItems">资源路径组</param>
        /// <returns></returns>
        public async UniTask<IEnumerable<T>> LoadAssetsAsync(IEnumerable<DataItem> dataItems)
        {
            var assetTasks = new List<UniTask<T>>(dataItems.Select(LoadAssetAsync));

            return await UniTask.WhenAll(assetTasks);
        }

        /// <summary>
        /// 异步加载指定类型的资源组 (并发加载)
        /// </summary>
        /// <param name="bodyInfo">资源信息</param>
        /// <returns></returns>
        public async UniTask<IEnumerable<T>> LoadAssetAsync(BodyInfo bodyInfo)
        {
            return await LoadAssetsAsync(bodyInfo.Data);
        }

        /// <summary>
        /// 释放加载的资源 （会删除引用的资源）
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="resetTotalVisits">是否将 <see cref="TotalVisits"/> 重置为0</param>
        /// <returns>如果资源不存在则返回false</returns>
        public bool Release(string path, bool resetTotalVisits = true)
        {
            if (!_operationHandles.Remove(path, out var handle))
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


        private async UniTask<T> LoadAssetAsync(string path)
        {
            // 判断是有缓存
            // 不会出现资源句柄还在但资源未加载的情况，因为在创建句柄就加载了资源
            if (TryGetBufferPoolAsset(path, out var asset))
            {
                Debug.Log("缓存资源");
                return asset;
            }

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
            _operationHandles.Add(path, assetBuffer);

            Debug.Log("事实加载");
            return assetBuffer.Handle.Result;
        }

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


            CheckClearBufferPool(key);


            asset = buffer.Handle.Result;
            return true;
        }

        private void CheckClearBufferPool(string ignorePath)
        {
            if (ClearTriggerCount < TotalVisits - _operationHandles.First(p => p.Key == ignorePath).Value.GetCount)
            {
                return;
            }

            // if(System.GC.GetTotalMemory(false) >= 1024 * 1024)
            foreach (var key in _operationHandles
                         .Where(v => v.Key != ignorePath && v.Value.GetCount <= LowClearTriggerNumber)
                         .Select(assetBuffer => assetBuffer.Key)
                         .ToArray())
            {
                Release(key, false);
            }

            TotalVisits = 0;
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

#region MyRegion

/*using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;


namespace Xiyu.CharacterIllustrationResource
{
    public class AssetLoader<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// 标识资源的地址，当该字段被初始化后不会额外添加或者减少
        /// </summary>
        private readonly Dictionary<string, UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> _pathMapToResourceLocation = new();

        /// <summary>
        /// 存储资源加载句柄，用于释放资源
        /// </summary>
        private readonly Dictionary<string, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<T>> _operationHandles = new();
        private readonly Dictionary<string, AssetBuffer> _bufferMappingTable = new();

        public int TotalVisits { get; private set; }

        private int _lowClearTriggerNumber = 10;

        public int LowClearTriggerNumber
        {
            get => _lowClearTriggerNumber;
            set => _lowClearTriggerNumber = UnityEngine.Mathf.Clamp(value, 1, 1024);
        }

        public AssetLoader(IEnumerable<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> resourceLocations)
        {
            foreach (var resourceLocation in resourceLocations)
            {
                _pathMapToResourceLocation.Add(resourceLocation.PrimaryKey, resourceLocation);
            }
        }


        public async UniTask<T> LoadAssetAsync(DataItem dataItem)
        {
            return await LoadAssetAsync(dataItem.Path);
        }

        public async UniTask<IEnumerable<T>> LoadAssetsAsync(IEnumerable<DataItem> dataItems)
        {
            var assetTasks = new List<UniTask<T>>(dataItems.Select(LoadAssetAsync));

            return await UniTask.WhenAll(assetTasks);
        }

        public async UniTask<IEnumerable<T>> LoadAssetAsync(BodyInfo bodyInfo)
        {
            return await LoadAssetsAsync(bodyInfo.Data);
        }

        private async UniTask<T> LoadAssetAsync(string path)
        {
            // 判断是否在缓存池
            if (TryGetBufferPoolAsset(path, out var asset))
            {
                return asset;
            }

            // 不在缓存池就进入加载流程
            // 先判断是否有这个资源的引用
            if (!_pathMapToResourceLocation.TryGetValue(path, out var resourceLocation))
            {
                throw new ResourceLoadFailedException($"加载的资源句柄不包含\"{path}\"！");
            }

            // 判断是否持有资源句柄，如果没有就通过'resourceLocation'地址 获取资源句柄
            if (!_operationHandles.TryGetValue(path, out var handle))
            {
                handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(resourceLocation);

                await handle;
                _operationHandles.Add(path, handle);


                if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    throw new ResourceLoadFailedException("资源加载失败！");
                }
            }


            var assetBuffer = new AssetBuffer(handle.Result);
            _bufferMappingTable.Add(path, assetBuffer);


            return assetBuffer.Asset;
        }

        private bool TryGetBufferPoolAsset(string key, out T asset)
        {
            // 判断是否在缓存池
            if (_bufferMappingTable.TryGetValue(key, out var buffer))
            {
                TotalVisits++;
                buffer.GetCount++;

                CheckClearBufferPool();

                asset = buffer.Asset;
                return true;
            }

            asset = null;
            return false;
        }

        private void CheckClearBufferPool()
        {
            // if(System.GC.GetTotalMemory(false) >= 1024 * 1024)
            foreach (var key in _bufferMappingTable
                         .Where(v => v.Value.GetCount <= LowClearTriggerNumber)
                         .Select(assetBuffer => assetBuffer.Key).ToArray())
            {
                Release(key, false);
            }

            TotalVisits = 0;
        }


        public bool Release(string path, bool resetTotalVisits = true)
        {
            if (!_operationHandles.Remove(path, out var handle))
            {
                return false;
            }

            if (resetTotalVisits)
            {
                TotalVisits = 0;
            }

            UnityEngine.AddressableAssets.Addressables.Release(handle);
            _bufferMappingTable.Remove(path);
            return true;
        }

        public void ReleaseAll()
        {
            foreach (var path in _operationHandles.Select(v => v.Key).ToArray())
            {
                Release(path, false);
            }

            TotalVisits = 0;
        }

        private struct AssetBuffer
        {
            public AssetBuffer(T asset)
            {
                Asset = asset;
                GetCount = 1;
            }

            public T Asset { get; }
            public int GetCount { get; set; }
        }
    }
}*/

#endregion