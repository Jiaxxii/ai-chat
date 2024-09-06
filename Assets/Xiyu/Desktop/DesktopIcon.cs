using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xiyu.Desktop
{
    public partial class DesktopIcon : MonoBehaviour
    {
        public static readonly Vector2 ContentSize = new(150, 150);

        public Vector2Int DesktopMatrix { get; internal set; }


        [SerializeField] private Image content;

        public Color SelectPanelColor
        {
            get => content.color;
            set => content.color = value;
        }


        [SerializeField] private Image iconImage;

        public Sprite IconSprite
        {
            get => iconImage.sprite;
            set => iconImage.sprite = value;
        }


        [SerializeField] private TextMeshProUGUI appNameText;

        public string AppName
        {
            get => appNameText.text;
            set => appNameText.text = value;
        }


        [SerializeField] private DesktopIconPointerEventProcessor pointerEventProcessor;
        public IPointerProcessor PointerProcessor => pointerEventProcessor;


        [SerializeField] private DesktopIconDragEventProcessor dragEventProcessor;
        public IDragProcessor DragProcessor => dragEventProcessor;


        [SerializeField] private Color selectColor;

        public Color SelectColor
        {
            get => selectColor;
            set => selectColor = value;
        }


        [SerializeField] private Color clearColor;

        public Color ClearColor
        {
            get => clearColor;
            set => clearColor = value;
        }


        private DesktopIcon Init(Sprite iconSprite, string appName)
        {
            SelectPanelColor = Color.clear;
            IconSprite = iconSprite;
            AppName = appName;

            PointerProcessor.OnPointerEnterHandler += _ => SelectPanelColor = SelectColor;
            PointerProcessor.OnPointerExitHandler += _ => SelectPanelColor = ClearColor;

            SetActive(true);

            return this;
        }

        public void SetAnchoredPosition(Vector2 anchoredPosition)
        {
            ((RectTransform)transform).anchoredPosition = anchoredPosition;
        }

        public void SetActive(bool value)
        {
            content.gameObject.SetActive(value);
        }
    }
    
}