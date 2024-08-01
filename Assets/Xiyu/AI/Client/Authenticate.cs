using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using Xiyu.ArtificialIntelligence;

namespace Xiyu.AI.Client
{
    /// <summary>
    /// 鉴权类型
    /// </summary>
    public enum AuthenticateType
    {
        /// <summary>
        /// access_token是用户的访问令牌，用于校验调用者的身份信息，确保具有要执行的操作的权限。在调用百度智能云千帆提供的 API前，必须先获取访问凭证accessToken。
        /// </summary>
        AccessToken,

        /// <summary>
        /// 使用安全认证中的Access Key（即AK）/Secret Key（即SK）进行签名计算。将HTTP请求发送到百度智能云时，需要对请求进行签名计算，以便百度智能云可以识别身份。
        /// </summary>
        IAM
    }

    /// <summary>
    /// 鉴权
    /// </summary>
    public abstract class Authenticate
    {
        /// <summary>
        /// 鉴权类型
        /// </summary>
        public abstract AuthenticateType AuthenticateType { get; }

        /// <summary>
        /// 将 "key",["v1","v2",'v2',v3] 规范化为 "key","v1,v2,v3"
        /// </summary>
        /// <param name="header"></param>
        /// <param name="urlEncode"></param>
        /// <returns></returns>
        protected static KeyValuePair<string, string> HeaderParameter(KeyValuePair<string, IList<string>> header, bool urlEncode = false)
        {
            var hashSet = new HashSet<string>();
            return new KeyValuePair<string, string>(header.Key, string.Join(',',
                header.Value.Where(v => hashSet.Add(v)).Select(v => urlEncode ? HttpUtility.UrlEncode(v) : v)));
        }

        /// <summary>
        /// ReSharper disable once InvalidXmlDocComment
        /// 将 ["key",["v1,v2,v3"],"key2",["v1,v2,v3"]] 规范化为 key=v1,v2,v3&key2=v1,v2,v3
        /// </summary>
        /// <param name="query"></param>
        /// <param name="urlEncode"></param>
        /// <returns></returns>
        protected static string QueryParameter(Multimap<string, string> query, bool urlEncode = false)
        {
            var sb = new StringBuilder();
            var index = 0;
            foreach (var keyValue in query)
            {
                index++;
                sb.Append(urlEncode ? HttpUtility.UrlEncode(keyValue.Key) : keyValue.Key)
                    .Append('=')
                    .Append(string.Join(',', keyValue.Value.Select(v => urlEncode ? HttpUtility.UrlEncode(v) : v)));
                if (index < query.Count)
                {
                    sb.Append('&');
                }
            }

            return sb.ToString();
        }
        
        public virtual UnityWebRequest ConfigureWebRequest(RequestOptions requestOptions, HttpMethod method, Uri uri)
        {
            var queryString = QueryParameter(requestOptions.QueryParameters);
            if (!string.IsNullOrEmpty(queryString))
            {
                uri = new Uri($"{uri.AbsoluteUri}?{queryString}");
            }

            var webRequest = new UnityWebRequest(uri, method.Method);

            foreach (var header in requestOptions.HeaderParameters)
            {
                var keyValue = HeaderParameter(header);
                webRequest.SetRequestHeader(keyValue.Key, keyValue.Value.Replace("%2f", "/"));
            }

            return webRequest;
        }

        public virtual UnityWebRequest ConfigureWebRequest(Multimap<string, string> queryParameters, Multimap<string, string> headerParameters, HttpMethod method, Uri uri)
        {
            var queryString = QueryParameter(queryParameters);
            if (!string.IsNullOrEmpty(queryString))
            {
                uri = new Uri($"{uri.AbsoluteUri}?{queryString}");
            }

            var webRequest = new UnityWebRequest(uri, method.Method);

            foreach (var header in headerParameters)
            {
                var keyValue = HeaderParameter(header);
                webRequest.SetRequestHeader(keyValue.Key, keyValue.Value.Replace("%2f", "/"));
            }

            return webRequest;
        }
        
        
        
        
    }
}