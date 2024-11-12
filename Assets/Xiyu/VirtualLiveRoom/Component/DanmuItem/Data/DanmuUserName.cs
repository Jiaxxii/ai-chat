#nullable enable

using Newtonsoft.Json;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom.Component.DanmuItem.Data
{
    [System.Serializable]
    public readonly struct DanmuUserName
    {
        public DanmuUserName(Color fontColor, string name, Color panelColor)
        {
            FontColor = fontColor;
            Name = name;
            PanelColor = panelColor;
        }

        public Color FontColor { get; }

        public string Name { get; }

        public Color PanelColor { get; }
    }
}