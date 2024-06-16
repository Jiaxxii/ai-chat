using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Xiyu.AIChat
{
    public class Test : MonoBehaviour
    {
        [Header("填写应用的API Key")] [SerializeField]
        private string apiKey = string.Empty;


        [Header("填写应用的Secret Key")] [SerializeField]
        private string clientSecret = string.Empty;


        private IEnumerator Start()
        {
            // F:\Unity\Editor\2022.3.14f1c1\Editor\Audio\你好，我叫爱实！.mp3
            using var www = UnityWebRequestMultimedia.GetAudioClip($@"F:\Unity\Editor\2022.3.14f1c1\Editor\Audio\你好，我叫爱实！.mp3", AudioType.MPEG);

            yield return www.SendWebRequest();
            
            AudioClip downloadedClip = DownloadHandlerAudioClip.GetContent(www);  
            
            Debug.Log(downloadedClip.length);



            // var token = string.Empty;
            // yield return BaiduSettings.GeToken(apiKey, clientSecret, resultToken => token = resultToken);
            //
            //
            // var form = new WWWForm();
            // form.AddField("tex", WebUtility.UrlEncode("2024年6月12日22:14:07"));
            // form.AddField("tok", token);
            // form.AddField("cuid", "x3Xp6OKERfotQw690PcPPll6eXJkPFhE");
            // form.AddField("ctp", "1");
            // form.AddField("lan", "zh");
            //
            // using var request = UnityWebRequest.Post("https://tsn.baidu.com/text2audio", form);
            //
            // request.SetRequestHeader("Content-Type", "audio/mp3");
            //
            // request.uploadHandler = new UploadHandlerRaw(form.data);
            // request.downloadHandler = new DownloadHandlerBuffer();
            //
            // yield return request.SendWebRequest();
            //
            //
            // if (request.result == UnityWebRequest.Result.Success)
            // {
            //     var fileData = request.downloadHandler.data;
            //     var savePath = Path.Combine(Application.dataPath, "Resources", "Audios", "temp.mp3");
            //     File.WriteAllBytes(savePath, fileData);
            // }
        }


        public class TextRequestData
        {
            [JsonProperty(PropertyName = "tex")] public string Text { get; set; }

            // ReSharper disable once StringLiteralTypo
            [JsonProperty(PropertyName = "cuid")] public string CuID { get; set; }


            // ReSharper disable once InconsistentNaming
            [JsonProperty(PropertyName = "ctp")] public int CTP { get; set; } = 1;


            [JsonProperty(PropertyName = "lan")] public string Lan { get; set; } = "zh";
        }
    }
}