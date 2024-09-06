using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Xiyu.Desktop
{

    public interface IPointerProcessor
    {
        public event UnityAction<PointerEventData> OnPointerEnterHandler;
        public event UnityAction<PointerEventData> OnPointerExitHandler;
    }


    [RequireComponent(typeof(DesktopIcon))]
    public class DesktopIconPointerEventProcessor : MonoBehaviour, IPointerProcessor, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent<PointerEventData> onPointEnter;
        [SerializeField] private UnityEvent<PointerEventData> onPointExit;

        public event UnityAction<PointerEventData> OnPointerEnterHandler
        {
            add => onPointEnter.AddListener(value);
            remove => onPointEnter.RemoveListener(value);
        }

        public event UnityAction<PointerEventData> OnPointerExitHandler
        {
            add => onPointExit.AddListener(value);
            remove => onPointExit.RemoveListener(value);
        }

        public void OnPointerEnter(PointerEventData eventData) => onPointEnter?.Invoke(eventData);

        public void OnPointerExit(PointerEventData eventData) => onPointExit?.Invoke(eventData);
    }
}