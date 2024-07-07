using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Xiyu.AIChat.LargeLanguageModel.Service;
using Debug = UnityEngine.Debug;

namespace Xiyu.AIChat.LargeLanguageModel
{
    public enum HttpMethod
    {
        Get,
        Post
    }


    public abstract class LargeLanguageModel<TRequestBody, TResponse> : MonoBehaviour
    {
        [SerializeField] protected ConfigSetting<TRequestBody> configSetting;


        public TRequestBody RequestBody => configSetting.Body;

        protected readonly Stopwatch Timer = new();

        public event Action<TRequestBody> OnSendRequestEventHandler;
        public event Action<TResponse> OnReceiveResponseEventHandler;
        public event Action<string> OnSendRequestFailEventHandler;
        

        public virtual IEnumerator Request(Action<TResponse> onRequestComplete = null, Action<string> onError = null)
        {
            Timer.Restart();
            var queryString = GetQueryString(configSetting);

            var url = string.IsNullOrEmpty(queryString) ? configSetting.Url : string.Concat(configSetting.Url, "?", queryString);
            using var request = new UnityWebRequest(url, configSetting.Method);

            foreach (var header in configSetting.RequestOptions.HeaderParameters)
            {
                // request.SetRequestHeader(header.Key, header.Value);
                request.SetRequestHeader(header.Key, null);
            }


            var jsonData = JsonConvert.SerializeObject(configSetting.Body, configSetting.RequestBodyJss);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();


            OnSendRequestEventHandler?.Invoke(configSetting.Body);
            yield return request.SendWebRequest();


            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<TResponse>(request.downloadHandler.text, configSetting.ResponseJss);
                
                if (response == null)
                {
                    Debug.LogError(request.downloadHandler.text);
                    OnSendRequestFailEventHandler?.Invoke(request.downloadHandler.text);
                    yield break;
                }

                Timer.Stop();
                onRequestComplete?.Invoke(response);
                OnReceiveResponseEventHandler?.Invoke(response);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError(request.error);
#endif
                onError?.Invoke(request.error);
                OnSendRequestFailEventHandler?.Invoke(request.error);
            }
        }

        protected string GetQueryString(IServiceRequest<TRequestBody> requestBody = null)
        {
            // requestBody ??= configSetting;
            //
            // var queryStringBuilder = new StringBuilder();
            //
            // // var queryStringArray = requestBody.GetQueryString().ToArray();
            // for (var i = 0; i < queryStringArray.Length; i++)
            // {
            //     var queryString = queryStringArray[i];
            //
            //     if (string.IsNullOrEmpty(queryString.Key) || queryString.Key == "authorization")
            //     {
            //         continue;
            //     }
            //
            //     queryStringBuilder.Append(queryString.Key);
            //     queryStringBuilder.Append('=');
            //     queryStringBuilder.Append(System.Net.WebUtility.UrlEncode(queryString.Value));
            //
            //     if (i < queryStringArray.Length - 1)
            //     {
            //         queryStringBuilder.Append('&');
            //     }
            // }
            //
            // return queryStringBuilder.Length == 0 ? null : queryStringBuilder.ToString();
            return null;
        }
    }
}