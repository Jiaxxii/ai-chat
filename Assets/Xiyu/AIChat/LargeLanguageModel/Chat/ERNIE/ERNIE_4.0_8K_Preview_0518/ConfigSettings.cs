using System;
using System.Collections.Generic;
using UnityEngine;
using Xiyu.AIChat.LargeLanguageModel.Service;

namespace Xiyu.AIChat.LargeLanguageModel.Chat.ERNIE.ERNIE_4._0_8K_Preview_0518
{
    [RequireComponent(typeof(Service))]
    public class ConfigSettings : ConfigSetting<RequestBody>
    {
        protected override void Awake()
        {
            Url = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions_adv_pro";
            base.Awake();
        }
    }


    [Serializable]
    public class RequestBody
    {
        [SerializeField] private List<Message> messages;

        public List<Message> Messages
        {
            get => messages;
            set => messages = value;
        }

        [SerializeField] [TextArea(7, 15)] private string system;

        public string System
        {
            get => system;
            set => system = value;
        }


        [SerializeField] private bool enableCitation;
        [SerializeField] private bool enableTrace;
        [SerializeField] [Range(2, 2048)] private int maxOutputTokens = 256;

        [SerializeField] private string responseFormat = "json_object";


        public bool EnableCitation
        {
            get => enableCitation;
            set => enableCitation = value;
        }


        public bool EnableTrace
        {
            get => enableTrace;
            set => enableTrace = value;
        }


        public int MaxOutputTokens
        {
            get => maxOutputTokens;
            set => maxOutputTokens = value;
        }


        public string ResponseFormat
        {
            get => responseFormat;
            set => responseFormat = value;
        }
    }


    [Serializable]
    public class Message
    {
        [SerializeField] private RoleType roleType;
        [SerializeField] [TextArea(3, 5)] private string content;
        [SerializeField] private string name;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public Message(RoleType roleType, string content)
        {
            this.roleType = roleType;
            this.content = content;
        }

        public string Role
        {
            get => roleType.ToString().ToLower();
            set => roleType = Enum.Parse<RoleType>(value);
        }

        public string Content
        {
            get => content;
            set => content = value;
        }
    }
}