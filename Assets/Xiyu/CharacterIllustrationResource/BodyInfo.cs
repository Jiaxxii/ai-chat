using UnityEngine;

namespace Xiyu.CharacterIllustrationResource
{
    [System.Serializable]
    public class BodyInfo
    {
        public BodyInfo(BodyType type, DataItem[] data)
        {
            Type = type;
            Data = data;
        }

        public BodyType Type { get; }
        public DataItem[] Data { get; }


        public enum BodyType
        {
            Body,
            Faces
        }
    }

    [System.Serializable]
    public readonly struct DataItem
    {
        public DataItem(Vector2 size, Vector2 position, string path)
        {
            Size = size;
            Position = position;
            Path = path;
        }

        public string Path { get; }
        public Vector2 Size { get; }
        public Vector2 Position { get; }
    }
}