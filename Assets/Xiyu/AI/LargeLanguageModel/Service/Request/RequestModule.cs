using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xiyu.AI.LargeLanguageModel.Service.Request
{
    [Serializable]
    public class RequestModule : SerializeParameterModule
    {
        [SerializeField] private List<Message> messages;
        [SerializeField] [Range(0, 1)] private float temperature = 0.95F;
        [SerializeField] [Range(2, 1024)] private int maxOutputToken = 128;
        [SerializeField] private List<string> stop;
        [SerializeField] [Range(0, 1)] private float topP = 0.7F;
        [SerializeField] [TextArea(5, 15)] private string system;
        [SerializeField] private string userId;


        [SerializeField] private string responseFormat = "text";


        public List<Message> Messages
        {
            get => messages;
            set => messages = value;
        }

        public float Temperature
        {
            get => temperature;
            set => temperature = value;
        }

        public int MaxOutputToken
        {
            get => maxOutputToken;
            set => maxOutputToken = value;
        }

        public List<string> Stop
        {
            get => stop;
            set => stop = value;
        }

        public float TopP
        {
            get => topP;
            set => topP = value;
        }

        public string System
        {
            get => system;
            set => system = value;
        }

        public string UserId
        {
            get => userId;
            set => userId = value;
        }

        public string ResponseFormat
        {
            get => responseFormat;
            set => responseFormat = value;
        }


        public override bool IsDefault()
        {
            throw new NotImplementedException("主模块配置中的参数是全模型通用的（包含）");
        }
    }
}