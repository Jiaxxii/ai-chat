using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Xiyu
    .VirtualLiveRoom
    .Component
    .Navigation
{
    public class ExchangeBoard
    {
        public ExchangeBoard(TagPage current)
        {
            Current = current;
        }

        public int Index { get; set; }
        public TagPage Current { get; }
    }

    public class TagPageDragControl : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;

        [SerializeField] private Transform basePanel;

        [SerializeField] private DragCloneTagPage dragCloneTagPage;


        // [SerializeField] private UnityEngine.AddressableAssets.AssetReferenceT<GameObject> assetReferenceTagPage;
        // private static GameObject _preform;

        private readonly Dictionary<int, ExchangeBoard> _animationDictionary = new();

        private readonly List<ExchangeBoard> _exchangeList = new();

        // private int _nullSlotIndex,

        public IEnumerator Create(TagPage tagPage, ViewContentLoader viewContentLoader, Action<TagPage> onCreateComplete = null)
        {
            // 加载预制体 （只会在第一次调用时加载）
            // yield return LoadAssetAsync();

            // 开启自动排序
            layoutGroup.enabled = true;

            tagPage.SetTitle(viewContentLoader.WebIcon, viewContentLoader.TitleName);


            tagPage.TagPageEventSender.OnTagPageDrag += eventData => OnTagPageDragHandler(tagPage, eventData);
            tagPage.TagPageEventSender.OnTagPageBeginDrag += eventData => OnTagPageBeginDragHandler(tagPage, eventData);
            tagPage.TagPageEventSender.OnTagPageEndDrag += eventData => OnTagPageEndDragHandler(tagPage, eventData);


            _exchangeList.Add(new ExchangeBoard(tagPage)
            {
                Index = GetNewIndex()
            });


            _animationDictionary.Add(tagPage.GetInstanceID(), _exchangeList[^1]);

            onCreateComplete?.Invoke(tagPage);

            yield return null;
        }

        public void InitTagPage(TagPage tagPage, ViewContentLoader viewContentLoader)
        {
            // 加载预制体 （只会在第一次调用时加载）
            // yield return LoadAssetAsync();

            // 开启自动排序
            layoutGroup.enabled = true;

            tagPage.SetTitle(viewContentLoader.WebIcon, viewContentLoader.TitleName);


            tagPage.TagPageEventSender.OnTagPageDrag += eventData => OnTagPageDragHandler(tagPage, eventData);
            tagPage.TagPageEventSender.OnTagPageBeginDrag += eventData => OnTagPageBeginDragHandler(tagPage, eventData);
            tagPage.TagPageEventSender.OnTagPageEndDrag += eventData => OnTagPageEndDragHandler(tagPage, eventData);


            _exchangeList.Add(new ExchangeBoard(tagPage)
            {
                Index = GetNewIndex()
            });


            _animationDictionary.Add(tagPage.GetInstanceID(), _exchangeList[^1]);
        }


        public bool Remove(TagPage tagPage)
        {
            if (!_animationDictionary.TryGetValue(tagPage.GetInstanceID(), out var exchangeBoard))
            {
                return false;
            }

            _animationDictionary.Remove(tagPage.GetInstanceID());
            var targetIndex = _exchangeList.FindIndex(v => v.Index == exchangeBoard.Index);

            _exchangeList[targetIndex].Current.DestroyContent();

            for (var i = targetIndex; i < _exchangeList.Count - 1; i++)
            {
                _exchangeList[i + 1].Index = _exchangeList[i].Index;
            }

            _newIndex = _exchangeList[^1].Index + 1;

            _exchangeList.RemoveAt(targetIndex);

            return true;
        }

        private int _newIndex;


        private void OnTagPageBeginDragHandler(TagPage tagPage, PointerEventData eventData)
        {
            dragCloneTagPage.SetDrag(tagPage);
        }

        private void OnTagPageDragHandler(TagPage tagPage, PointerEventData eventData)
        {
            dragCloneTagPage.RT.anchoredPosition += new Vector2(eventData.delta.x / canvas.transform.localScale.x, 0);

            var currentExchangeBoard = _animationDictionary[tagPage.GetInstanceID()];

            if (!TryFindSwap(currentExchangeBoard, () => dragCloneTagPage.RT.anchoredPosition.x, out var swapExchangeBoard))
            {
                return;
            }


            layoutGroup.enabled = false;

            var currentIndex = tagPage.transform.GetSiblingIndex();
            var targetIndex = swapExchangeBoard.Current.transform.GetSiblingIndex();

            tagPage.transform.SetSiblingIndex(targetIndex);
            swapExchangeBoard.Current.transform.SetSiblingIndex(currentIndex);

            (currentExchangeBoard.Index, swapExchangeBoard.Index) = (swapExchangeBoard.Index, currentExchangeBoard.Index);


            (tagPage.RT.anchoredPosition, swapExchangeBoard.Current.RT.anchoredPosition) = (swapExchangeBoard.Current.RT.anchoredPosition, tagPage.RT.anchoredPosition);

            layoutGroup.enabled = true;
        }


        private void OnTagPageEndDragHandler(TagPage tagPage, PointerEventData eventData)
        {
            dragCloneTagPage.ReleaseDrag();
        }


        // 找出需要交换位置的那个UI
        private bool TryFindSwap(ExchangeBoard exchangeBoard, [CanBeNull] Func<float> getCurrentPositionX, out ExchangeBoard swapExchangeBoard)
        {
            var leftEb = GetLeft(exchangeBoard);
            var right = GetRight(exchangeBoard);

            // Debug.Log($"left:{(leftEb == null ? "空" : "非空")}，right:{(right == null ? "空" : "非空")}");

            var currentX = getCurrentPositionX?.Invoke() ?? exchangeBoard.Current.RT.anchoredPosition.x;
            if (leftEb != null && leftEb.Current.RT.anchoredPosition.x > currentX)
            {
                swapExchangeBoard = leftEb;
                return true;
            }

            if (right != null && right.Current.RT.anchoredPosition.x < currentX)
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
        private ExchangeBoard GetRight(ExchangeBoard current) => current.Index < _exchangeList.Count - 1 ? _exchangeList.Find(v => v.Index == current.Index + 1) : null;


        // private IEnumerator LoadAssetAsync()
        // {
        //     if (_preform != null)
        //     {
        //         yield break;
        //     }
        //
        //     var handle = assetReferenceTagPage.LoadAssetAsync();
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