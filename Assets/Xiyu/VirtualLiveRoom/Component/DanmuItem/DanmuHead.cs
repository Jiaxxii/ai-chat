using UnityEngine;
using UnityEngine.UI;
using Xiyu.VirtualLiveRoom.Component.DanmuItem.Data;

namespace Xiyu.VirtualLiveRoom.Component.DanmuItem
{
    public class DanmuHead : UIContainer
    {
        [SerializeField] private Image head;
        [SerializeField] private Image basePanel;

        public UnityEngine.Color PanelColor
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }


        public void SetHeadSprite(Sprite sprite, UnityEngine.Vector2 offset, UnityEngine.Vector2 imageSize)
        {
            head.sprite = sprite;
            head.rectTransform.sizeDelta = imageSize;
            head.rectTransform.anchoredPosition += offset;
        }


        public void UpdateData(Data.DanmuHead danmuHead)
        {
            var sprite = Resources.Load<Sprite>($"Default/User/{danmuHead.SpriteName}");

            if (sprite == null)
            {
                sprite = Resources.Load<Sprite>("Default/User/main");
            }

            head.sprite = sprite;
            PanelColor = danmuHead.PanelColor;
            head.rectTransform.sizeDelta = danmuHead.Size;
            head.rectTransform.anchoredPosition += danmuHead.Offset;
        }


        public Data.DanmuHead ReadOnlyData => new(head.sprite.name, head.rectTransform.anchoredPosition.ToXiyuVector2(), head.rectTransform.sizeDelta.ToXiyuVector2(),
            PanelColor.ToXiyuColor());
    }
}