#nullable enable
using System;
using Newtonsoft.Json;

namespace Xiyu.AI.TextToSpeech
{
    public record ModelItemInfo
    {
        [JsonProperty("_id")] public string Id { get; set; } = string.Empty;

        public ModelType Type { get; set; } = ModelType.SVC;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public StateType State { get; set; } = StateType.Failed;

        public string[] Tags { get; set; } = Array.Empty<string>();

        public string[]? Languages { get; set; } = null;

        public VisibilityType Visibility { get; set; } = VisibilityType.UnList;

        public bool LockVisibility { get; set; } = false;

        public bool Liked { get; set; } = false;

        public bool Marked { get; set; } = false;

        public Author Author { get; set; } = new();


        public enum ModelType
        {
            [JsonProperty("svc")]
            // ReSharper disable once InconsistentNaming
            SVC,
            [JsonProperty("tts")] TTS
        }


        public enum StateType
        {
            Created,
            Training,
            Trained,
            Failed
        }

        public enum VisibilityType
        {
            Public,
            [JsonProperty("unlist")] UnList,
            Private
        }

        public void ToJson() => JsonConvert.SerializeObject(this, Formatting.None);
    }

    public record Author
    {
        [JsonProperty("_id")] public string Id { get; set; } = string.Empty;
        [JsonProperty("nickname")] public string Nickname { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}