// using System;
// using System.Collections;
// using System.Net.Http;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Serialization;
// using UnityEngine;
// using UnityEngine.Networking;
// using Xiyu.AI.Client;
// using Xiyu.AI.LargeLanguageModel.Service.Request;
// using Xiyu.AI.LargeLanguageModel.Service.Response;
//
// namespace Xiyu.AI.LargeLanguageModel
// {
//     public class UnityLLM : MonoBehaviour
//     {
//         public UnityLLM Init(Authenticate authenticate, Uri uri, HttpMethod method, DownloadHandler downloadHandler = null)
//         {
//             Auth = authenticate;
//             Url = uri;
//             HttpMethod = method;
//             _downloadHandler = downloadHandler ?? new DownloadHandlerBuffer();
//
// #if UNITY_EDITOR
//             OnRequestFailEventHandler += Debug.LogError;
// #endif
//             return this;
//         }
//
//
//         public event Action<string> OnRequestFailEventHandler;
//
//         protected Authenticate Auth;
//         protected Uri Url;
//         protected HttpMethod HttpMethod;
//
//         private DownloadHandler _downloadHandler;
//
//         public RequestResource RequestResource { get; set; }
//         public int OutTimeSecond { get; set; } = 15;
//
//         public JsonSerializerSettings JsonSerializerSettings { get; set; } = new()
//         {
//             Formatting = Formatting.None,
//             ContractResolver = new DefaultContractResolver
//             {
//                 NamingStrategy = new SnakeCaseNamingStrategy()
//             },
//             DefaultValueHandling = DefaultValueHandling.Ignore
//         };
//
//
//         public virtual IEnumerator Request(RequestOptions requestOptions, Action<string> onSuccess)
//         {
//             using var request = SetConfigureWebRequest(requestOptions);
//
//             yield return request.SendWebRequest();
//
//
//             if (request.responseCode == 200 && request.result == UnityWebRequest.Result.Success)
//             {
//                 var resultJson = _downloadHandler.text;
//                 onSuccess?.Invoke(resultJson);
//             }
//             else
//             {
//                 OnRequestFailEventHandler?.Invoke(request.error);
//             }
//         }
//
//         public virtual IEnumerator Request<T>(RequestOptions requestOptions, Action<T> onSuccess) where T : DeserializeParameterModule
//         {
//             using var request = SetConfigureWebRequest(requestOptions);
//
//             yield return request.SendWebRequest();
//
//
//             if (request.responseCode == 200 && request.result == UnityWebRequest.Result.Success)
//             {
//                 var resultJson = _downloadHandler.text;
//
//                 var response = DeserializeParameterModule.Deserialize<T>(resultJson, JsonSerializerSettings);
//                 onSuccess?.Invoke(response);
//             }
//             else
//             {
//                 OnRequestFailEventHandler?.Invoke(request.error);
//             }
//         }
//
//
//         protected virtual UnityWebRequest SetConfigureWebRequest(RequestOptions requestOptions)
//         {
//             var request = Auth.ConfigureWebRequest(requestOptions, HttpMethod, Url);
//
// #if UNITY_EDITOR
//             var jsonContent = RequestResource.ToJson();
//
//             request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonContent));
// #else
//             request.uploadHandler = new UploadHandlerRaw(RequestResource.ToJson(Encoding.UTF8));
// #endif
//
//             request.downloadHandler = _downloadHandler;
//
//             return request;
//         }
//     }
// }