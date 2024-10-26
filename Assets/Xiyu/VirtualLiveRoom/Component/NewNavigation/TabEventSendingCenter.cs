using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.NewNavigation
{
    /// <summary>
    /// 标题栏拖拽窗口的事件发射器
    /// </summary>
    public class TabEventSendingCenter : MonoBehaviour, ITabEventSendingCenter, IPointerClickHandler,
        IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Button closeTabButton;

        [SerializeField] private Tab tab;
        public Tab Tab => tab;

        public event UnityAction<PageInfo, PointerEventData> OnTagDrag;
        public event UnityAction<PageInfo, PointerEventData> OnTagBeginDrag;
        public event UnityAction<PageInfo, PointerEventData> OnTagEndDrag;

        public event UnityAction<PageInfo> OnTabClose;

        public event UnityAction<PageInfo, PointerEventData> OnTagPointerClick;


        private void Awake() => closeTabButton.onClick.AddListener(() => OnTabClose?.Invoke(Tab.PageInfo));


        public void OnDrag(PointerEventData eventData) => OnTagDrag?.Invoke(Tab.PageInfo, eventData);

        public void OnBeginDrag(PointerEventData eventData) => OnTagBeginDrag?.Invoke(Tab.PageInfo, eventData);

        public void OnEndDrag(PointerEventData eventData) => OnTagEndDrag?.Invoke(Tab.PageInfo, eventData);

        public void OnPointerClick(PointerEventData eventData) => OnTagPointerClick?.Invoke(Tab.PageInfo, eventData);
    }

    public interface ITabEventSendingCenter
    {
        public event UnityAction<PageInfo, PointerEventData> OnTagDrag;
        public event UnityAction<PageInfo, PointerEventData> OnTagBeginDrag;
        public event UnityAction<PageInfo, PointerEventData> OnTagEndDrag;

        public event UnityAction<PageInfo> OnTabClose;

        public event UnityAction<PageInfo, PointerEventData> OnTagPointerClick;

        public Tab Tab { get; }
    }
}