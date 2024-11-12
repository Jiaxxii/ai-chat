using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Xiyu.AI.Client;
using Xiyu.AI.LargeLanguageModel;
using Xiyu.AI.LargeLanguageModel.Service.Request;
using Xiyu.AI.LargeLanguageModel.Service.Response;
using Xiyu.ArtificialIntelligence;
using Xiyu.Expand;
using Xiyu.GameFunction.GeometricTransformations;
using Xiyu.LoggerSystem;
using Xiyu.VirtualLiveRoom.AudioSystem;
using Xiyu.VirtualLiveRoom.Component.BarrageComments;
using Xiyu.VirtualLiveRoom.Component.Character;
using Xiyu.VirtualLiveRoom.Component.Character.Emotion;
using Xiyu.VirtualLiveRoom.Component.DanmuItem.Data;
using CharacterController = Xiyu.VirtualLiveRoom.Component.Character.CharacterController;
using Random = UnityEngine.Random;

namespace Xiyu.VirtualLiveRoom.View.LiveRoomView
{
    public class AishiBehaviour : MonoBehaviour
    {
        [SerializeField] private ChatProcessor chatProcessor;
        [SerializeField] private BulletCommentsScreenView bulletCommentsScreenView;
        private LLM _service;

        private static readonly HashSet<CharacterBasicEmotions> Emotions = new(Enum.GetNames(typeof(CharacterBasicEmotions)).Select(Enum.Parse<CharacterBasicEmotions>));

        private IEnumerable<CharacterBasicEmotions> _result;

        private void Awake()
        {
            if (chatProcessor == null)
                chatProcessor = FindObjectOfType<ChatProcessor>();


            DanmuController.OnDanmuSend += danmu =>
            {
                SendAudioSound().Forget();
                // 刷新弹幕列表
                bulletCommentsScreenView.SendBulletComment(danmu.DanmuContent.Content);
                OnDanmuControllerSend(danmu).Forget();
            };


            var auth = new IamAuthenticate(AuthenticateManager.AuthenticateElectronAuth.AccessKey, AuthenticateManager.AuthenticateElectronAuth.SecretKey);
            _service = new LLM(auth, new Uri("https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions"), HttpMethod.Post)
            {
                RequestResource = new RequestResource
                {
                    RequestRequestModule = new RequestModule
                    {
                        Messages = new List<Message>(),
                        MaxOutputToken = 10
                    },
                    SearchForModule = new SearchForModule
                    {
                        DisableSearch = false, EnableTrace = false, EnableCitation = false
                    },
                    // PenaltyModule = new PenaltyModule{}
                }
            };
        }


        [SerializeField] private RectTransform rectTrans;


        private AishiCharacterController _characterController;

        private AishiGeomTransform _aishiGeomTransform;


        private static async UniTaskVoid SendAudioSound()
        {
            (await AudioManager.Instance.GetAudioOperatorPlayer("sound").SetClip("clip_ui_3")).SetLoop(false).Play();
        }

        // private readonly TimeoutController _timeoutController = new();

        private async UniTaskVoid OnDanmuControllerSend(DanmuData danmuData)
        {
            chatProcessor.MessageBoxInteractable(false);

            var timeConsuming = new TimeConsuming();

            // 等待消息回复
            var message = await chatProcessor.SendWebMessageAsync(danmuData);


            // Debug.Log($"<color=yellow>{Aishi.Name}</color>回复：{message.Content}");
            await LoggerManager.Instance.LogInfoAsync($"<color=yellow>{Aishi.Name}</color> 回复：{message.Content}");

            // 消息情绪分析任务
            var emotionAnalysisTask = SendEmotionAnalysis(danmuData, message);
            timeConsuming.Dispose();

            // 不需要捕获异常，SendSpeechAsync已经处理了超时
            // var timeout = TimeSpan.FromSeconds(Mathf.Clamp(message.Content.Length * 0.2F, 3F, 8F));

            // 语音合成任务 *https://fish.audio/zh-CN/ 需要梯子了
            // var speechTask = chatProcessor.SendSpeechAsync(danmuData, message, _timeoutController.Timeout(timeout));


            // await UniTask.WhenAll(emotionAnalysisTask, speechTask);


            await emotionAnalysisTask;

            await UniTask.WhenAll(
                // 执行表情变换
                ActionEmos(),
                // 打印字幕
                chatProcessor.Print(message));


            chatProcessor.MessageBoxInteractable(true);
        }


        public async UniTask InitCharacter()
        {
            _characterController = await CharacterController
                .CreateRole<AishiCharacterController>(rectTrans.sizeDelta, rectTrans, "ai", "ai_a_0004", "ai_a_0010", false);

            var geometry = _characterController.Geometry;

            geometry.Scale = new Vector3(.5F, .5F, .5F);

            var topY = -(geometry.SizeScaling.y - rectTrans.sizeDelta.y).Half();
            geometry.Position = new UnityEngine.Vector2(0, topY - rectTrans.sizeDelta.y * 0.2F);


            _aishiGeomTransform = (AishiGeomTransform)geometry;

            await EmotionsCenter.LoadSettingsAsync();

            await _characterController.Display("ai_a_0004", "ai_a_0010", 0.075F);
        }


        private async UniTask ActionEmos()
        {
            foreach (var emotion in _result)
            {
                var faceArray = EmotionsCenter.ReadOnlyDictionary[emotion];

                var faceCode = $"ai_a_{faceArray.GetRandom():0000}";

                await UniTask.WhenAll(
                    _characterController.Display("ai_a_0004", faceCode, 0.075F),
                    _aishiGeomTransform.EmotionToAction(emotion)
                );

                await UniTask.WaitForSeconds(Random.Range(1, 3F));
            }
        }

        private async UniTask SendEmotionAnalysis(DanmuData danmuData, Message message)
        {
            BuilderMessage(danmuData, message);

            var responseModule = await _service.RequestAsync<ResponseModule>(new RequestOptions
            {
                HeaderParameters = new Multimap<string, string> { { "Content-Type", "application/json" } },
                QueryParameters = new Multimap<string, string>()
            });


            await LoggerManager.Instance.LogInfoAsync($"情绪分析：{responseModule.Result}");
            _result = Emotions.Where(emo => responseModule.Result.Contains(emo.ToString()));
        }


        private void BuilderMessage(DanmuData danmuData, Message message)
        {
            var msg =
                $"用户“{danmuData.DanmuUserName.Name}”说“{danmuData.DanmuContent.Content}”主播“{Aishi.Name}”回复:“{message.Content}”{Aishi.Name}此时的情绪是以下哪几种（简要回答）？\n[{string.Join(',', Emotions.Select(emo => emo.ToString()))}]";

            _service.RequestResource.RequestRequestModule.Messages.Clear();
            _service.RequestResource.RequestRequestModule.Messages.Add(new Message(Message.RoleType.User, msg));
        }
    }
}