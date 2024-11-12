using Newtonsoft.Json;

namespace Xiyu.VirtualLiveRoom.Component.DanmuItem.Data
{
    [System.Serializable]
    public readonly struct DanmuHead
    {
        public DanmuHead(string spriteName, Vector2 offset, Vector2 size, Color panelColor)
        {
            SpriteName = spriteName;
            Offset = offset;
            Size = size;
            PanelColor = panelColor;
        }

        public Color PanelColor { get; }

        public string SpriteName { get; }

        public Vector2 Offset { get; }

        public Vector2 Size { get; }
    }
}