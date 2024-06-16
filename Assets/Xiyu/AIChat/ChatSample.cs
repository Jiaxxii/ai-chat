using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.AIChat.LargeLanguageModel;
using Xiyu.AIChat.LargeLanguageModel.BaiDu;

namespace Xiyu.AIChat
{
    public class ChatSample : MonoBehaviour
    {
        [SerializeField] private LLM chatSettings;

        [SerializeField] private Button sendMessageButton;
        [SerializeField] private TMP_InputField inputField;

        [SerializeField] private TextMeshProUGUI outPutTextMeshProUGUI;
        [SerializeField] [Range(0.001F, 0.5F)] private float nextCharTime = 0.002F;

        private void Awake()
        {
            sendMessageButton.onClick.AddListener(() =>
            {
                SendMessage();
                inputField.onSubmit.Invoke(inputField.text);
            });
            inputField.onSubmit.AddListener(_ =>
            {
                SendMessage();
                inputField.text = string.Empty;
            });
        }

        // private IEnumerator Start()
        // {
        //     yield return new WaitForSeconds(2F);
        //     SendData("复华市 天气晴天 早上7：23，我们在校园中相遇……", message => StartCoroutine(PrintText(message)));
        // }

        private void SendMessage()
        {
            SendData(inputField.text, message => StartCoroutine(PrintText(message)));
        }

        private IEnumerator PrintText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                yield break;
            }

            var sb = new StringBuilder(text.Length).Append("  ");
            var waitForSecond = new WaitForSeconds(nextCharTime);

            const string end = "……";
            foreach (var str in text)
            {
                sb.Append(str);
                outPutTextMeshProUGUI.text = $"{sb}{end}";
                if (str == ' ')
                {
                    continue;
                }

                yield return waitForSecond;
            }

            outPutTextMeshProUGUI.text = sb.ToString();
        }

        public void SendData(string message, Action<string> onGetMessage)
        {
            if (message is null || string.IsNullOrEmpty(message))
                return;

            //发送数据
            chatSettings.PostMessage(new Message(Message.RoleType.user, message), onGetMessage);
        }
    }
}