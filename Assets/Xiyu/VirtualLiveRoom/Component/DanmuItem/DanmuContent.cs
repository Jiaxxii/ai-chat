using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.DanmuItem
{
    public class DanmuContent : UIContainer
    {
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private Image basePanel;

        public string Content
        {
            get => contentText.text;
            set => contentText.text = value;
        }

        public Color FontColor
        {
            get => contentText.color;
            set => contentText.color = value;
        }

        public Color PanelColor
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }
    }
}