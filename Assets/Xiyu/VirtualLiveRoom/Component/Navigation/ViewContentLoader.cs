#if OldCode
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Xiyu.VirtualLiveRoom.Component.Navigation
{
    [Obsolete("组件已经弃用，请使用\"Xiyu.VirtualLiveRoom.Component.NewNavigation.WebContentLoader\"", false)]
    [Serializable]
    public class ViewContentLoader
    {
        [Header("资源")] [SerializeField] private string preformName;

        [SerializeField] private AssetReferenceT<GameObject> preformViewContent;

        [Header("网页")] [Space] [TextArea(1, 3)] [SerializeField]
        private string url;

        [TextArea(1, 2)] [SerializeField] private string titleName;
        [SerializeField] private Sprite webIcon;


        public string PreformName => preformName;


        public string URL => url;

        public Sprite WebIcon => webIcon;

        public string TitleName => titleName;

        public ViewContent ViewContent { get; private set; }

        public void Release()
        {
            if (!preformViewContent.IsDone)
            {
                return;
            }

            Addressables.Release(preformViewContent.OperationHandle);
            // UnityEngine.AddressableAssets.Addressables.ReleaseInstance(preformViewContent.OperationHandle);
        }

        public IEnumerator LoadAsync(Transform parent, Action<ViewContent> onComplete = null)
        {
            var handle = preformViewContent.LoadAssetAsync();
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                ViewContent = Object.Instantiate(handle.Result, parent: parent)
                    .GetComponent<ViewContent>();
                onComplete?.Invoke(ViewContent);
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public IEnumerator LoadAsync(Transform parent, TagPage tagPage, Action<ViewContent> onComplete = null)
        {
            var handle = preformViewContent.LoadAssetAsync();
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                ViewContent = Object.Instantiate(handle.Result, parent: parent)
                    .GetComponent<ViewContent>();

                ViewContent.TagPage = tagPage;

                onComplete?.Invoke(ViewContent);
            }
            else
            {
                throw new NullReferenceException();
            }
        }


        //
        // private sealed class PreformNameURLEqualityComparer : IEqualityComparer<ViewContent>
        // {
        //     public bool Equals(ViewContent x, ViewContent y)
        //     {
        //         if (ReferenceEquals(x, y)) return true;
        //         if (ReferenceEquals(x, null)) return false;
        //         if (ReferenceEquals(y, null)) return false;
        //         if (x.GetType() != y.GetType()) return false;
        //         return x.preformName == y.preformName && x.url == y.url;
        //     }
        //
        //     public int GetHashCode(ViewContent obj)
        //     {
        //         return HashCode.Combine(obj.preformName, obj.url);
        //     }
        // }
        //
        // public static IEqualityComparer<ViewContent> PreformNameURLComparer { get; } = new PreformNameURLEqualityComparer();
    }
}
#endif