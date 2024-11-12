using Newtonsoft.Json;

namespace Xiyu.VirtualLiveRoom.Component.DanmuItem.Data
{
    [System.Serializable]
    public readonly struct DanmuContent
    {
        public DanmuContent(string content, Color fontColor, Color panelColor)
        {
            Content = content;
            FontColor = fontColor;
            PanelColor = panelColor;
        }

        public string Content { get; }

        public Color FontColor { get; }

        public Color PanelColor { get; }
    }
}