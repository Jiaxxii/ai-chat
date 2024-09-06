using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Xiyu.Desktop
{
    public interface IDragProcessor
    {
        public event UnityAction<PointerEventData> OnBeginDragHandler;

        public event UnityAction<PointerEventData> OnEndDragHandler;

        public event UnityAction<PointerEventData> OnDragHandler;
    }


    [RequireComponent(typeof(DesktopIcon))]
    public class DesktopIconDragEventProcessor : MonoBehaviour, IDragProcessor, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private UnityEvent<PointerEventData> onBeginDrag;
        [SerializeField] private UnityEvent<PointerEventData> onEndDrag;
        [SerializeField] private UnityEvent<PointerEventData> onDrag;


        public event UnityAction<PointerEventData> OnBeginDragHandler
        {
            add => onBeginDrag.AddListener(value);
            remove => onBeginDrag.RemoveListener(value);
        }

        public event UnityAction<PointerEventData> OnEndDragHandler
        {
            add => onEndDrag.AddListener(value);
            remove => onEndDrag.RemoveListener(value);
        }

        public event UnityAction<PointerEventData> OnDragHandler
        {
            add => onDrag.AddListener(value);
            remove => onDrag.RemoveListener(value);
        }


        public void OnBeginDrag(PointerEventData eventData) => onBeginDrag?.Invoke(eventData);

        public void OnEndDrag(PointerEventData eventData) => onEndDrag?.Invoke(eventData);

        public void OnDrag(PointerEventData eventData) => onDrag?.Invoke(eventData);
    }
}