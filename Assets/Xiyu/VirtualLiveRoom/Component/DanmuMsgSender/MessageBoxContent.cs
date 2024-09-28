using UnityEngine;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.DanmuMsgSender
{
    public class MessageBoxContent : UIContainer
    {
        [SerializeField] private Image basePanel;


        public Sprite BackGround
        {
            get => basePanel.sprite;
            set => SetBackGround(value);
        }

        public Color PanelColor
        {
            get => basePanel.color;
            set => SetBackGround(value);
        }

        private void SetBackGround(Sprite bg)
        {
            basePanel.sprite = bg;
            basePanel.color = Color.white;
        }

        private void SetBackGround(Color color)
        {
            basePanel.sprite = null;
            basePanel.color = color;
        }
    }
}