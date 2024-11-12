using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.VirtualLiveRoom.Component.DanmuItem.Data;

namespace Xiyu.VirtualLiveRoom.Component.DanmuItem
{
    public class DanmuUserName : UIContainer
    {
        [SerializeField] private Image basePanel;
        [SerializeField] private TextMeshProUGUI userNameText;

        public UnityEngine.Color FontColor
        {
            get => userNameText.color;
            set => userNameText.color = value;
        }


        public string Name
        {
            get => userNameText.text;
            set => userNameText.text = value;
        }

        public UnityEngine.Color PanelColor
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }

        public void UpdateData(Data.DanmuUserName danmuUserName)
        {
            Name = danmuUserName.Name;
            PanelColor = danmuUserName.PanelColor;
            FontColor = danmuUserName.FontColor;
        }


        public Data.DanmuUserName ReadOnlyData => new(FontColor.ToXiyuColor(), Name, PanelColor.ToXiyuColor());
    }
}