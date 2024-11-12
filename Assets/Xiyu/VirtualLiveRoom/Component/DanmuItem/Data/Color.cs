using System.Runtime.CompilerServices;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom.Component.DanmuItem.Data
{
    public static class ColorToUnityColorExpand
    {
        public static Xiyu.VirtualLiveRoom.Component.DanmuItem.Data.Color ToXiyuColor(this UnityEngine.Color color)
        {
            return new Xiyu.VirtualLiveRoom.Component.DanmuItem.Data.Color(color.r, color.g, color.b, color.a);
        }
    }

    public readonly struct Color
    {
        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public float R { get; }
        public float G { get; }
        public float B { get; }
        public float A { get; }

        public static implicit operator UnityEngine.Color(Xiyu.VirtualLiveRoom.Component.DanmuItem.Data.Color color)
        {
            return new UnityEngine.Color(color.R, color.G, color.B, color.A);
        }
    }
}