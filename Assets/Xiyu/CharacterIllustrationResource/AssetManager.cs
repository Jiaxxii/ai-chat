using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Xiyu.CharacterIllustrationResource
{
    public sealed class AssetManager<T>
    {
        // Dictionary<string,AssetLoader<T>>
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            Formatting = Formatting.None,
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };


        public static async UniTask LoadMappingTable(string path)
        {
            var jsonContent = await System.IO.File.ReadAllTextAsync(path, System.Text.Encoding.UTF8);

            JsonConvert.DeserializeObject<Dictionary<string, BodyInfo>>(jsonContent, JsonSerializerSettings);
        }
    }
}