using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Xiyu.AI.Prompt.NewPromptCenter
{
    [System.Serializable]
    public class PromptRequestListResult
    {
        [JsonProperty(PropertyName = "requestId")]
        public string RequestId { get; set; }

        [JsonProperty(PropertyName = "code")]
        [CanBeNull]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        [CanBeNull]
        public string Message { get; set; }


        [JsonProperty(PropertyName = "result")]
        public PromptListInfos PromptListInfos { get; set; }

        public void ThrowIfErrorRequested()
        {
            if (!string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Message))
            {
                throw new PromptRequestException(RequestId, Code, Message);
            }
        }
    }

    [System.Serializable]
    public class PromptListInfos
    {
        [JsonProperty(PropertyName = "pageInfo")]
        public PageInfo PageInfo { get; set; }

        [JsonProperty(PropertyName = "items")] public PromptInfo[] Items { get; set; }
    }

    [System.Serializable]
    public class PageInfo
    {
        [JsonProperty(PropertyName = "marker")]
        public string Marker { get; set; }

        [JsonProperty(PropertyName = "isTruncated")]
        public bool IsTruncated { get; set; }

        [JsonProperty(PropertyName = "nextMarker")]
        public string NextMarker { get; set; }

        [JsonProperty(PropertyName = "maxKeys")]
        public int MaxKeys { get; set; }
    }

    [System.Serializable]
    public class PromptInfo
    {
        [JsonProperty(PropertyName = "templateId")]
        public string TemplateId { get; set; }

        [JsonProperty(PropertyName = "templateName")]
        public string TemplateName { get; set; }

        [JsonProperty(PropertyName = "templateContent")]
        public string TemplateContent { get; set; }

        [JsonProperty(PropertyName = "templateVariables")]
        public string TemplateVariables { get; set; }

        [JsonProperty(PropertyName = "variableIdentifier")]
        public string VariableIdentifier { get; set; }

        [JsonProperty(PropertyName = "negativeTemplateContent")]
        public string NegativeTemplateContent { get; set; }

        [JsonProperty(PropertyName = "type")] public string Type { get; set; }

        [JsonProperty(PropertyName = "sceneType")]
        public string SceneType { get; set; }

        [JsonProperty(PropertyName = "labels")]
        public Labels[] Labels { get; set; }
        // 推理参数
        // public object HyperParameters { get; set; }
    }

    [System.Serializable]
    public class Labels
    {
        [JsonProperty(PropertyName = "labelId")]
        public string LabelId { get; set; }

        [JsonProperty(PropertyName = "labelName")]
        public string LabelName { get; set; }

        [JsonProperty(PropertyName = "color")] public string Color { get; set; }
    }
}