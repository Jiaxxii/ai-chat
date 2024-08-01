using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using Xiyu.AI.LargeLanguageModel.Service.Response;
using Xiyu.LoggerSystem;

namespace Xiyu.AI.Prompt
{
    public class Prompt : DeserializeParameterModule
    {
        [JsonProperty(PropertyName = "status")]
        public int HttpStateCode { get; set; }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonIgnore]
        public override string Result
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }


        [JsonProperty(PropertyName = "result")]
        public PromptResult PromptResult { get; set; }

        public override bool IsDefaultOrNull() => PromptResult == null || PromptResult.IsDefaultOrNull();

        public string ToString(Dictionary<string, string> keyValuePairMap)
        {
            // 参数格式
            var variableIdentifier = PromptResult.VariableIdentifier;

            // 占位参数
            var variables = PromptResult.TemplateVariables.Split(',');

            var sb = new StringBuilder(PromptResult.RawContent);
            foreach (var var in variables)
            {
                var placeholderVar = variableIdentifier.Insert(1, var);

                if (keyValuePairMap.TryGetValue(var, out var value))
                {
                    sb.Replace(placeholderVar, value);
                }
                else
                {
                    // Debug.LogWarning($"不包含的占位变量名称：\'{var}\' not in range[{string.Join(',', variables)}]");
                    LoggerManager.Instance.LogWarning($"不包含的占位变量名称：\'{var}\' not in range[{string.Join(',', variables)}]");
                }
            }


            return sb.ToString();
        }
    }

    [Serializable]
    public class PromptResult
    {
        [JsonProperty(PropertyName = "templatePK")]
        public string ID { get; set; }


        [JsonProperty(PropertyName = "templateName")]
        public string Name { get; set; }


        [JsonProperty(PropertyName = "templateContent")]
        public string RawContent { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }


        [JsonProperty(PropertyName = "templateVariables")]
        public string TemplateVariables { get; set; }


        [JsonProperty(PropertyName = "variableIdentifier")]
        public string VariableIdentifier { get; set; }

        public bool IsDefaultOrNull() => string.IsNullOrEmpty(ID) ||
                                         string.IsNullOrEmpty(Name) ||
                                         string.IsNullOrEmpty(RawContent) ||
                                         string.IsNullOrEmpty(Content) ||
                                         string.IsNullOrEmpty(TemplateVariables) ||
                                         string.IsNullOrEmpty(VariableIdentifier);
    }
}