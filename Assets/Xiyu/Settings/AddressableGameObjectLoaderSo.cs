using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Xiyu.VirtualLiveRoom.Component.NewNavigation;

namespace Xiyu.Settings
{
    [CreateAssetMenu(fileName = "New GameObject Loader", menuName = "ScriptableObject/AddressableGameObjectLoader")]
    public class AddressableGameObjectLoaderSo : ScriptableObject
    {
        [SerializeField] private List<AssetGameObjectLoader> viewContentCollection;


        private readonly ConcurrentDictionary<string, AssetReferenceGameObject> _referenceMap = new();
        private readonly ConcurrentDictionary<string, AsyncOperationHandle<GameObject>> _buffer = new();


        private readonly object _lock = new { };

        private void Awake()
        {
            _referenceMap.Clear();
            _buffer.Clear();
            TryLoadReferenceMap();
        }

        private void TryLoadReferenceMap()
        {
            if (viewContentCollection.Count == 0)
            {
                throw new InvalidOperationException("网页内容收集器元素为空！");
            }

            if (viewContentCollection.Count == _referenceMap.Count)
            {
                return;
            }

            var rawSet = new HashSet<string>(viewContentCollection.Select(v => v.Key));
            var instance = new HashSet<string>(_referenceMap.Select(v => v.Key));

            var exceptColl = rawSet.Except(instance).ToArray();

            if (exceptColl.Length == 0)
            {
                return;
            }

            foreach (var key in exceptColl)
            {
                _referenceMap.TryAdd(key, viewContentCollection.Find(v => v.Key == key).AssetReferenceGameObject);
            }
        }


        public async UniTask<GameObject> LoadPrefabricateAssetAsync(string key)
        {
            lock (_lock)
            {
                TryLoadReferenceMap();
            }

            if (!_referenceMap.TryGetValue(key, out var refGameObject))
            {
                throw new ArgumentException($"网址无效！{key}");
            }

            // 有预制体直接返回
            if (!_buffer.TryGetValue(key, out var handle))
            {
                handle = refGameObject.LoadAssetAsync();
            }

            _buffer.TryAdd(key, handle);

            if (!handle.IsDone)
            {
                await handle;
            }


            // 缓存预制体
            return handle.Result;
        }


        public async UniTask<GameObject> LoadGameObjectInstanceAssetAsync(string key, Transform parent)
        {
            return Instantiate(await LoadPrefabricateAssetAsync(key), parent);
        }


        public async UniTask<T> LoadComponentAssetAsync<T>(string key, Transform parent) where T : Component
        {
            return (await LoadGameObjectInstanceAssetAsync(key, parent)).GetComponent(typeof(T)) as T;
        }


        public bool ReleaseAsset(string key)
        {
            if (!_referenceMap.TryGetValue(key, out var refGameObject))
            {
                return false;
            }

            refGameObject.ReleaseAsset();

            if (!_buffer.TryRemove(key, out _))
            {
                return false;
            }

            return true;
        }
    }
}