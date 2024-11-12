using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Xiyu.VirtualLiveRoom.Component.NewNavigation
{
    public class ExchangeBoard
    {
        public ExchangeBoard(Tab current)
        {
            Current = current;
        }

        public int Index { get; set; }
        public Tab Current { get; }
    }

    public class TabDragControl : UIContainer
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;

        [SerializeField] private Transform basePanel;

        [SerializeField] private DragCloneTab dragCloneTab;

        private readonly Dictionary<int, ExchangeBoard> _animationDictionary = new();

        private readonly List<ExchangeBoard> _exchangeList = new();

        public Tab LastTab => _exchangeList[^1].Current;


        public IEnumerator Create(PageInfo pageInfo, Action<Tab> onCreateComplete = null)
        {
            GameObject preform = null;
            yield return Tab.LoadPreformCoroutine(v => preform = v);

            // 开启自动排序
            layoutGroup.enabled = true;

            var tab = Tab.CreateTab(preform, basePanel, pageInfo);

            tab.EventCenter.OnTagBeginDrag += (_, eventData) => OnTabBeginDragHandler(tab, eventData);
            tab.EventCenter.OnTagDrag += (_, eventData) => OnTabDragHandler(tab, eventData);
            tab.EventCenter.OnTagEndDrag += (_, eventData) => OnTabEndDragHandler(tab, eventData);


            _exchangeList.Add(new ExchangeBoard(tab)
            {
                Index = GetNewIndex()
            });


            _animationDictionary.Add(tab.GetInstanceID(), _exchangeList[^1]);

            onCreateComplete?.Invoke(tab);
            yield return new WaitForEndOfFrame();
        }

        public async UniTask<Tab> CreateTabAsync(PageInfo pageInfo)
        {
            var preform = await Tab.LoadPreformAsync();

            // 开启自动排序
            layoutGroup.enabled = true;

            var tab = Tab.CreateTab(preform, basePanel, pageInfo);

            tab.EventCenter.OnTagBeginDrag += (_, eventData) => OnTabBeginDragHandler(tab, eventData);
            tab.EventCenter.OnTagDrag += (_, eventData) => OnTabDragHandler(tab, eventData);
            tab.EventCenter.OnTagEndDrag += (_, eventData) => OnTabEndDragHandler(tab, eventData);


            _exchangeList.Add(new ExchangeBoard(tab)
            {
                Index = GetNewIndex()
            });


            _animationDictionary.Add(tab.GetInstanceID(), _exchangeList[^1]);

            await UniTask.WaitForEndOfFrame(this);
            return tab;
        }

        public bool RemoveTag(Tab tab)
        {
            if (!_animationDictionary.TryGetValue(tab.GetInstanceID(), out var exchangeBoard))
            {
                return false;
            }

            _animationDictionary.Remove(tab.GetInstanceID());
            var targetIndex = _exchangeList.FindIndex(v => v.Index == exchangeBoard.Index);

            // Debug.Break();
            // _exchangeList[targetIndex].Current.DestroyContent();

            for (var i = targetIndex; i < _exchangeList.Count - 1; i++)
            {
                _exchangeList[i + 1].Index = _exchangeList[i].Index;
            }

            _newIndex = _exchangeList[^1].Index + 1;

            _exchangeList.RemoveAt(targetIndex);

            Object.Destroy(tab.gameObject);

            return true;
        }

        private int _newIndex;


        private void OnTabBeginDragHandler(Tab tab, PointerEventData eventData)
        {
            dragCloneTab.SetDrag(tab);
        }

        private void OnTabDragHandler(Tab tab, PointerEventData eventData)
        {
            dragCloneTab.AnchoredPosition += new Vector2(eventData.delta.x / canvas.transform.localScale.x, 0);

            var currentExchangeBoard = _animationDictionary[tab.GetInstanceID()];

            if (!TryFindSwap(currentExchangeBoard, () => dragCloneTab.AnchoredPosition.x, out var swapExchangeBoard))
            {
                return;
            }


            layoutGroup.enabled = false;

            var currentIndex = tab.transform.GetSiblingIndex();
            var targetIndex = swapExchangeBoard.Current.transform.GetSiblingIndex();

            tab.transform.SetSiblingIndex(targetIndex);
            swapExchangeBoard.Current.transform.SetSiblingIndex(currentIndex);

            (currentExchangeBoard.Index, swapExchangeBoard.Index) = (swapExchangeBoard.Index, currentExchangeBoard.Index);


            (tab.AnchoredPosition, swapExchangeBoard.Current.AnchoredPosition) = (swapExchangeBoard.Current.AnchoredPosition, tab.AnchoredPosition);

            layoutGroup.enabled = true;
        }


        private void OnTabEndDragHandler(Tab tab, PointerEventData eventData)
        {
            dragCloneTab.ReleaseDrag();
        }


        // 找出需要交换位置的那个UI
        private bool TryFindSwap(ExchangeBoard exchangeBoard, [CanBeNull] Func<float> getCurrentPositionX, out ExchangeBoard swapExchangeBoard)
        {
            var leftEb = GetLeft(exchangeBoard);
            var right = GetRight(exchangeBoard);

            var currentX = getCurrentPositionX?.Invoke() ?? exchangeBoard.Current.AnchoredPosition.x;
            if (leftEb != null && leftEb.Current.AnchoredPosition.x > currentX)
            {
                swapExchangeBoard = leftEb;
                return true;
            }

            if (right != null && right.Current.AnchoredPosition.x < currentX)
            {
                swapExchangeBoard = right;
                return true;
            }

            swapExchangeBoard = null;
            return false;
        }


        private int GetNewIndex() => _newIndex++;

        [CanBeNull]
        private ExchangeBoard GetLeft(ExchangeBoard current) => current.Index > 0 ? _exchangeList.Find(v => v.Index == current.Index - 1) : null;

        [CanBeNull]
        private ExchangeBoard GetRight(ExchangeBoard current) =>
            current.Index < _exchangeList.Count - 1 ? _exchangeList.Find(v => v.Index == current.Index + 1) : null;


        // private IEnumerator LoadAssetAsync()
        // {
        //     if (_preform != null)
        //     {
        //         yield break;
        //     }
        //
        //     var handle = assetReferenceTab.LoadAssetAsync();
        //
        //     yield return handle;
        //
        //
        //     if (handle.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         _preform = handle.Result;
        //     }
        //     else
        //     {
        //         _preform = null;
        //         throw new NullReferenceException();
        //     }
        // }
    }
}