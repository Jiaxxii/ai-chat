using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.DanmuItem
{
    public class DanmuUserName : UIContainer
    {
        [SerializeField] private Image basePanel;
        [SerializeField] private TextMeshProUGUI userNameText;

        public Color FontColor
        {
            get => userNameText.color;
            set => userNameText.color = value;
        }


        public string Name
        {
            get => userNameText.text;
            set => userNameText.text = value;
        }

        public Color PanelColor
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }
        
        
    }
}