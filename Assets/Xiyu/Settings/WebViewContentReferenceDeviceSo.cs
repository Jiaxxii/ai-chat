using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Xiyu.VirtualLiveRoom.Component.NewNavigation;
using Object = UnityEngine.Object;

namespace Xiyu.Settings
{
    [CreateAssetMenu(fileName = "New WebPageConfig", menuName = "ScriptableObject/WebPageConfigSo")]
    public class WebViewContentReferenceDeviceSo : ScriptableObject
    {
        [SerializeField] private List<WebContentLoader> viewContentCollection;


        private readonly Dictionary<string, AssetReferenceGameObject> _referenceMap = new();
        private readonly Dictionary<string, GameObject> _buffer = new();


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

            var rawSet = new HashSet<string>(viewContentCollection.Select(v => v.Url));
            var instance = new HashSet<string>(_referenceMap.Select(v => v.Key));

            var exceptColl = rawSet.Except(instance).ToArray();

            if (exceptColl.Length == 0)
            {
                return;
            }

            foreach (var url in exceptColl)
            {
                _referenceMap.Add(url, viewContentCollection.Find(v => v.Url == url).AssetReferenceGameObject);
            }
        }


        #region Old

        public IEnumerator LoadPrefabricateAsset(string url, Action<GameObject> onPrefabricateLoadComplete)
        {
            TryLoadReferenceMap();
            if (!_referenceMap.TryGetValue(url, out var refGameObject))
            {
                Debug.LogWarning($"网址无效！{url}");
                yield break;
            }

            // 有预制体直接返回
            if (_buffer.TryGetValue(url, out var bufferPrefabricate))
            {
                onPrefabricateLoadComplete?.Invoke(bufferPrefabricate);
                yield break;
            }


            // 加载页面预制体
            var handle = refGameObject.LoadAssetAsync();
            yield return handle;

            // 缓存预制体
            _buffer.Add(url, handle.Result);
            onPrefabricateLoadComplete.Invoke(handle.Result);
        }

        public async UniTask<GameObject> LoadPrefabricateAssetAsync(string url)
        {
            TryLoadReferenceMap();
            if (!_referenceMap.TryGetValue(url, out var refGameObject))
            {
                throw new ArgumentException($"网址无效！{url}");
            }

            // 有预制体直接返回
            if (_buffer.TryGetValue(url, out var bufferPrefabricate))
            {
                return bufferPrefabricate;
            }


            // 加载页面预制体
            var result = await refGameObject.LoadAssetAsync();

            // 缓存预制体
            _buffer.Add(url, result);
            return result;
        }

        public IEnumerator LoadGameObjectInstanceAsset(string url, Transform parent, Action<GameObject> onInstanceCreationCompleted)
        {
            yield return LoadPrefabricateAsset(url, prefabricate => onInstanceCreationCompleted?.Invoke(Instantiate(prefabricate, parent)));
        }

        public async UniTask<GameObject> LoadGameObjectInstanceAssetAsync(string url, Transform parent)
        {
            return Object.Instantiate(await LoadPrefabricateAssetAsync(url), parent);
        }

        public IEnumerator LoadComponentAsset<T>(string url, Transform parent, Action<T> onInstanceLoadComplete) where T : Component
        {
            yield return LoadGameObjectInstanceAsset(url, parent, obj => onInstanceLoadComplete?.Invoke(obj.GetComponent(typeof(T)) as T));
        }

        public async UniTask<T> LoadComponentAssetAsync<T>(string url, Transform parent) where T : Component
        {
            return (await LoadGameObjectInstanceAssetAsync(url, parent)).GetComponent(typeof(T)) as T;
        }

        [CanBeNull]
        public GameObject GetPrefabricateAsset(string url)
        {
            return _buffer.GetValueOrDefault(url);
        }

        public GameObject GetGameObjectInstanceAsset(string url, Transform parent)
        {
            var prefabricate = GetPrefabricateAsset(url);
            return (object)prefabricate == null ? null : Instantiate(prefabricate, parent);
        }

        public T GetComponentAsset<T>(string url, Transform parent) where T : Component
        {
            var instance = GetGameObjectInstanceAsset(url, parent);
            if ((object)instance == null || !instance.TryGetComponent(typeof(T), out var component))
            {
                return null;
            }

            return (T)component;
        }

        public bool ReleaseAsset(string url)
        {
            if (!_referenceMap.TryGetValue(url, out var refGameObject))
            {
                return false;
            }

            refGameObject.ReleaseAsset();

            if (!_buffer.Remove(url))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}