using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Xiyu.AI.Prompt.NewPromptCenter
{
    public class PromptRequestResult
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
        public Prompt Prompt { get; set; }

        public void ThrowIfErrorRequested()
        {
            if (!string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Message))
            {
                throw new PromptRequestException(RequestId, Code, Message);
            }
        }
    }

    public class Prompt
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


        public string GetPromptString(params (string varName, string varValue)[] var)
        {
            if (VariableIdentifier.Length % 2 != 0)
            {
                throw new ArgumentException($"错误的变量参数包裹\"{VariableIdentifier}\"，它应该是成对出现的！");
            }

            var variableNames = new HashSet<string>(TemplateVariables.Split(','));
            var stringBuilder = new StringBuilder(TemplateContent);

            foreach (var varInfo in var)
            {
                if (variableNames.Contains(varInfo.varName))
                {
                    var variable = VariableIdentifier.Insert(VariableIdentifier.Length / 2, varInfo.varName);
                    stringBuilder.Replace(variable, varInfo.varValue);
                }
                else
                {
                    Debug.LogWarning($"未定义的变量名称：{varInfo.varName}。(集合中[{string.Join(',', variableNames)}])");
                }
            }

            return stringBuilder.ToString();
        }

        public string GetPromptString(params KeyValuePair<string, string>[] var)
        {
            if (VariableIdentifier.Length % 2 != 0)
            {
                throw new ArgumentException($"错误的变量参数包裹\"{VariableIdentifier}\"，它应该是成对出现的！");
            }

            var variableNames = new HashSet<string>(TemplateVariables.Split(','));
            var stringBuilder = new StringBuilder(TemplateContent);

            foreach (var varInfo in var)
            {
                if (variableNames.Contains(varInfo.Key))
                {
                    var variable = VariableIdentifier.Insert(VariableIdentifier.Length / 2, varInfo.Key);
                    stringBuilder.Replace(variable, varInfo.Value);
                }
                else
                {
                    Debug.LogWarning($"未定义的变量名称：{varInfo.Key}。(集合中[{string.Join(',', variableNames)}])");
                }
            }

            return stringBuilder.ToString();
        }

        public void ThrowJsonSerializationException()
        {
            if (string.IsNullOrEmpty(TemplateId) || string.IsNullOrEmpty(TemplateName) || string.IsNullOrEmpty(TemplateContent))
            {
                throw new JsonSerializationException();
            }
        }
    }
}