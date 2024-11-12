using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Xiyu.AI.Client;
using Xiyu.AI.LargeLanguageModel;
using Xiyu.AI.LargeLanguageModel.Service.Request;
using Xiyu.AI.LargeLanguageModel.Service.Response;
using Xiyu.AI.Prompt.NewPromptCenter;
using Xiyu.AI.TextToSpeech;
using Xiyu.ArtificialIntelligence;
using Xiyu.LoggerSystem;
using Xiyu.VirtualLiveRoom.Component;
using Xiyu.VirtualLiveRoom.Component.DanmuItem.Data;

namespace Xiyu.VirtualLiveRoom.View.LiveRoomView
{
    public class ChatProcessor : MonoBehaviour, IEnumerable<Message>
    {
        [SerializeField] private DanmuController danmuController;
        [SerializeField] private SubtitlesBar subtitlesBar;

        [SerializeField] private AudioSource audioSource;

        private readonly LinkedList<Message> _chatHistory = new();

        private readonly TextToSpeechBody _textToSpeechBody = new();

        // public event Func<DanmuData, Message, UniTaskVoid> OnCallbackDanmuMessage;

        private bool _firstSend;

        private LLM _service;

        private void Awake()
        {
            // DEBUG "7f92f8afb8ec43bf81429cc1c9199cb1"
            // _textToSpeechBody.ReferenceId = "ba1a1f11162241a3b2a6a61724126be5";
            _textToSpeechBody.ReferenceId = "ba1a1f11162241a3b2a6a61724126be5";

            var auth = new IamAuthenticate(AuthenticateManager.AuthenticateElectronAuth.AccessKey, AuthenticateManager.AuthenticateElectronAuth.SecretKey);
            _service = new LLM(auth, new Uri("https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/ernie-char-8k"), HttpMethod.Post)
            {
                RequestResource = new RequestResource
                {
                    RequestRequestModule = new RequestModule
                    {
                        Messages = new List<Message>()
                    },
                    SearchForModule = new SearchForModule
                    {
                        DisableSearch = false, EnableTrace = false, EnableCitation = false
                    }
                }
            };

            // DanmuController.OnDanmuSend += data => DanmuControllerOnOnDanmuSend(data).Forget();
        }

        public async UniTask<Message> SendWebMessageAsync(DanmuData danmuData)
        {
            // 可能会加载上一次的弹幕
            if (!_firstSend)
            {
                _firstSend = true;
                var prompt = PromptRequest.LastRequestPrompt;

                var sendMessage = prompt.GetPromptString(("player", User.UserName), ("message", danmuData.DanmuContent.Content));

                AppendMessage(Message.RoleType.User, sendMessage);
            }
            else
            {
                AppendMessage(Message.RoleType.User, danmuData.DanmuContent.Content);
            }

            // 等待回复完成
            return await SendMessageAsync(danmuData);
        }

        public async UniTask SendSpeechAsync(DanmuData danmuData, Message message, CancellationToken cancellationToken)
        {
            _textToSpeechBody.Text = message.Content;
            try
            {
                var audioClip = await Service.TextToSpeech(_textToSpeechBody, 1.5F, cancellationToken);
                audioSource.clip = audioClip;
                audioSource.time = 0;
                audioSource.Play();
            }
            catch (OperationCanceledException)
            {
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                LoggerManager.Instance.LogError("语言合成失败！（超时）");
            }
        }

        public async UniTask Print(Message message)
        {
            await subtitlesBar.Print(message.Content);
        }

        public void MessageBoxInteractable(bool active)
        {
            danmuController.MessageSender.MessageBox.Interactable = active;
        }

        private async UniTask<Message> SendMessageAsync(DanmuData danmuData)
        {
            var messageList = _service.RequestResource.RequestRequestModule.Messages;
            if (messageList.Count >= 10)
            {
                _service.RequestResource.RequestRequestModule.Messages = messageList.TakeLast(10).ToList();
            }

            var responseModule = await _service.RequestAsync<ResponseModule>(new RequestOptions
            {
                HeaderParameters = new Multimap<string, string> { { "Content-Type", "application/json" } },
                QueryParameters = new Multimap<string, string>()
            });

            if (string.IsNullOrEmpty(responseModule.Result))
            {
                await LoggerManager.Instance.LogInfoAsync($"<color=yellow>{Aishi.Name}</color>未回复：{responseModule.Error.ErrorMessage}");
                return null;
            }

            AppendMessage(Message.RoleType.Assistant, responseModule.Result);

            // OnCallbackDanmuMessage?.Invoke(danmuData, _chatHistory.Last.Value).Forget();

            return _chatHistory.Last.Value;
        }

        private void AppendMessage(Message.RoleType role, string content)
        {
            var message = new Message(role, content);
            _chatHistory.AddLast(message);

            _service.RequestResource.RequestRequestModule.Messages.Add(message);
        }

        public IEnumerator<Message> GetEnumerator()
        {
            foreach (var message in _chatHistory)
            {
                yield return message;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}