#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Xiyu.AI.TextToSpeech
{
    public class TextToSpeechBody
    {
        public TextToSpeechBody()
        {
            Text = string.Empty;
            ReferenceId = string.Empty;
        }
        public TextToSpeechBody(string text, string referenceId)
        {
            Text = text;
            ReferenceId = referenceId;
        }

        public static JsonSerializerSettings JsonSerializerSettings { get; set; } = new()
        {
            Formatting = Formatting.None,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            },
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public string Text { get; set; }
        [JsonProperty("reference_id")] public string ReferenceId { get; set; }


        public FormatType Format { get; set; } = FormatType.Wav;
        [JsonProperty("mp3_bitrate")] public int BitrateMp3 { get; set; } = 128;
        [JsonProperty("opus_bitrate")] public int BitrateOpus { get; set; } = 32;

        public bool Normalize { get; set; } = true;

        public LatencyType Latency { get; set; } = LatencyType.Normal;


        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, JsonSerializerSettings);
        }


        public enum FormatType
        {
            [JsonProperty("wav")] Wav,
            [JsonProperty("pcm")] Pcm,
            [JsonProperty("mp3")] Mp3,
            [JsonProperty("opus")] Opus
        }

        public enum LatencyType
        {
            [JsonProperty("normal")] Normal,
            [JsonProperty("balanced")] Balanced
        }
    }
}