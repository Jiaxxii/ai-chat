using UnityEngine;
using UnityEngine.UI;
using Xiyu.VirtualLiveRoom.EventFunctionSystem;

namespace Xiyu.VirtualLiveRoom.Component.DanmuMsgSender
{
    public class MessageBoxContent : UIContainer
    {
        [SerializeField] private Image basePanel;

        [WebContentInit(false)]
        protected override Cysharp.Threading.Tasks.UniTask Initialization(System.Threading.CancellationToken cancellationToken = default)
        {
            return base.Initialization(cancellationToken);
        }


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