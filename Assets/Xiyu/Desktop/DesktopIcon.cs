using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xiyu.Constant;
using Xiyu.Expand;

namespace Xiyu.Desktop
{
    public class DesktopIcon : MonoBehaviour, IEquatable<DesktopIcon>
        , IDragHandler
        , IBeginDragHandler
        , IEndDragHandler
        , IPointerEnterHandler
        , IPointerExitHandler
        , IPointerClickHandler
    {
        public static readonly Vector2 ContentSize = new(150, 150);


        private static GameObject _prefab;
        private static GameObject Prefab => _prefab == null ? Resources.Load<GameObject>("RoleTemplate/Desktop ICON Item") : _prefab;


        [SerializeField] private Image content;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI appNameText;

        public event UnityAction<DesktopIcon, PointerEventData> OnPointerEnterEvent;
        public event UnityAction<DesktopIcon, PointerEventData> OnPointerExitEvent;
        public event UnityAction<DesktopIcon, PointerEventData> OnPointerClickEvent;


        public event UnityAction<DesktopIcon, PointerEventData> OnDragEvent;
        public event UnityAction<DesktopIcon, PointerEventData> OnBeginDragEvent;
        public event UnityAction<DesktopIcon, PointerEventData> OnEndDragEvent;

        public Color SelectColor
        {
            get => content.color;
            set => content.color = value;
        }


        public bool IsSelect { get; set; }


        private DesktopIcon Init(Sprite iconSprite, string appName)
        {
            SetHighlight(Color.clear);
            iconImage.sprite = iconSprite;
            appNameText.text = appName;
            return this;
        }

        private DesktopIcon Init(Icon icon) => Init(icon.IconSprite, icon.IconName);


        public Sprite IconSprite
        {
            get => iconImage.sprite;
            set => iconImage.sprite = value;
        }

        public string AppName
        {
            get => appNameText.text;
            set => appNameText.text = value;
        }


        public void SetHighlight(Color selectColor)
        {
            content.color = selectColor;
        }

        public static DesktopIcon Create(Transform parent, Sprite icon, string iconName)
        {
            var instance = Instantiate(Prefab, parent: parent).GetComponent<DesktopIcon>();

            return instance.Init(icon, iconName);
        }

        public static DesktopIcon Create(Transform parent, Icon icon) => Create(parent, icon.IconSprite, icon.IconName);


        public void OnDrag(PointerEventData eventData) => OnDragEvent?.Invoke(this, eventData);

        public void OnBeginDrag(PointerEventData eventData) => OnBeginDragEvent?.Invoke(this, eventData);

        public void OnEndDrag(PointerEventData eventData) => OnEndDragEvent?.Invoke(this, eventData);

        public void OnPointerEnter(PointerEventData eventData) => OnPointerEnterEvent?.Invoke(this, eventData);

        public void OnPointerExit(PointerEventData eventData) => OnPointerExitEvent?.Invoke(this, eventData);

        public void OnPointerClick(PointerEventData eventData) => OnPointerClickEvent?.Invoke(this, eventData);

        public bool Equals(DesktopIcon other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(content, other.content) && Equals(iconImage, other.iconImage) && Equals(appNameText, other.appNameText) &&
                   IsSelect == other.IsSelect;
        }
    }
}