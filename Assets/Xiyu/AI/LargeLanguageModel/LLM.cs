// 西雨

using System;
using System.Collections;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using Xiyu.AI.Client;
using Xiyu.AI.LargeLanguageModel.Service.Request;
using Xiyu.AI.LargeLanguageModel.Service.Response;
using Xiyu.LoggerSystem;

namespace Xiyu.AI.LargeLanguageModel
{
    /// <summary>
    /// 针对 百度大语言模型-chat 中的请求接口
    /// </summary>
    public class LLM
    {
        // ReSharper disable once UnusedMember.Local
        private LLM()
        {
        }

        /// <summary>
        /// 初始化接口（服务）
        /// </summary>
        /// <param name="authenticate">鉴权凭证，目前提供两种方式:<see cref="AccessToken"/>,<see cref="IamAuthenticate"/></param>
        /// <param name="uri">接口地址:https</param>
        /// <param name="method">请求方式 （默认为：<see cref="HttpMethod.Post"/>）</param>
        public LLM(Authenticate authenticate, Uri uri, HttpMethod method = null)
        {
            Auth = authenticate;
            Url = uri;
            HttpMethod = method ?? HttpMethod.Post;
            // _downloadHandler = downloadHandler ?? new DownloadHandlerBuffer();

            OnRequestFailEventHandler += v => LoggerManager.Instance.LogError(v,true);
        }


        /// <summary>
        /// 在请求发送错误时调用
        /// </summary>
        public event Action<string> OnRequestFailEventHandler;

        protected readonly Authenticate Auth;
        protected readonly Uri Url;
        protected readonly HttpMethod HttpMethod;

        // private DownloadHandler _downloadHandler;

        /// <summary>
        /// 请求数据 Body
        /// </summary>
        public RequestResource RequestResource { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int OutTimeSecond { get; set; } = 15; // SECOND

        /// <summary>
        /// <see cref="Request"/> 类型 T 的反序列化样式
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = new()
        {
            Formatting = Formatting.None,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        /// <summary>
        /// 发送一次请求
        /// </summary>
        /// <param name="requestOptions">Query Header</param>
        /// <param name="onSuccess">完成时返回原文本数据(JSON)</param>
        /// <returns>需要使用<see cref="UnityEngine.MonoBehaviour.StartCoroutine(IEnumerator)"/></returns>
        public virtual IEnumerator Request(RequestOptions requestOptions, Action<string> onSuccess)
        {
            using var request = SetConfigureWebRequest(requestOptions);

            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();


            if (request.responseCode == 200 && request.result == UnityWebRequest.Result.Success)
            {
                var resultJson = request.downloadHandler.text;
                onSuccess?.Invoke(resultJson);
            }
            else
            {
                OnRequestFailEventHandler?.Invoke(request.error);
            }
        }


        /// <summary>
        /// 发送一次请求
        /// </summary>
        /// <param name="requestOptions">Query Header</param>
        public virtual async UniTask<string> RequestAsync(RequestOptions requestOptions)
        {
            using var request = SetConfigureWebRequest(requestOptions);

            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();


            if (request.responseCode == 200 && request.result == UnityWebRequest.Result.Success)
            {
                var resultJson = request.downloadHandler.text;
                return resultJson;
            }

            OnRequestFailEventHandler?.Invoke(request.error);
            return await UniTask.FromException<string>(new UnityWebRequestException(request));
        }

        /// <summary>
        /// 发送一次请求
        /// </summary>
        /// <param name="requestOptions">Query Header</param>
        /// <param name="onSuccess">完成时返回原文本数据(JSON)</param>
        /// <typeparam name="T"><see cref="T"/>必须继承自<see cref="DeserializeParameterModule"/></typeparam>
        /// <returns>需要使用<see cref="UnityEngine.MonoBehaviour.StartCoroutine(IEnumerator)"/></returns>
        public virtual IEnumerator Request<T>(RequestOptions requestOptions, Action<T> onSuccess) where T : DeserializeParameterModule, new()
        {
            using var request = SetConfigureWebRequest(requestOptions);

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();


            if (request.responseCode == 200 && request.result == UnityWebRequest.Result.Success)
            {
                var resultJson = request.downloadHandler.text;

                var response = DeserializeParameterModule.Deserialize<T>(resultJson, JsonSerializerSettings);
                onSuccess?.Invoke(response);
            }
            else
            {
                OnRequestFailEventHandler?.Invoke(request.error);
            }
        }

        /// <summary>
        /// 发送一次请求
        /// </summary>
        /// <param name="requestOptions">Query Header</param>
        /// <typeparam name="T"><see cref="T"/>必须继承自<see cref="DeserializeParameterModule"/></typeparam>
        public virtual async UniTask<T> RequestAsync<T>(RequestOptions requestOptions) where T : DeserializeParameterModule, new()
        {
            using var request = SetConfigureWebRequest(requestOptions);

            request.downloadHandler = new DownloadHandlerBuffer();
            await request.SendWebRequest();


            if (request.responseCode == 200 && request.result == UnityWebRequest.Result.Success)
            {
                var resultJson = request.downloadHandler.text;

                var response = DeserializeParameterModule.Deserialize<T>(resultJson, JsonSerializerSettings);
                return response;
            }

            OnRequestFailEventHandler?.Invoke(request.error);
            return await UniTask.FromException<T>(new UnityWebRequestException(request));
        }
        
        
        protected virtual UnityWebRequest SetConfigureWebRequest(RequestOptions requestOptions)
        {
            var request = Auth.ConfigureWebRequest(requestOptions, HttpMethod, Url);

#if UNITY_EDITOR
            var jsonContent = RequestResource.ToJson();

            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonContent));
#else
            request.uploadHandler = new UploadHandlerRaw(RequestResource.ToJson(System.Text.Encoding.UTF8));
#endif

            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }
    }
}