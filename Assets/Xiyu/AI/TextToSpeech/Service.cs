#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Xiyu.AI.Client;
using Xiyu.LoggerSystem;
using Logger = Xiyu.LoggerSystem.Logger;

namespace Xiyu.AI.TextToSpeech
{
    public static class Service
    {
        public static async UniTask<float> GetApiAvailableAsync()
        {
            using var webRequest = new UnityWebRequest("https://api.fish.audio/wallet/self/api-credit", HttpMethod.Get.ToString());

            webRequest.SetRequestHeader("Authorization", $"Bearer {AuthenticateManager.AuthenticateElectronAuth.TTSAuth}");

            webRequest.downloadHandler = new DownloadHandlerBuffer();

            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var creditStr = JsonConvert.DeserializeObject<ApiAvailableRequest>(webRequest.downloadHandler.text)!.Credit;

                var credit = Convert.ToSingle(creditStr);

                return credit;
            }

            return -1;
        }


        public static async UniTask<ModelItemInfo?> GetWebModelInfo(string modelId = "ba1a1f11162241a3b2a6a61724126be5")
        {
            using var webRequest = new UnityWebRequest($"https://api.fish.audio/model/{modelId}", HttpMethod.Get.ToString());

            webRequest.SetRequestHeader("Authorization", $"Bearer {AuthenticateManager.AuthenticateElectronAuth.TTSAuth}");

            webRequest.downloadHandler = new DownloadHandlerBuffer();

            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var modelItemInfo = JsonConvert.DeserializeObject<ModelItemInfo>(webRequest.downloadHandler.text);

                return modelItemInfo;
            }

            return null;
        }


        /// <summary>
        /// 获取模型列表并且筛选
        /// </summary>
        /// <param name="self">仅返回（查找）自己创建的模型</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageNumber">页面号</param>
        /// <param name="title">筛选模型的标题</param>
        /// <param name="tags">筛选模型的语言</param>
        /// <param name="description">介绍</param>
        /// <param name="languages">筛选模型的语言</param>
        /// <param name="titleLanguage">用于筛选模型的标题语言</param>
        /// <returns></returns>
        public static async UniTask<IEnumerable<ModelItemInfo>?> GetWebModelInfoList(
            bool self = false,
            int pageSize = 10,
            int pageNumber = 1,
            string? title = null,
            string[]? tags = null,
            string? description = null,
            string[]? languages = null, string? titleLanguage = null)
        {
            using var webRequest = new UnityWebRequest("https://api.fish.audio/model", HttpMethod.Get.ToString());

            webRequest.SetRequestHeader("Authorization", $"Bearer {AuthenticateManager.AuthenticateElectronAuth.TTSAuth}");
            webRequest.SetRequestHeader("self", self.ToString());


            if (pageSize <= 0)
                webRequest.SetRequestHeader("page_size", 10.ToString());

            if (pageNumber <= 0)
                webRequest.SetRequestHeader("page_number", 1.ToString());

            if (!string.IsNullOrEmpty(title))
                webRequest.SetRequestHeader("title", title);

            if (!string.IsNullOrEmpty(description))
                webRequest.SetRequestHeader("description", description);

            if (tags != null && tags.Any(s => !string.IsNullOrEmpty(s)))
                webRequest.SetRequestHeader("tags", JsonConvert.SerializeObject(tags.Select(s => !string.IsNullOrEmpty(s)), Formatting.None));

            if (languages != null && languages.Any(s => !string.IsNullOrEmpty(s)))
                webRequest.SetRequestHeader("languages", JsonConvert.SerializeObject(languages.Select(s => !string.IsNullOrEmpty(s)), Formatting.None));

            if (!string.IsNullOrEmpty(titleLanguage))
                webRequest.SetRequestHeader("title_language", titleLanguage);

            webRequest.downloadHandler = new DownloadHandlerBuffer();
            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var modelItemInfos = JsonConvert.DeserializeObject<ModelItemInfoListRequest>(webRequest.downloadHandler.text)!;

                return modelItemInfos.ModelItemInfos;
            }

            return Array.Empty<ModelItemInfo>();
        }


        public static async UniTask<AudioClip?> TextToSpeech(TextToSpeechBody body, float gain, CancellationToken cancellationToken)
        {
            var wavByte = await TextToSpeech(body, cancellationToken);

            if (wavByte.Length <= 0)
            {
                return null;
            }

            // WAV文件头信息通常包含“RIFF”和“WAVE”，以及音频的格式数据
            // 对于PCM WAV文件，你需要知道采样率、通道数和位深度来正确创建AudioClip
            const int sampleRate = 44100; // 从你的WAV文件信息中获取
            const int channels = 1; // 从你的WAV文件信息中获取，这里是单声道
            const int bitsPerSample = 16; // 从你的WAV文件信息中获取，这里是16位深度
            var lengthSamples = (wavByte.Length - 44) / (channels * bitsPerSample / 8); // 计算音频数据的长度，减去WAV头的大小(通常是 44 字节)

            // 使用AudioClip.Create方法创建AudioClip
            var audioClip = AudioClip.Create(body.Text, lengthSamples, channels, sampleRate, false);

            // 将字节数据填充到AudioClip中
            // 注意：这里假设WAV文件的头信息是44字节，并且已经被跳过。
            // 如果你的WAV文件头不是44字节，你需要相应地调整偏移量。
            audioClip.SetData(ConvertBytesToFloat(wavByte, 44, lengthSamples, channels, bitsPerSample, gain), 0);


            return audioClip;
        }


        private static readonly TimeoutController TimeoutController = new();

        public static async UniTask<byte[]> TextToSpeech(TextToSpeechBody body, CancellationToken cancellationToken)
        {
            using var webRequest = new UnityWebRequest("https://api.fish.audio/v1/tts", HttpMethod.Post.ToString());

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", $"Bearer {AuthenticateManager.AuthenticateElectronAuth.TTSAuth}");


            var requestBody = body.ToJson();


            webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestBody));
            webRequest.downloadHandler = new DownloadHandlerBuffer();


            await webRequest.SendWebRequest().WithCancellation(cancellationToken);
            TimeoutController.Reset();

            await LoggerManager.Instance.LogInfoAsync($"请求状态：{webRequest.result}，error：{webRequest.error} other：{webRequest}", cancellationToken: cancellationToken);

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                return webRequest.downloadHandler.data;
            }


            return Array.Empty<byte>();
        }


        private static float[] ConvertBytesToFloat(byte[] source, int offset, int lengthSamples, int channels, int bitsPerSample, float gain)
        {
            var floatArray = new float[lengthSamples * channels];
            var bytesPerSample = bitsPerSample / 8;
            for (var i = 0; i < lengthSamples; i++)
            {
                for (var c = 0; c < channels; c++)
                {
                    var sampleIndex = offset + i * channels * bytesPerSample + c * bytesPerSample;
                    if (bitsPerSample == 16) // 16位深度
                    {
                        var sample = BitConverter.ToInt16(source, sampleIndex);
                        var sample32 = (sample / 32768f) * gain; // 将16位样本转换为[-1, 1]范围内的浮点数
                        floatArray[i * channels + c] = sample32;
                    }
                    // 如果需要，这里可以添加对其他位深度的支持，如8位或24位等。
                }
            }

            return floatArray;
        }

        private record ModelItemInfoListRequest
        {
            [JsonProperty(PropertyName = "items")] public ModelItemInfo[] ModelItemInfos { get; set; } = Array.Empty<ModelItemInfo>();
        }

        private record ApiAvailableRequest
        {
            [JsonProperty(PropertyName = "credit")]
            public string Credit { get; set; } = string.Empty;
        }
    }
}