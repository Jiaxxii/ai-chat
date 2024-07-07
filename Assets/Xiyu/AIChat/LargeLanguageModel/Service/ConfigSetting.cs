using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using Xiyu.AIChat.Client;

namespace Xiyu.AIChat.LargeLanguageModel.Service
{
    public abstract class ConfigSetting<TRequestBody> : MonoBehaviour, IServiceRequest<TRequestBody>, IJsonConvertJss
    {
        [SerializeField] private string apiKey;
        [SerializeField] private string clientSecret;
        [SerializeField] private string url;
        [SerializeField] private HttpMethod httpMethod = HttpMethod.Post;
        [SerializeField] private TRequestBody body;

        protected KeyValuePair<string, string> TokenQueryString;

        public string Url
        {
            get => url;
            protected set => url = value;
        }

        public string Method => httpMethod.ToString().ToUpper();

        public virtual JsonSerializerSettings RequestBodyJss { get; } = new()
        {
            Formatting = Formatting.None,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };

        public virtual JsonSerializerSettings ResponseJss { get; } = new()
        {
            Formatting = Formatting.None,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };


        public TRequestBody Body
        {
            get => body;
            set => body = value;
        }

        protected virtual void Awake()
        {
            StartCoroutine(AccessToken.GetTokenAsync(apiKey, clientSecret, info => TokenQueryString = new KeyValuePair<string, string>("access_token", info.AccessToKen)));
        }

        public virtual RequestOptions RequestOptions { get; private set; }
    }
}