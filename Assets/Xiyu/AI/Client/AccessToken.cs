using System;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine.Networking;

namespace Xiyu.AI.Client
{
    public class AccessToken : Authenticate
    {
        public override AuthenticateType AuthenticateType => AuthenticateType.AccessToken;
        

        private readonly KeyValuePair<string, List<string>> _contentType = new("Content-Type", new List<string> { "application/json" });
        private readonly KeyValuePair<string, List<string>> _grantType = new("grant_type", new List<string> { "client_credentials" });
        private readonly KeyValuePair<string, List<string>> _clientId;
        private readonly KeyValuePair<string, List<string>> _clientSecret;
        
        

        public AccessToken(string clientId, string clientSecret)
        {
            _clientId = new KeyValuePair<string, List<string>>("client_id", new List<string> { clientId });
            _clientSecret = new KeyValuePair<string, List<string>>("client_secret", new List<string> { clientSecret });
        }

        public override UnityWebRequest ConfigureWebRequest(RequestOptions requestOptions, HttpMethod method, Uri uri)
        {

            requestOptions.HeaderParameters.OverAdd(_contentType.Key,_contentType.Value);

            requestOptions.QueryParameters.TryAdd(_grantType.Key, _grantType.Value);
            requestOptions.QueryParameters.TryAdd(_clientId.Key, _clientId.Value);
            requestOptions.QueryParameters.TryAdd(_clientSecret.Key, _clientSecret.Value);
            

            return base.ConfigureWebRequest(requestOptions, method, uri);
        }
    }
}