using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Xiyu.CharacterIllustration
{
    public readonly struct TransformInfo
    {
        public TransformInfo(string path, Vector2 position, Vector2 size)
        {
            Path = path;
            Position = position;
            Size = size;
        }

        public TransformInfo(string path, float x, float y, float width, float height)
        {
            Path = path;
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public TransformInfo(JObject property)
        {
            if (!property.TryGetValue("path", out var pathToken))
            {
                throw new ArgumentException("提供的JObject中无属性:path");
            }

            if (!property.TryGetValue("x", out var xToken) || !property.TryGetValue("y", out var yToken))
            {
                throw new ArgumentException("提供的JObject中无属性:x or y");
            }

            if (!property.TryGetValue("width", out var widthToken) || !property.TryGetValue("height", out var heightToken))
            {
                throw new ArgumentException("提供的JObject中无属性:width or height");
            }

            Path = pathToken.Value<string>();
            Position = new Vector2(xToken.Value<float>(), yToken.Value<float>());
            Size = new Vector2(widthToken.Value<float>(), heightToken.Value<float>());
        }

        public string Path { get; }
        public Vector2 Position { get; }
        public Vector2 Size { get; }

    }
}