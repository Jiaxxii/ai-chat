using System;
using UnityEngine;

namespace Xiyu.AI.LargeLanguageModel.Service.Request
{

    [Serializable]
    public class Message
    {
        [SerializeField] private RoleType role;
        [SerializeField] [TextArea(3, 10)] private string content;
        
        public string Role
        {
            get => role.ToString().ToLowerInvariant();
            set => role = Enum.Parse<RoleType>(value,true);
        }
        
        public string Content
        {
            get => content;
            set => content = value;
        }
        
        public enum RoleType
        {
            User,
            Assistant
        }
    }
}