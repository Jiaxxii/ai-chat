using System;
using System.Collections;
using System.Security.Cryptography;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Xiyu.AIChat
{
    public class AccessToken
    {
        public string ApiKey { get; }
        public string ClientSecret { get; }

        public AccessToken(string apiKey, string clientSecret)
        {
            ApiKey = apiKey;
            ClientSecret = clientSecret;
        }

        public IEnumerator GetTokenAsync(Action<TokenInfo> onTokenComplete)
        {
            yield return GetTokenAsync(ApiKey, ClientSecret, onTokenComplete);
        }

        // void Demo()
        // {
        //     using var aes = Aes.Create();
        //
        //     aes.Key = System.Text.Encoding.UTF8.GetBytes("a");
        //     aes.IV = System.Text.Encoding.UTF8.GetBytes("a");
        //
        // }
        
        public static IEnumerator GetTokenAsync(string apiKey, string clientSecret, Action<TokenInfo> onTokenComplete)
        {
            using var request = new UnityWebRequest("https://aip.baidubce.com/oauth/2.0/token", "POST");

            var data = System.Text.Encoding.UTF8.GetBytes($"grant_type=client_credentials&client_id={apiKey}&client_secret={clientSecret}");

            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");


            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var token = JsonConvert.DeserializeObject<TokenInfo>(request.downloadHandler.text);

                if (token is null || string.IsNullOrEmpty(token.AccessToKen))
                {
                    Debug.LogError($"未获取到token，可能api响应数据格式已更新！raw content:\n{request.downloadHandler.text}");
                    yield break;
                }

                onTokenComplete?.Invoke(token);
            }
            else
            {
                Debug.LogError($"{request.error}");
            }
        }

        [Serializable]
        public class TokenInfo
        {
            [JsonProperty(propertyName: "access_token")]
            public string AccessToKen { get; set; }
        }
    }
}