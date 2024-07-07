using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using Xiyu.AIChat.LargeLanguageModel;
using Xiyu.AIChat.LargeLanguageModel.Chat.ERNIE;
using Xiyu.AIChat.LargeLanguageModel.Chat.ERNIE.ERNIE_Character_8K;
using Xiyu.GameFunction.BarrageComments;
using Xiyu.GameFunction.InputComponent;

namespace Xiyu.AIChat
{
    public class MessageProcessor : MonoBehaviour
    {
        [SerializeField] private LargeLanguageModel<RequestBody, ResponseResult> largeLanguageModel;

        [Tooltip("在提问时追加提示信息")] [SerializeField] [TextArea(5, 10)]
        private string prompt;

        public int Emotion { get; private set; } = 100;


        private bool _isLoading;

        private ResponseResult _responseResult;

        private void Awake()
        {
            if (largeLanguageModel == null)
            {
                largeLanguageModel = FindObjectOfType<LargeLanguageModel<RequestBody, ResponseResult>>();
            }

            // 注册弹幕发送事件
            FindObjectOfType<BulletCommentsScreenView>().OnBulletCommentSubmitEventHandler += (content, waitForSecond) =>
            {
                StartCoroutine(OnOnBulletCommentSubmitEventHandler(content, waitForSecond));
            };

            // 在请求发送时
            largeLanguageModel.OnSendRequestEventHandler += _ =>
            {
                _isLoading = true;
                OutputTextManager.Instance.LoadingAsync(() => !_isLoading);
            };

            // 在请求返回时
            largeLanguageModel.OnReceiveResponseEventHandler += result =>
            {
                _isLoading = false;
                largeLanguageModel.RequestBody.Messages.Add(new Message(RoleType.Assistant, result.Result));
            };
        }

        private void Start()
        {
            StartCoroutine(OnOnBulletCommentSubmitEventHandler(JsonConvert.SerializeObject(new UserSend
            {
                Role = "旁白",
                Time = "中午",
                Scene = "学校走廊",
                Message = "(中午最后一节课爱实拉着真奈美的手走向食堂)"
            }), 0));
        }

        public class UserSend
        {
            public string Role { get; set; }
            public string Time { get; set; }
            public string Scene { get; set; }
            public string Message { get; set; }
        }

        // 弹幕消息发生时
        private IEnumerator OnOnBulletCommentSubmitEventHandler(string content, float waitForSecond)
        {
            yield return new WaitForSeconds(waitForSecond);
            _isLoading = true;

            var message = string.IsNullOrEmpty(prompt) ? content : $"(提示:{prompt})\n{content}";

            largeLanguageModel.RequestBody.Messages.Add(new Message(RoleType.User, message));

            yield return largeLanguageModel.Request(result => _responseResult = result, Debug.LogError);

            yield return OutputTextManager.Instance.PrintTextCoroutine(_responseResult.Result);
        }
    }
}