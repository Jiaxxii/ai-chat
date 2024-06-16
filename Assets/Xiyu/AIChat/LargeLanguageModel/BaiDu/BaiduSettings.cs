using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Xiyu.AIChat.LargeLanguageModel.BaiDu
{
    [RequireComponent(typeof(ChatBaiDu))]
    public class BaiduSettings : MonoBehaviour
    {
        #region 参数定义

        [Header("填写应用的API Key")] [SerializeField]
        private string apiKey = string.Empty;

        public string ApiKey
        {
            get => apiKey;
            set => apiKey = value;
        }


        [Header("填写应用的Secret Key")] [SerializeField]
        private string clientSecret = string.Empty;

        public string ClientSecret
        {
            get => clientSecret;
            set => clientSecret = value;
        }

        /// <summary>
        /// 是否从服务器获取token
        /// </summary>
        [SerializeField] private bool isGetTokenFromServer = true;


        [SerializeField] private string toKen = string.Empty;

        public string ToKen
        {
            get => toKen;
            set => toKen = value;
        }

        // /// <summary>
        // /// 获取Token的地址
        // /// </summary>
        // [SerializeField] private string toKenAuthorizeURL = "https://aip.baidubce.com/oauth/2.0/token";

        #endregion


        private void Awake()
        {
            if (isGetTokenFromServer)
            {
                StartCoroutine(GeToken(apiKey,clientSecret,callBackToKen => ToKen = callBackToKen));
            }
        }
        

        public static IEnumerator GeToken(string apiKey, string clientSecret, Action<string> onTokenComplete)
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

                onTokenComplete?.Invoke(token.AccessToKen);
            }
            else
            {
                Debug.LogError($"{request.error}");
            }
        }

        /// <summary>
        /// 返回的token
        /// </summary>
        [System.Serializable]
        public class TokenInfo
        {
            [JsonProperty(propertyName: "access_token")]
            public string AccessToKen { get; set; }
        }
    }
}