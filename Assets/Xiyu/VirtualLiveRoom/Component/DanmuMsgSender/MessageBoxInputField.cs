using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Xiyu.Constant;

namespace Xiyu.VirtualLiveRoom.Component.DanmuMsgSender
{
    public class MessageBoxInputField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField messageInputField;
        [SerializeField] private Button messageSendButton;

        public event UnityAction<string> OnMessageSend;

        public bool Interactable
        {
            get => messageSendButton.interactable;
            set => messageSendButton.interactable = value;
        }


        public Func<string, bool> SubmitCheck { get; set; } = GameConstant.DefaultDanmuMessageSendBoxSubmitCheck;

        public string Content
        {
            get => messageInputField.text;
            set => messageInputField.text = value;
        }


        private void Start()
        {
            // messageInputField.onSubmit.AddListener(MessageSend);
            messageSendButton.onClick.AddListener(() => MessageSend(Content));
        }


        private void MessageSend(string msg)
        {
            if (!SubmitCheck.Invoke(msg))
            {
                Preview("哼！你不能发这条弹幕~");
                GUIUtility.systemCopyBuffer = string.IsNullOrEmpty(msg) ? "你想发的弹幕是你不想发的内容" : msg.Substring(0, GameConstant.MaxDanmuMessageLength);
                return;
            }

            OnMessageSend?.Invoke($"<nobr>{msg}");
#if !UNITY_EDITOR
            messageInputField.text = string.Empty;
#endif
            messageInputField.Select();
        }


        public void Preview(string placeholder)
        {
            messageInputField.text = string.Empty;
            messageInputField.placeholder.GetComponent<TextMeshProUGUI>().text = placeholder;
        }
    }
}