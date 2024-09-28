using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Xiyu.VirtualLiveRoom.Tools.Addressabe
{
    public abstract class AssetReferenceItem<T> : MonoBehaviour
        where T : UnityEngine.Object
    {
        private static readonly Lazy<AssetReferenceItem<T>> Lazy = new(FindObjectOfType<AssetReferenceItem<T>>);
        public static AssetReferenceItem<T> Instance => Lazy.Value;

        [SerializeField] private AssetReferenceT<T> assetReferenceType;

        [SerializeField] private string assetName;
        [SerializeField] private bool preload;


        // public AssetReferenceT<T> AssetReferenceType => assetReferenceType;

        public string AssetName => assetName;

        public bool Preload => preload;

        [CanBeNull] public T AssetInstance { get; private set; }


        // private bool _isCoroutineRun;
        // private bool _isLoading;
        public bool IsLoading { get; private set; }

        protected virtual void Awake()
        {
            if (preload)
            {
                StartCoroutine(TryLoadAssetAsync());
            }
        }


        public void TryLoadAsset(Action<T> onComplete = null)
        {
            if (IsLoading)
            {
                onComplete?.Invoke(null);
                return;
            }

            StartCoroutine(TryLoadAssetAsync(asset => onComplete?.Invoke(AssetInstance = asset)));
        }

        public IEnumerator TryLoadAssetAsync(Action<T> onComplete = null)
        {
            if (IsLoading)
            {
                yield break;
            }

            IsLoading = false;

            var handle = assetReferenceType.LoadAssetAsync();
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AssetInstance = handle.Result;
                onComplete?.Invoke(AssetInstance);
                IsLoading = true;
            }
            else
            {
                throw new Exception();
            }
        }

        public bool TryGetAsset(out T asset)
        {
            asset = AssetInstance;
            return IsLoading && AssetInstance != null;
        }
    }
}