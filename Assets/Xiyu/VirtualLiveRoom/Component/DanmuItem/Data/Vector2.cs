namespace Xiyu.VirtualLiveRoom.Component.DanmuItem.Data
{
    public static class Vector2ToUnityVector2Expand
    {
        public static Xiyu.VirtualLiveRoom.Component.DanmuItem.Data.Vector2 ToXiyuVector2(this UnityEngine.Vector2 vector)
        {
            return new Xiyu.VirtualLiveRoom.Component.DanmuItem.Data.Vector2(vector.x, vector.y);
        }
    }

    public readonly struct Vector2
    {
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }
        public float Y { get; }


        public static implicit operator UnityEngine.Vector2(Xiyu.VirtualLiveRoom.Component.DanmuItem.Data.Vector2 vector)
        {
            return new UnityEngine.Vector2(vector.X, vector.Y);
        }
    }
}