#if OldCode
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.Navigation
{
    [Obsolete("组件已经弃用，请使用\"Xiyu.VirtualLiveRoom.Component.NewNavigation.ITabEventSendingCenter\"", false)]
    public interface ITagPageEventSender
    {
        public event UnityAction<PointerEventData> OnTagDrag;
        public event UnityAction<PointerEventData> OnTagBeginDrag;
        public event UnityAction<PointerEventData> OnTagEndDrag;

        public event UnityAction<PointerEventData> OnTagPagePointerClick;

        public event UnityAction OnTagPageViewClose;
    }

    [Obsolete("组件已经弃用，请使用\"Xiyu.VirtualLiveRoom.Component.NewNavigation.TabEventSendingCenter\"", false)]
    public class TagPageEventSender : MonoBehaviour, ITagPageEventSender,
        IDragHandler, IBeginDragHandler, IEndDragHandler,
        IPointerClickHandler
    {
        [SerializeField] private TagPage dragObject;
        [SerializeField] private Button tagPageCloseButton;

        public event UnityAction<PointerEventData> OnTagDrag;
        public event UnityAction<PointerEventData> OnTagBeginDrag;
        public event UnityAction<PointerEventData> OnTagEndDrag;

        public event UnityAction OnTagPageViewClose;

        public event UnityAction<PointerEventData> OnTagPagePointerClick;


        public TagPage Object => dragObject;


        private void Awake()
        {
            tagPageCloseButton.onClick.AddListener(() => OnTagPageViewClose?.Invoke());
        }


        public void OnDrag(PointerEventData eventData) => OnTagDrag?.Invoke(eventData);

        public void OnBeginDrag(PointerEventData eventData) => OnTagBeginDrag?.Invoke(eventData);

        public void OnEndDrag(PointerEventData eventData) => OnTagEndDrag?.Invoke(eventData);

        public void OnPointerClick(PointerEventData eventData) => OnTagPagePointerClick?.Invoke(eventData);
    }
}
#endif