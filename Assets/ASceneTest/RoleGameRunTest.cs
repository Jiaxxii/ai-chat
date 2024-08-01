using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Xiyu.AI.Client;
using Xiyu.AI.LargeLanguageModel;
using Xiyu.AI.LargeLanguageModel.Service.Request;
using Xiyu.AI.LargeLanguageModel.Service.Response;
using Xiyu.AI.Prompt;
using Xiyu.ArtificialIntelligence;
using Xiyu.Constant;
using Xiyu.Expand;
using Xiyu.GameFunction.CharacterComponent;
using Xiyu.GameFunction.GeometricTransformations;
using Xiyu.GameFunction.SceneView;
using Xiyu.LoggerSystem;

namespace ASceneTest
{
    public class RoleGameRunTest : MonoBehaviour
    {
        [SerializeField] [Range(0F, 1F)] private float value;
        [SerializeField] private TextAsset textAsset;

        [SerializeField] private CharacterContentRoot characterContentRoot;

        [SerializeField] private RequestResource requestResource;
        private ICharacterControl _contentRoot;

        // [SerializeField] private float v1, v2 = 30f, v3 = 90f;
        // [SerializeField] private int vibrato = 10;
        //
        // [SerializeField] private bool fadeOut;
        // [SerializeField] private Ease Ease;

        private async void Awake()
        {
            await PromptCenterProcessing.Init(default);
            LoggerManager.Instance.ThrowFail("自动异常抛出");
        }

        private IEnumerator Start()
        {
            if (characterContentRoot is not null)
            {
                _contentRoot = characterContentRoot;
                yield return _contentRoot.Init("ai", JObject.Parse(textAsset.text), true);
            }
            else
            {
                yield return CharacterContentRoot.CreateRole<CharacterContentRoot>(
                    transform,
                    "ai",
                    JObject.Parse(textAsset.text),
                    control => _contentRoot = control,
                    true);
            }


            yield return _contentRoot.Display("ai_a_0006", "ai_a_0026");

            if (_contentRoot.Geometry is not GeomTransforms gt)
            {
                throw new InvalidCastException();
            }


            gt.SetScale(0.5F)
                .MoveTo(ViewHorizontalAlign.Center, ViewVerticalAlign.Top)
                .Offset(new Vector2(0, -GameInsView.ScreenSize.y * (1 - value)));

            // .DoFade(new Color(1, 1, 1, 0.5F), 5);
            // yield return _contentRoot.DisplayFaceFade("ai_a_0028");
            yield return new WaitForSeconds(1);
            yield return gt.WaitForDoJump();

            var player = (name: "player", value: GameConstant.Player);

            var sendMessage = string.Empty;
            yield return PromptCenterProcessing.GetHttpPrompt("pt-7z8gkd4xd5bnqqd7", prompt =>
            {
                sendMessage = prompt.ToString(new Dictionary<string, string>
                {
                    { "player", GameConstant.Player },
                    { "message", "爱实！" }
                });
            }, default, player, ("message", "爱实我们做爱吧，昨天你就答应我了哦！"));

            var auth = new IamAuthenticate(AuthenticateManager.AuthenticateElectronAuth.AccessKey, AuthenticateManager.AuthenticateElectronAuth.SecretKey);
            var llm = new LLM(auth, new Uri("https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/ernie-char-8k"));
            requestResource.RequestRequestModule.Messages = new List<Message>()
            {
                new()
                {
                    Role = "user",
                    Content = sendMessage
                }
            };
            llm.RequestResource = requestResource;


            var requestOptions = new RequestOptions
            {
                HeaderParameters = new Multimap<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            yield return llm.Request<ResponseModule>(requestOptions, result =>
            {
                Xiyu.LoggerSystem.LoggerManager.Instance.LogInfo(result.Result);
                Xiyu.LoggerSystem.LoggerManager.Instance.LogInfo(result.BanRound.ToString());
                requestResource.RequestRequestModule.Messages.Add(new Message
                {
                    Role = Message.RoleType.Assistant.ToString().ToLowerInvariant(),
                    Content = result.Result
                });
            });


            while (true)
            {
                yield return null;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    var type = new[] { LogLevel.Info, LogLevel.Warn, LogLevel.Error }.GetRandom();

                    Xiyu.LoggerSystem.LoggerManager.Instance.Log(type, type switch
                    {
                        LogLevel.Info => "我是一条消息",
                        LogLevel.Error => "我是一条错误",
                        LogLevel.Warn => "我是一条警告",
                        _ => throw new ArgumentOutOfRangeException()
                    });
                    // requestOptions.Clear();
                    // yield return llm.Request<ResponseModule>(requestOptions, result =>
                    // {
                    //     Xiyu.LoggerSystem.LoggerManager.Instance.LogInfo(result.Result);
                    //     Xiyu.LoggerSystem.LoggerManager.Instance.LogInfo(result.BanRound.ToString());
                    //     requestResource.RequestRequestModule.Messages.Add(new Message
                    //     {
                    //         Role = Message.RoleType.Assistant.ToString().ToLowerInvariant(),
                    //         Content = result.Result
                    //     });
                    // });
                }
            }
        }
    }
}