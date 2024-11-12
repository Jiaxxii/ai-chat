using System;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom.Component.NewNavigation
{
    [Serializable]
    public class AssetGameObjectLoader
    {
        [SerializeField] private string key;
        [SerializeField] private UnityEngine.AddressableAssets.AssetReferenceGameObject preformViewContent;

        public string Key => key;
        public UnityEngine.AddressableAssets.AssetReferenceGameObject AssetReferenceGameObject => preformViewContent;
    }
}