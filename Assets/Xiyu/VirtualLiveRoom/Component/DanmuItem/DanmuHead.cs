using UnityEngine;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.DanmuItem
{
    public class DanmuHead : UIContainer
    {
        [SerializeField] private Image head;

        // [SerializeField] private Mask mask;
        // [SerializeField] private Image headBaseImage;
        [SerializeField] private Image basePanel;

        public Color PanelColor
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }


        public void SetHeadSprite(Sprite sprite, Vector2 offset, Vector2 imageSize)
        {
            head.sprite = sprite;
            head.rectTransform.sizeDelta = imageSize;
            head.rectTransform.anchoredPosition += offset;
        }
    }
}