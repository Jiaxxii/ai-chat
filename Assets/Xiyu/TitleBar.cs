using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xiyu.VirtualLiveRoom;

namespace Xiyu
{
    public class ExchangeBoard
    {
        public ExchangeBoard(Icon current)
        {
            Current = current;
        }

        public int Index { get; set; }
        public Icon Current { get; }
    }

    public class TitleBar : MonoBehaviour
    {
        [SerializeField] private GameObject prf;
        [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;

        [SerializeField] private VirtualIconControl virtualIconControl;

        private readonly Dictionary<int, ExchangeBoard> _animationDictionary = new();

        private readonly List<ExchangeBoard> _exchangeList = new();


        [SerializeField] private Canvas canvas;

        private int _newIndex;

        private void Awake()
        {
            Append();
            Append();
            Append();
            Append();
            Append();
            Append();
        }

        public void Append()
        {
            var icon = Instantiate(prf, transform)
                .GetComponent<Icon>();

            icon.OnBeginDragEvent += OnDragBeginEventHandler;
            icon.OnDragEvent += OnDragEventHandler;
            icon.OnEndDragEvent += OnDragEndEventHandler;

            _exchangeList.Add(new ExchangeBoard(icon)
            {
                Index = GetNewIndex()
            });


            _animationDictionary.Add(icon.GetInstanceID(), _exchangeList[^1]);
        }

        private void OnDragBeginEventHandler(Icon icon, PointerEventData eventData)
        {
            virtualIconControl.SetClone(icon);

            virtualIconControl.Icon.RT.anchoredPosition = icon.RT.anchoredPosition;
        }

        private void OnDragEventHandler(Icon icon, PointerEventData eventData)
        {
            virtualIconControl.Icon.RT.anchoredPosition += new Vector2(eventData.delta.x / canvas.transform.localScale.x, 0);

            var currentExchangeBoard = _animationDictionary[icon.GetInstanceID()];

            if (!TryFindSwap(currentExchangeBoard, () => virtualIconControl.Icon.RT.anchoredPosition.x, out var swapExchangeBoard))
            {
                return;
            }


            horizontalLayoutGroup.enabled = false;

            var currentIndex = icon.transform.GetSiblingIndex();
            var targetIndex = swapExchangeBoard.Current.transform.GetSiblingIndex();

            icon.transform.SetSiblingIndex(targetIndex);
            swapExchangeBoard.Current.transform.SetSiblingIndex(currentIndex);

            (currentExchangeBoard.Index, swapExchangeBoard.Index) = (swapExchangeBoard.Index, currentExchangeBoard.Index);


            (icon.RT.anchoredPosition, swapExchangeBoard.Current.RT.anchoredPosition) = (swapExchangeBoard.Current.RT.anchoredPosition, icon.RT.anchoredPosition);

            horizontalLayoutGroup.enabled = true;
        }

        private void OnDragEndEventHandler(Icon icon, PointerEventData eventData)
        {
            virtualIconControl.Release();
        }


        // 找出需要交换位置的那个UI
        private bool TryFindSwap(ExchangeBoard exchangeBoard, [CanBeNull] Func<float> getCurrentPositionX, out ExchangeBoard swapExchangeBoard)
        {
            var leftEb = GetLeft(exchangeBoard);
            var right = GetRight(exchangeBoard);

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
    }
}