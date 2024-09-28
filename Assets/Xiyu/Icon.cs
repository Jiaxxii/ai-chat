using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xiyu
{
    public class Icon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image basePanel;

        public RectTransform RT => basePanel.rectTransform;

        public Color Color
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }


        public event UnityAction<Icon, PointerEventData> OnBeginDragEvent;
        public event UnityAction<Icon, PointerEventData> OnDragEvent;
        public event UnityAction<Icon, PointerEventData> OnEndDragEvent;


        public void OnBeginDrag(PointerEventData eventData) => OnBeginDragEvent?.Invoke(this, eventData);

        public void OnDrag(PointerEventData eventData) => OnDragEvent?.Invoke(this, eventData);

        public void OnEndDrag(PointerEventData eventData) => OnEndDragEvent?.Invoke(this, eventData);
    }
}