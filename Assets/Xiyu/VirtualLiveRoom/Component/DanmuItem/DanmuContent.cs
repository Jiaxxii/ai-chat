using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.VirtualLiveRoom.Component.DanmuItem.Data;

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

        public UnityEngine.Color FontColor
        {
            get => contentText.color;
            set => contentText.color = value;
        }

        public UnityEngine.Color PanelColor
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }

        public void UpdateData(Data.DanmuContent danmuContent)
        {
            Content = danmuContent.Content;
            FontColor = danmuContent.FontColor;
            PanelColor = danmuContent.PanelColor;
        }

        public Data.DanmuContent ReadOnlyData => new(Content, FontColor.ToXiyuColor(), PanelColor.ToXiyuColor());
    }
}