using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Xiyu.AI.Client;
using Xiyu.Application;
using Xiyu.ArtificialIntelligence;
using Xiyu.LoggerSystem;

namespace Xiyu.AI.Prompt
{
    public static class PromptCenterProcessing
    {
        public static async Task Init(CancellationToken cancellationToken)
        {
            foreach (var filePath in Directory.GetFiles(ApplicationData.PromptPath, "*.json"))
            {
                var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken);
                _ = DeserializePrompt(content, out _, filePath);
            }
        }

        private static readonly Dictionary<string, Prompt> Map = new();


        private static readonly Uri Uri = new("https://qianfan.baidubce.com/wenxinworkshop/prompt/template/info");


        public static Prompt GetPrompt(string templateID)
        {
            if (Map.TryGetValue(templateID, out var value))
            {
                return value;
            }

            var promptPath = Path.Combine(ApplicationData.PromptPath, $"{templateID}.json");
            var content = FileSystem.File.ReadAllText(promptPath);

            // 如果返回的内容是空的表示没有这个文件
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException($"未从本地读取到\"{templateID}.json\"文件!");
            }

            _ = DeserializePrompt(content, out var prompt);
            return prompt;
        }

        public static async Task<Prompt> GetPromptAsync(string templateID, CancellationToken cancellationToken = default)
        {
            if (Map.TryGetValue(templateID, out var value))
            {
                return value;
            }

            var promptPath = Path.Combine(ApplicationData.PromptPath, $"{templateID}.json");
            var content = await FileSystem.File.ReadAllTextAsync(promptPath, cancellationToken);

            // 如果返回的内容是空的表示没有这个文件
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException($"未从本地读取到\"{templateID}.json\"文件!");
            }

            _ = DeserializePrompt(content, out var prompt);
            return prompt;
        }

        public static IEnumerator GetHttpPrompt(string templateID, UnityAction<Prompt> onComplete, CancellationToken cancellationToken = default,
            params (string valueName, string value)[] @params)
        {
            if (Map.TryGetValue(templateID,out var value))
            {
                onComplete.Invoke(value);
                yield break;
            }

            var auth = new IamAuthenticate(AuthenticateManager.AuthenticateElectronAuth.AccessKey, AuthenticateManager.AuthenticateElectronAuth.SecretKey);

            using var request = auth.ConfigureWebRequest(new Multimap<string, string>(), new Multimap<string, string>
            {
                { "Content-Type", "application/json" }
            }, HttpMethod.Post, Uri);


            var jObject = new JObject
            {
                ["id"] = templateID.StartsWith("pt-") ? templateID : throw new ArgumentException("模板ID不是\"pt-\"开头")
            };

            if (@params is { Length: > 0 })
            {
                foreach (var pair in @params)
                {
                    jObject.Add(pair.valueName, (JToken)pair.value);
                }
            }

            var body = jObject.ToString();

            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));

            yield return request.SendWebRequest();


            if (request.responseCode == 200 && request.result == UnityWebRequest.Result.Success)
            {
                var content = request.downloadHandler.text;

                if (DeserializePrompt(content, out var prompt))
                {
                    Save(prompt!, cancellationToken);
                }

                onComplete.Invoke(prompt);
            }
            else
            {
                // Debug.LogError($"请求失败！code:{request.responseCode}、{request.result.ToString()}\nerror:{request.error}");
                LoggerManager.Instance.LogError($"请求失败！code:{request.responseCode}、{request.result.ToString()}\nerror:{request.error}", cancellationToken: cancellationToken);
            }
        }


        private static bool DeserializePrompt(string value, out Prompt prompt, string promptPath = null)
        {
            try
            {
                prompt = JsonConvert.DeserializeObject<Prompt>(value);
                if (prompt!.IsDefaultOrNull())
                {
                    // Debug.LogError($"模板反序列化失败！{(string.IsNullOrEmpty(promptPath) ? string.Empty : promptPath)}");
                    LoggerManager.Instance.LogError($"模板反序列化失败！{(string.IsNullOrEmpty(promptPath) ? string.Empty : promptPath)}");
                    prompt = new Prompt
                    {
                        Error = { ErrorCode = 100, ErrorMessage = "模板序列后重要成员依然为空！" }
                    };
                    return false;
                }

                if (!Map.TryAdd(prompt.PromptResult.ID, prompt))
                {
                    // Debug.LogWarning($"重复包含的模板:{prompt.PromptResult.ID}");
                    LoggerManager.Instance.LogError($"重复包含的模板:{prompt.PromptResult.ID}");
                    return false;
                }

                return true;
            }
            catch (JsonSerializationException e)
            {
                // Debug.LogError($"模板反序列化失败！模板路径：{(string.IsNullOrEmpty(promptPath) ? string.Empty : promptPath)}\n{e.Message}");
                LoggerManager.Instance.LogError($"模板反序列化失败！模板路径：{(string.IsNullOrEmpty(promptPath) ? string.Empty : promptPath)}\n{e.Message}");
                prompt = new Prompt
                {
                    Error = { ErrorCode = 100, ErrorMessage = "模板序列后重要成员依然为空！" }
                };
                return false;
            }
        }

        private static async void Save(Prompt prompt, CancellationToken cancellationToken)
        {
            var saveFilePath = Path.Combine(ApplicationData.PromptPath, $"{prompt.PromptResult.ID}.json");

            await FileSystem.File.WriteAllTextAsync(saveFilePath, JsonConvert.SerializeObject(prompt), cancellationToken);
        }
    }
}