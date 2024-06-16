using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Xiyu.AIChat.LargeLanguageModel.BaiDu
{
    [RequireComponent(typeof(BaiduSettings))]
    public class ChatBaiDu : LLM
    {
        [SerializeField] private BaiduSettings baiduSettings;

        /// <summary>
        /// 历史对话
        /// </summary>
        [SerializeField] private List<Message> history = new();

        public List<Message> GetHistory(Message message)
        {
            history.Add(message);
            return history;
        }

        private void Awake()
        {
            baiduSettings = GetComponent<BaiduSettings>();
            Url = GetEndPointUrl();
        }

        public override IEnumerator Request(Message.RoleType roleType, string message, Action<string> onReceivedReply)
        {
            yield return Request(new Message(roleType, message), onReceivedReply);
        }
        public override IEnumerator Request(Message message, Action<string> onReceivedReply)
        {
            var requestData = new RequestData
            {
                Messages = GetHistory(message),
                Stream = config.Stream,
                Temperature = config.Temperature,
                TopP = config.TopP,
                PenaltyScore = config.PenaltyScore,
                System = config.System,
                Stop = config.Stop,
                MaxOutputTokens = config.MaxOutputTokens,
                UserID = config.UserID
            };

            yield return Request(requestData, onReceivedReply);
        }
        public override IEnumerator Request(RequestData requestData, Action<string> onReceivedReply)
        {
            Stopwatch.Restart();

            var postUrl = $"{Url}?access_token={baiduSettings.ToKen}";


            using var request = new UnityWebRequest(postUrl, "POST");


            var jsonData = JsonConvert.SerializeObject(requestData);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonData);

            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                var msg = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<ResponseData>(msg);

                //历史记录
                history.Add(new Message(Message.RoleType.assistant, response.Result));

                //回调
                onReceivedReply?.Invoke(response.Result);
            }


            Stopwatch.Stop();
            Debug.Log("chat百度-耗时：" + Stopwatch.Elapsed.TotalSeconds);
        }

        /// <summary>
        /// 获取资源路径 TODO
        /// </summary>
        private static string GetEndPointUrl()
        {
            return "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/ernie-char-8k";
        }
    }
}