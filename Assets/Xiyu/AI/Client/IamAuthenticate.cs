using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using UnityEngine.Networking;
using Xiyu.ArtificialIntelligence;

namespace Xiyu.AI.Client
{
    public class IamAuthenticate : Authenticate
    {
        // private const string Charset = "UTF-8";
        private const string HeaderHost = "host";
        private const string HeaderContentMd5 = "content-md5";
        private const string HeaderContentLength = "content-length";
        // private const string HeaderContentType = "content-type";

        private static readonly HashSet<string> BceHeaderToSign = new() { HeaderHost, HeaderContentMd5, HeaderContentLength };

        private const string BcePrefix = "x-bce-";

        public override AuthenticateType AuthenticateType => AuthenticateType.IAM;

        public string IamAk { get; }

        public string IamSk { get; }

        public IamAuthenticate(string iamAk, string iamSk)
        {
            IamAk = iamAk;
            IamSk = iamSk;
        }

        public int SignExpireInSeconds { get; set; } = 1800;

        private static string Md5(string data)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(data);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        private static string HmacSha256(string key, string data)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private static string GetCanonicalUri(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = $"/{path}";
            }

            return HttpUtility.UrlEncode(path).Replace("%2f", "/");
        }

        private static string GetCanonicalQuery(Multimap<string, string> parameters)
        {
            var queryParams = parameters
                .Where(p => !string.Equals(p.Key, "authorization", StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Key)
                .SelectMany(p => p.Value.Select(k => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(k)}"));
            return string.Join("&", queryParams);
        }

        private static string[] GetCanonicalHeaders(Multimap<string, string> headers)
        {
            var signeds = headers
                .Where(h => h.Key.StartsWith(BcePrefix, StringComparison.OrdinalIgnoreCase) ||
                            BceHeaderToSign.Contains(h.Key.ToLowerInvariant()))
                .OrderBy(h => h.Key);
            var signedHeaders = signeds.Select(k => k.Key.ToLowerInvariant());
            var headerEntries = signeds.SelectMany(h =>
            {
                return h.Value.Select(k =>
                    $"{HttpUtility.UrlEncode(h.Key.ToLowerInvariant())}:{HttpUtility.UrlEncode(k.Trim())}");
            });
            return new[] { string.Join(";", signedHeaders), string.Join("\n", headerEntries) };
        }

        private string Sign(Multimap<string, string> queryParams, Multimap<string, string> headerParams,
            string timestamp, string method, string path)
        {
            // string path = uri.AbsolutePath;
            string authStringPrefix = $"bce-auth-v1/{IamAk}/{timestamp}/{SignExpireInSeconds}";
            string signingKey = HmacSha256(IamSk, authStringPrefix);
            string canonicalUri = GetCanonicalUri(path);
            string canonicalQuery = GetCanonicalQuery(queryParams);
            string[] canonicalHeaders = GetCanonicalHeaders(headerParams);

            List<string> canonicalRequest = new List<string>
            {
                method.ToUpperInvariant(),
                canonicalUri,
                canonicalQuery,
                canonicalHeaders[1]
            };

            string signature = HmacSha256(signingKey, string.Join("\n", canonicalRequest));
            return $"{authStringPrefix}/{canonicalHeaders[0]}/{signature}";
        }

        private static string GetCanonicalTime()
        {
            // .ToString("yyyy-MM-ddTHH:mm:ssZ")
            return (DateTime.UtcNow - TimeSpan.FromSeconds(0)).ToString("yyyy-MM-ddTHH:mm:ssZ");
        }


        public override UnityWebRequest ConfigureWebRequest(RequestOptions requestOptions, HttpMethod method, Uri uri)
        {
            if (string.IsNullOrEmpty(IamAk) || string.IsNullOrEmpty(IamSk))
            {
                return null;
            }

            var timestamp = GetCanonicalTime();
            requestOptions.HeaderParameters.Add(HeaderHost, uri.Host);
            var sign = Sign(requestOptions.QueryParameters, requestOptions.HeaderParameters, timestamp,
                method.Method, uri.AbsolutePath);
            requestOptions.HeaderParameters.Add("Authorization", sign);

            return base.ConfigureWebRequest(requestOptions, method, uri);
        }

        public override UnityWebRequest ConfigureWebRequest(Multimap<string, string> queryParameters, Multimap<string, string> headerParameters, HttpMethod method, Uri uri)
        {
            if (string.IsNullOrEmpty(IamAk) || string.IsNullOrEmpty(IamSk))
            {
                return null;
            }

            var timestamp = GetCanonicalTime();
            headerParameters.Add(HeaderHost, uri.Host);
            var sign = Sign(queryParameters, headerParameters, timestamp,
                method.Method, uri.AbsolutePath);
            headerParameters.Add("Authorization", sign);

            return base.ConfigureWebRequest(queryParameters, headerParameters, method, uri);
        }
    }
}