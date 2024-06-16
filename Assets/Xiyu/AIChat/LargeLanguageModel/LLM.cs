using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Xiyu.AIChat.LargeLanguageModel.BaiDu;

namespace Xiyu.AIChat.LargeLanguageModel
{
    public abstract class LLM : MonoBehaviour
    {
        /// <summary>
        /// API 地址
        /// </summary>
        protected string Url;

        [Tooltip("请求body数据")] [SerializeField] protected Config config;


        protected readonly Stopwatch Stopwatch = new();


        public virtual void PostMessage(RequestData requestData, Action<string> onReceivedReply)
        {
            StartCoroutine(Request(requestData, onReceivedReply));
        }

        public virtual void PostMessage(Message message, Action<string> onReceivedReply)
        {
            StartCoroutine(Request(message, onReceivedReply));
        }

        public virtual void PostMessage(Message.RoleType roleType, string message, Action<string> onReceivedReply)
        {
            StartCoroutine(Request(roleType, message, onReceivedReply));
        }


        public abstract IEnumerator Request(Message.RoleType roleType, string message, Action<string> onReceivedReply);
        public abstract IEnumerator Request(Message message, Action<string> onReceivedReply);

        public abstract IEnumerator Request(RequestData requestData, Action<string> onReceivedReply);
    }
}