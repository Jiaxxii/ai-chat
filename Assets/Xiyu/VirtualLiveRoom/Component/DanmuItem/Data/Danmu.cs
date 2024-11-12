namespace Xiyu.VirtualLiveRoom.Component.DanmuItem.Data
{
    [System.Serializable]
    public readonly struct Danmu
    {
        public Danmu(Color panelColor)
        {
            PanelColor = panelColor;
        }

        public Color PanelColor { get; }
    }
}