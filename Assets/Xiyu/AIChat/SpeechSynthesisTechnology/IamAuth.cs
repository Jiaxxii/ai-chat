using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web;
using Xiyu.AIChat.Client;

namespace Xiyu.AIChat.SpeechSynthesisTechnology
{
    public class IamAuth
    {
        private static readonly HashSet<string> BceHeaderToSign = new() { HeaderHost, HeaderContentMd5, HeaderContentLength };

        private const string HeaderHost = "host";
        private const string HeaderContentMd5 = "content-md5";
        private const string HeaderContentLength = "content-length";
        private const string HeaderContentType = "content-type";

        public string IamAk { get; set; }
        public string IamSk { get; set; }

        private const string Charset = "UTF-8";


        private const string BcePrefix = "x-bce-";


        public int SignExpireInSeconds { get; set; }


        public bool ApplyToParams(RequestOptions requestOptions, HttpMethod method, Uri uri)
        {
            if (string.IsNullOrEmpty(IamAk) || string.IsNullOrEmpty(IamSk))
            {
                return false;
            }

            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            requestOptions.HeaderParameters.Add(HeaderHost, uri.Host);

            var sign = Sign(requestOptions.QueryParameters, requestOptions.HeaderParameters, timestamp, method.Method, uri.AbsolutePath);

            requestOptions.HeaderParameters.Add("Authorization", sign);
            return true;
        }

        private string Sign(Multimap<string, string> queryParams, Multimap<string, string> headerParams, string timestamp, string methodMethod,
            string uriAbsolutePath)
        {
            // 前缀字符串
            var authStringPrefix = $"bce-auth-v1/{IamAk}/{timestamp}/{SignExpireInSeconds}";
            var signingKey = HmacSha256(IamSk, authStringPrefix);
            var canonicalUri = HttpUtility.UrlEncode(!uriAbsolutePath.StartsWith('/') ? string.Concat('/', uriAbsolutePath) : uriAbsolutePath).Replace("%2f", "/");

            var canonicalQuery = GetCanonicalQuery(queryParams);
            var canonicalHeaders = GetCanonicalHeaders(headerParams);

            var canonicalRequest = new List<string>
            {
                methodMethod.ToUpperInvariant(),
                canonicalUri,
                canonicalQuery,
                canonicalHeaders[1]
            };

            var signature = HmacSha256(signingKey, string.Join("\n", canonicalRequest));
            return $"{authStringPrefix}/{canonicalHeaders[0]}/{signature}";
        }

        private static string[] GetCanonicalHeaders(Multimap<string, string> headerParams)
        {
            var signed = headerParams
                .Where(h =>
                    // 选择使用以 “x-bce-" 开头的头部 或  _bceHeaderToSign(需要被编码) 中包含的头部
                    h.Key.StartsWith(BcePrefix, StringComparison.OrdinalIgnoreCase) || BceHeaderToSign.Contains(h.Key.ToLowerInvariant()))
                .OrderBy(h => h.Key).ToArray();

            var signedHeaders = new string[signed.Length];
            var headerEntries = new List<string>();

            for (var i = 0; i < signed.Length; i++)
            {
                signedHeaders[i] = signed[i].Key.ToLowerInvariant();
                headerEntries.AddRange(signed[i].Value.Select(value => $"{HttpUtility.UrlEncode(signedHeaders[i])}:{HttpUtility.UrlEncode(value.Trim())}"));
            }


            return new[] { string.Join(';', signedHeaders), string.Join("\n", headerEntries) };
        }

        // Multimap = Dictionary<string,IList<string>>
        private static string GetCanonicalQuery(Multimap<string, string> parameters)
        {
            
            var queryParams = parameters
                // 返回非““authorization””的参数 
                .Where(p => !string.Equals(p.Key, "authorization", StringComparison.OrdinalIgnoreCase))
                // 按照 ASCLL 进行排序 （小到大）
                .OrderBy(p => p.Key)
                .SelectMany(p => p.Value.Select(k => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(k)}"));

            return string.Join('&', queryParams);
        }

        private static string HmacSha256(string key, string data)
        {
            using var sha256 = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(key));

            var buffer = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));

            return Convert.ToString(buffer).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}