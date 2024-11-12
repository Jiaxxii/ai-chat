#nullable enable
using System;
using Xiyu.VirtualLiveRoom.Component.DanmuItem.Data;

namespace Xiyu.VirtualLiveRoom.View.LiveRoomView
{
    public readonly struct HistoricalRecord : IEquatable<HistoricalRecord>
    {
        public HistoricalRecord(DanmuData[]? historicalRecords)
        {
            HistoricalRecords = historicalRecords;
        }

        public DanmuData[]? HistoricalRecords { get; }


        public static HistoricalRecord None { get; } = new(null);



        public override bool Equals(object? obj)
        {
            return obj is HistoricalRecord other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (HistoricalRecords != null ? HistoricalRecords.GetHashCode() : 0);
        }

        public static bool operator ==(HistoricalRecord left, HistoricalRecord right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HistoricalRecord left, HistoricalRecord right)
        {
            return !left.Equals(right);
        }

        public bool Equals(HistoricalRecord other)
        {
            return Equals(HistoricalRecords, other.HistoricalRecords);
        }
    }
}