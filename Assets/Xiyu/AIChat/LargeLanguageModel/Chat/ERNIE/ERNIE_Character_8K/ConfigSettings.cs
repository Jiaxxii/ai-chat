using System;
using System.Collections.Generic;
using UnityEngine;
using Xiyu.AIChat.LargeLanguageModel.Service;

namespace Xiyu.AIChat.LargeLanguageModel.Chat.ERNIE.ERNIE_Character_8K
{
    [RequireComponent(typeof(Service))]
    public class ConfigSettings : ConfigSetting<RequestBody>
    {
        protected override void Awake()
        {
            Url = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/ernie-char-8k";
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
    }


    [Serializable]
    public class Message
    {
        [SerializeField] private RoleType roleType;
        [SerializeField] [TextArea(3, 5)] private string content;

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