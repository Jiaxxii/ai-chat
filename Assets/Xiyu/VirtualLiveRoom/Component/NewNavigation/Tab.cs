using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.NewNavigation
{
    /// <summary>
    /// 一个标题栏 (页面选项卡)
    /// </summary>
    public class Tab : UIContainer
    {
        [SerializeField] private Image basePanel;

        // public CancellationToken DestroyCancellationToken => destroyCancellationToken;
        
        public Vector2 AnchoredPosition
        {
            get => basePanel.rectTransform.anchoredPosition;
            set => basePanel.rectTransform.anchoredPosition = value;
        }

        public Vector2 SizeDelta
        {
            get => basePanel.rectTransform.sizeDelta;
            set => basePanel.rectTransform.sizeDelta = value;
        }


        [SerializeField] private CanvasGroup contentCanvasGroup;

        public float Alpha
        {
            get => contentCanvasGroup.alpha;
            set => contentCanvasGroup.alpha = value;
        }


        [SerializeField] private Image iconImage;

        public Sprite Icon
        {
            get => iconImage.sprite;
            set => iconImage.sprite = value;
        }


        [SerializeField] private TextMeshProUGUI titleText;

        public string Content
        {
            get => titleText.text;
            set => titleText.text = value;
        }


        [SerializeField] private TabEventSendingCenter eventCenter;
        public ITabEventSendingCenter EventCenter => eventCenter;


        public PageInfo PageInfo { get; private set; }

        private Tab Init(PageInfo pageInfo)
        {
            titleText.text = pageInfo.Title;
            iconImage.sprite = pageInfo.Icon;
            PageInfo = pageInfo;

            return this;
        }


        public void SetContentActiveAndRay(bool active)
        {
            contentCanvasGroup.alpha = active ? 1 : 0;
            contentCanvasGroup.interactable = active;
            contentCanvasGroup.blocksRaycasts = active;
        }

        private static GameObject _gameObject;

        public static IEnumerator LoadPreformCoroutine(Action<GameObject> onComplete)
        {
            if (_gameObject != null)
            {
                onComplete?.Invoke(_gameObject);
                yield break;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>("new Tab");
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete?.Invoke(_gameObject = handle.Result);
            }
        }

        public static async UniTask<GameObject> LoadPreformAsync()
        {
            if (_gameObject != null)
            {
                return _gameObject;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>("new Tab");
            await UniTask.WaitUntil(() => handle.IsDone);

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception("Failed to load asset: " + handle.OperationException);
            }
            
            // 我的 Unity版本(2022.3.14)不需要也没有“Release”方法
            return (_gameObject = handle.Result);
        }
        


        internal static Tab CreateTab(GameObject original, Transform parent, PageInfo pageInfo)
        {
            var instantiate = UnityEngine.Object.Instantiate(original, parent);

            if (instantiate.TryGetComponent(typeof(Tab), out var component))
            {
                return ((Tab)component).Init(pageInfo);
            }

            for (var i = 0; i < instantiate.transform.childCount; i++)
            {
                if (instantiate.transform.GetChild(i).TryGetComponent(typeof(Tab), out component))
                {
                    return ((Tab)component).Init(pageInfo);
                }
            }

            throw new NullReferenceException($"游戏对象\"{instantiate.gameObject.name}\"（顶级与子级第一层）未找到组件\"{nameof(Tab)}\"!");
        }
    }
}