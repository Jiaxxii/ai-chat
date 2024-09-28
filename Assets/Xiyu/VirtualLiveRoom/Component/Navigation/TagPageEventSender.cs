using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.Navigation
{
    public interface ITagPageEventSender
    {
        public event UnityAction<PointerEventData> OnTagPageDrag;
        public event UnityAction<PointerEventData> OnTagPageBeginDrag;
        public event UnityAction<PointerEventData> OnTagPageEndDrag;

        public event UnityAction<PointerEventData> OnTagPagePointerClick;

        public event UnityAction OnTagPageViewClose;
    }

    public class TagPageEventSender : MonoBehaviour, ITagPageEventSender,
        IDragHandler, IBeginDragHandler, IEndDragHandler,
        IPointerClickHandler
    {
        [SerializeField] private TagPage dragObject;
        [SerializeField] private Button tagPageCloseButton;

        public event UnityAction<PointerEventData> OnTagPageDrag;
        public event UnityAction<PointerEventData> OnTagPageBeginDrag;
        public event UnityAction<PointerEventData> OnTagPageEndDrag;

        public event UnityAction OnTagPageViewClose;

        public event UnityAction<PointerEventData> OnTagPagePointerClick;


        public TagPage Object => dragObject;


        private void Awake()
        {
            tagPageCloseButton.onClick.AddListener(() => OnTagPageViewClose?.Invoke());
        }


        public void OnDrag(PointerEventData eventData) => OnTagPageDrag?.Invoke(eventData);

        public void OnBeginDrag(PointerEventData eventData) => OnTagPageBeginDrag?.Invoke(eventData);

        public void OnEndDrag(PointerEventData eventData) => OnTagPageEndDrag?.Invoke(eventData);

        public void OnPointerClick(PointerEventData eventData) => OnTagPagePointerClick?.Invoke(eventData);
    }
}