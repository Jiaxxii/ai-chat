#nullable enable
using System;

namespace Xiyu.VirtualLiveRoom.View.LiveRoomView
{
    [System.Serializable]
    public readonly struct LiveRoomInfo : IEquatable<LiveRoomInfo>
    {
        public LiveRoomInfo(string anchorName, string anchorPersonalIntroduction, int hot, int visitor)
        {
            AnchorName = anchorName;
            AnchorPersonalIntroduction = anchorPersonalIntroduction;
            Hot = hot;
            Visitor = visitor;
        }

        public string AnchorName { get; }

        public string AnchorPersonalIntroduction { get; }

        public int Hot { get; }

        public int Visitor { get; }


        public static LiveRoomInfo None { get; } = new(string.Empty, string.Empty, 0, 0);

        public bool Equals(LiveRoomInfo other)
        {
            return AnchorName == other.AnchorName && AnchorPersonalIntroduction == other.AnchorPersonalIntroduction && Hot == other.Hot && Visitor == other.Visitor;
        }

        public override bool Equals(object? obj)
        {
            return obj is LiveRoomInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AnchorName, AnchorPersonalIntroduction, Hot, Visitor);
        }

        public static bool operator ==(LiveRoomInfo left, LiveRoomInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LiveRoomInfo left, LiveRoomInfo right)
        {
            return !left.Equals(right);
        }
    }
}