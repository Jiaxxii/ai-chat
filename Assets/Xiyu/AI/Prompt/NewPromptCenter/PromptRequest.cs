using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using Xiyu.AI.Client;
using Xiyu.ArtificialIntelligence;
using File = Xiyu.FileSystem.File;

namespace Xiyu.AI.Prompt.NewPromptCenter
{
    public static class PromptRequest
    {
        private static readonly Uri PromptListUri = new("https://qianfan.baidubce.com/v2/promptTemplates");

        private static readonly string PromptResources = Environment.CurrentDirectory + "/prompts";
        private static readonly string PromptList = Path.Combine(PromptResources, "list");

        public static readonly string DefaultPromptID = "pt-nr8kqave80gdw8dk";


        public static Prompt LastRequestPrompt { get; private set; }
        public static Prompt DefaultPrompt { get; private set; }

        public static async UniTask<List<PromptInfo>> RequestPromptListAsync(string marker = null, int maxKeys = 10, bool pageReverse = false, string name = null,
            string[] labelIds = null,
            string type = null,
            CancellationToken cancellationToken = default)
        {
            using var request = ConfigureWebRequest(PromptListUri, "DescribePromptTemplates");

            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler = GetPromptListRequestBodyUploadHandler(marker, maxKeys, pageReverse, name, labelIds, type);

            await request.SendWebRequest().WithCancellation(cancellationToken);

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new UnityWebRequestException(request);
            }

            var promptRequestList = JsonConvert.DeserializeObject<PromptRequestListResult>(request.downloadHandler.text);

            promptRequestList.ThrowIfErrorRequested();

            var prompts = new List<PromptInfo>(promptRequestList.PromptListInfos.Items);

            while (promptRequestList.PromptListInfos.PageInfo.IsTruncated)
            {
                var results = await RequestPromptListAsync(promptRequestList.PromptListInfos.PageInfo.NextMarker, maxKeys, pageReverse, name, labelIds, type, cancellationToken);
                prompts.AddRange(results);
            }

            return prompts;
        }

        public static async UniTask<List<PromptInfo>> TryRequestPromptListAsync(string marker = null, int maxKeys = 10, bool pageReverse = false, string name = null,
            string[] labelIds = null,
            string type = null,
            CancellationToken cancellationToken = default)
        {
            var directoryInfo = new DirectoryInfo(PromptList);
            List<PromptInfo> promptInfos;

            if (directoryInfo.Exists)
            {
                promptInfos = await ReadPromptListAsync(cancellationToken);
                if (promptInfos.Count > 0)
                {
                    return promptInfos;
                }
            }

            promptInfos = await RequestPromptListAsync(marker, maxKeys, pageReverse, name, labelIds, type, cancellationToken);
            await WritPromptListToAsync(promptInfos, cancellationToken);
            return promptInfos;
        }

        public static async UniTask<Prompt> TryRequestPromptAsync(string templateId, string[] placeholders, CancellationToken cancellationToken)
        {
            if (ContainsPrompt(templateId))
            {
                return LastRequestPrompt = await ReadPromptAsync(templateId, cancellationToken);
            }

            var prompt = await RequestPromptAsync(templateId, placeholders, cancellationToken);
            await WritPromptToAsync(prompt, cancellationToken);

            return prompt;
        }

        public static async UniTask WritPromptListToAsync(IEnumerable<PromptInfo> collect, CancellationToken cancellationToken)
        {
            foreach (var prompt in collect)
            {
                var fullName = Path.Combine(PromptList, $"{prompt.TemplateName}__{prompt.TemplateId}.json");

                var directoryInfo = new DirectoryInfo(PromptList);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                await File.WriteAllTextAsync(fullName, JsonConvert.SerializeObject(prompt, Formatting.Indented), cancellationToken);
            }
        }

        public static async UniTask<Prompt> RequestPromptAsync(string templateId, string[] placeholders, CancellationToken cancellationToken)
        {
            using var request = ConfigureWebRequest(PromptListUri, "DescribePromptTemplate");

            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler = GetPromptRequestBodyUploadHandler(templateId, placeholders);

            try
            {
                await request.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException e)
            {
                throw new PromptRequestException("NULL", "-1", e.Message);
            }


            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new UnityWebRequestException(request);
            }


            var promptInfo = JsonConvert.DeserializeObject<PromptRequestResult>(request.downloadHandler.text);
            promptInfo.ThrowIfErrorRequested();

            LastRequestPrompt = promptInfo.Prompt;

            return promptInfo.Prompt;
        }

        public static async UniTask<Prompt> ReadPromptAsync(string templateId, CancellationToken cancellationToken)
        {
            var directoryInfo = new DirectoryInfo(PromptResources);

            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(PromptResources);
            }

            var targetFile = directoryInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly)
                .FirstOrDefault(info => info.Name.Contains(templateId));

            if (targetFile == null)
            {
                throw new FileNotFoundException($"路径\"{PromptResources}\"(不递归)未找到名称中包含\"{templateId}\"并且以\"*.json\"结尾的文件！");
            }


            var jsonContent = await File.ReadAllTextAsync(targetFile.FullName, cancellationToken);

            if (string.IsNullOrEmpty(jsonContent))
            {
                throw new ArgumentException($"文件\"{targetFile.FullName}\"内容为空！");
            }


            var prompt = JsonConvert.DeserializeObject<Prompt>(jsonContent);

            // 检测Json序列化后重要的参数是否已被赋值，如果没有则抛出异常
            prompt.ThrowJsonSerializationException();

            return prompt;
        }


        public static async UniTask<List<PromptInfo>> ReadPromptListAsync(CancellationToken cancellationToken)
        {
            var directoryInfo = new DirectoryInfo(PromptList);

            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException();
            }

            var result = new List<PromptInfo>();
            foreach (var fileInfo in directoryInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly))
            {
                var jsonContent = await File.ReadAllTextAsync(fileInfo.FullName, cancellationToken);

                if (string.IsNullOrEmpty(jsonContent) || !jsonContent.StartsWith('{') || !jsonContent.EndsWith('}'))
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"文件\"{fileInfo.FullName}\"不是一个有效的模板Json文件！");
#endif
                    continue;
                }

                result.Add(JsonConvert.DeserializeObject<PromptInfo>(jsonContent));
            }

            return result;
        }

        public static async UniTask WritPromptToAsync(Prompt prompt, CancellationToken cancellationToken)
        {
            var fullName = Path.Combine(PromptResources, $"{prompt.TemplateName}__{prompt.TemplateId}.json");

            var directoryInfo = new DirectoryInfo(PromptResources);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            await File.WriteAllTextAsync(fullName, JsonConvert.SerializeObject(prompt, Formatting.Indented), cancellationToken);
        }

        public static bool ContainsPrompt(string templateId)
        {
            var directoryInfo = new DirectoryInfo(PromptResources);

            if (!directoryInfo.Exists) return false;

            return directoryInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly).Any(info => Regex.IsMatch(info.Name, $"__{templateId}.json"));
        }


        public static async UniTask<Prompt> TryLoadDefaultPrompt(bool upDateLocalData = true, CancellationToken cancellationToken = default)
        {
            // 如果是 同步服务器模板列表 = true 或者 本地没有默认模板时就从服务器获取新的模板列表
            if (!upDateLocalData && ContainsPrompt(DefaultPromptID))
            {
                return DefaultPrompt = await ReadPromptAsync(DefaultPromptID, cancellationToken);
            }

            var webPromptInfos = await RequestPromptListAsync(cancellationToken: cancellationToken);

            var defaultPromptInfo = webPromptInfos.FirstOrDefault(v => v.TemplateId == DefaultPromptID);
            if (defaultPromptInfo == null)
            {
                throw new PromptRequestException("服务器模板列表未更新，请联系作者通知更新（抖音：西雨与雨）");
            }

            var defaultPrompt = await RequestPromptAsync(defaultPromptInfo.TemplateId, null, cancellationToken);

            await WritPromptListToAsync(webPromptInfos, cancellationToken: cancellationToken);
            await WritPromptToAsync(defaultPrompt, cancellationToken);

            return defaultPrompt;
        }

        private static UnityWebRequest ConfigureWebRequest(Uri requestUri, string actionName)
        {
            var imaAuth = new IamAuthenticate(AuthenticateManager.AuthenticateElectronAuth.AccessKey, AuthenticateManager.AuthenticateElectronAuth.SecretKey);

            var query = new Multimap<string, string> { { "Action", actionName } };
            var header = new Multimap<string, string> { { "Content-Type", "application/json" } };

            return imaAuth.ConfigureWebRequest(query, header, HttpMethod.Post, requestUri);
        }


        private static UploadHandlerRaw GetPromptListRequestBodyUploadHandler(string marker = null, int maxKeys = 10, bool pageReverse = false, string name = null,
            string[] labelIds = null, string type = null)
        {
            var requestBody = new JObject
            {
                { nameof(maxKeys), (maxKeys <= 0 ? 10 : maxKeys) },
                { nameof(pageReverse), pageReverse }
            };
            if (!string.IsNullOrEmpty(marker)) requestBody.Add(nameof(marker), marker);
            if (!string.IsNullOrEmpty(name)) requestBody.Add(nameof(name), name);


            if (labelIds is { Length: > 0 })
            {
                requestBody.Add(nameof(labelIds).ToLower(), JArray.FromObject(labelIds));
            }

            if (!string.IsNullOrEmpty(type) && type == "Custom" || type == "System")
            {
                requestBody.Add(nameof(type), type);
            }

            return new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestBody.ToString(Formatting.None)));
        }

        private static UploadHandlerRaw GetPromptRequestBodyUploadHandler(string templateId, string[] placeholders)
        {
            var requestJson = new JObject { { nameof(templateId), templateId } };

            if (placeholders is { Length: > 0 })
            {
                for (var i = 0; i < placeholders.Length; i++)
                {
                    requestJson.Add($"var{i + 1}", placeholders[i]);
                }
            }

            var s = requestJson.ToString(Formatting.Indented);

            return new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestJson.ToString(Formatting.Indented)));
        }
    }
}