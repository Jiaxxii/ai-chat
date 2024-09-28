using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.Navigation
{
    public class TagPage : UIContainer
    {
        [SerializeField] private Image basePanel;
        [SerializeField] private RectTransform content;

        [SerializeField] private Image headIma;
        [SerializeField] private TextMeshProUGUI titleText;

        [SerializeField] private CanvasGroup contentCanvasGroup;

        [SerializeField] private TagPageEventSender tagPageEventSender;

        public ITagPageEventSender TagPageEventSender => tagPageEventSender;

        public RectTransform RT => content;

        public float ContentAlpha
        {
            get => contentCanvasGroup.alpha;
            set => contentCanvasGroup.alpha = value;
        }

        public void SetContentActive(bool active)
        {
            ContentAlpha = active ? 1 : 0;
            contentCanvasGroup.interactable = active;
            contentCanvasGroup.blocksRaycasts = active;
        }

        public string Title
        {
            get => titleText.text;
            set => titleText.text = value;
        }

        public Sprite Icon
        {
            get => headIma.sprite;
            set => headIma.sprite = value;
        }

        public Vector2 AnchoredPosition
        {
            get => content.anchoredPosition;
            set => content.anchoredPosition = value;
        }

        public void SetTitle(Sprite icon, string title)
        {
            headIma.sprite = icon;
            titleText.text = title;
            titleText.color = Color.white;
        }

        public void DestroyContent()
        {
            Destroy(gameObject);
        }


        public static TagPage Create(GameObject asset, Transform parent) => Instantiate(asset, parent).GetComponent<TagPage>();
    }
}