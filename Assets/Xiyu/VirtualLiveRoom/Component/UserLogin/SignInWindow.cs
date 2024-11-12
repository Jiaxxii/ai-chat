using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.AI.Client;
using Xiyu.AI.Prompt.NewPromptCenter;
using Xiyu.Constant;
using Xiyu.Cryptography;
using Xiyu.LoggerSystem;
using Xiyu.VirtualLiveRoom.Component.NewNavigation;
using Xiyu.VirtualLiveRoom.Tools.Addressabe;
using Random = UnityEngine.Random;

namespace Xiyu.VirtualLiveRoom.Component.UserLogin
{
    public class SignInWindow : UIContainer
    {
        [SerializeField] private TMP_InputField inputFieldName;
        [SerializeField] private TMP_InputField inputFieldToken;
        [SerializeField] private Button loginButton;
        [SerializeField] private TMP_Text loginText;

        [SerializeField] private TMP_Text titleText;

        private const string LoadingString = "(.__.)/......";

        private CancellationTokenSource _loadingTextCancellationTokenSource;


        private void Awake()
        {
            loginButton.onClick.AddListener(UniTask.UnityAction(OnButtonClickEventHandler));

#if UNITY_EDITOR
            GUIUtility.systemCopyBuffer = File.ReadAllText(@"C:\Users\jiaxx\AppData\Roaming\JetBrains\Rider2024.1\scratches\NewFile1.txt");
#endif
            inputFieldName.text = PlayerPrefs.HasKey("player name") ? PlayerPrefs.GetString("player name") : string.Empty;
            inputFieldToken.text = PlayerPrefs.HasKey("aksk") ? PlayerPrefs.GetString("aksk") : string.Empty;
        }


        private async UniTaskVoid OnButtonClickEventHandler()
        {
            if (_loadingTextCancellationTokenSource != null)
            {
                await DoTitleTextAsync("验证中哦！~");
                return;
            }

            _loadingTextCancellationTokenSource = new CancellationTokenSource();

            // 播放加载动画
            PlayLoadingText(2F, _loadingTextCancellationTokenSource.Token);

            try
            {
                // 验证鉴权凭证
                await SetAuthenticationCertificateAsync(inputFieldToken.text);
                PlayerPrefs.SetString("aksk", inputFieldToken.text);

                // 设置用户名称与头像
                await SetPreferredNameAndHeadAsync(inputFieldName.text);
                PlayerPrefs.SetString("player name", inputFieldName.text);
                PlayerPrefs.SetString("user", User.UserHeadSprite.name);

                // 请求千帆API的模板List （验证网络连通性）
                var promptInfos = await RequestQianFanPromptListAsync(destroyCancellationToken);

                if (promptInfos.Count == 0 || promptInfos.All(v => v.TemplateId != PromptRequest.DefaultPromptID))
                {
                    await PromptRequest.TryLoadDefaultPrompt();
                }
                else
                {
                    await PromptRequest.TryRequestPromptAsync(PromptRequest.DefaultPromptID, null, default);
                }


                // 加载链接页面
                await NavigationCenter.Instance.AppendViewAsync(WebsiteCenter.LinkForm1);
            }
            catch (DecryptFailException e)
            {
                inputFieldName.text = string.Empty;
                await UniTask.WhenAll(
                    LoggerManager.Instance.LogWarnAsync(e.ErrorTitle),
                    DoTitleTextAsync(e.ErrorTitle)
                );
            }
            catch (Exception e)
            {
                inputFieldName.text = string.Empty;
                await LoggerManager.Instance.LogWarnAsync(e.ToString());
            }
            finally
            {
                _loadingTextCancellationTokenSource.Cancel();
                _loadingTextCancellationTokenSource.Dispose();
                _loadingTextCancellationTokenSource = null;
            }
        }

        private UniTask DoTitleTextAsync(string content)
        {
            titleText.text = content;
            titleText.alpha = 0F;
            return titleText.DOFade(0.75F, 1.75F).AsyncWaitForCompletion().AsUniTask();
        }


        private async UniTask<List<PromptInfo>> RequestQianFanPromptListAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // 检测网络是否链接
                await Authenticate.RequestWebServiceAsync();

                // 请求模板列表 (注意*第一次请求后会将模板保存到本地，在未来可能和服务器的模板不一致)
                // 可以使用“PromptRequest.RequestPromptListAsync()”来获取最新模板列表并且使用“PromptRequest.WritPromptListToAsync()”来更新
                var promptInfos = await PromptRequest.TryRequestPromptListAsync(cancellationToken: cancellationToken);

                // 如果没有获取到元素就表示服务器中没有*匹配*的模板可用
                if (promptInfos.Count == 0)
                {
                    throw new PromptRequestException("NULL", "-1", "服务器中未找到匹配的模板");
                }

                // 保存到本地
                await PromptRequest.WritPromptListToAsync(promptInfos, cancellationToken);


                return promptInfos;
            }
            // 网络未链接 or 超时 or PromptInfos请求失败
            catch (UnityWebRequestException e)
            {
                await UniTask.WhenAll(
                    LoggerManager.Instance.LogWarnAsync(e.ToString(), cancellationToken: cancellationToken),
                    DoTitleTextAsync("你的wife呸~ wifi是不是有问题嘛~~~")
                );
            }
            // 服务返回的数据流异常
            catch (JsonSerializationException e)
            {
                await UniTask.WhenAll(
                    LoggerManager.Instance.LogWarnAsync(e.ToString(), cancellationToken: cancellationToken),
                    DoTitleTextAsync("emmmmmm，一定是服务器的问题！！！")
                );
            }
            // 服务返回的Json结构改变导致序列化为空 (触发此异常表示Json反序列是成功的，只是关键数据未赋值)
            catch (PromptRequestException e)
            {
                await UniTask.WhenAll(
                    LoggerManager.Instance.LogWarnAsync(e.ToString(), cancellationToken: cancellationToken),
                    DoTitleTextAsync("对不起哦~可以联系<color=#E4514C>西雨与雨</color>（抖音）好好更新哦~")
                );
            }

            throw new Exception("未通过网络链接测试，具体请查看日志。");
        }

        private async void PlayLoadingText(float duration, CancellationToken cancellationToken)
        {
            inputFieldName.interactable = false;
            inputFieldToken.interactable = false;
            try
            {
                while (true)
                {
                    for (var i = 0; i < LoadingString.Length; i++)
                    {
                        loginText.text = LoadingString.Substring(0, i);

                        await UniTask.WaitForSeconds(duration / LoadingString.Length, cancellationToken: cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                loginText.text = "GO!";
                inputFieldName.interactable = true;
                inputFieldToken.interactable = true;
            }
        }

        private async UniTask SetPreferredNameAndHeadAsync(string userName)
        {
            // 未输入名称则提供默认名称
            if (string.IsNullOrEmpty(userName))
            {
                inputFieldName.text = GameConstant.Player;
                await DoTitleTextAsync($"那你就叫“<color=#ff669b>{GameConstant.Player}</color>”~~~");
            }


            if (PlayerPrefs.HasKey("user"))
            {
                var spriteName = PlayerPrefs.GetString("user", "main");
                User.UserHeadSprite = (Sprite)await Resources.LoadAsync<Sprite>($"Default/User/{spriteName}");
            }
            else
            {
                var sprites = Resources.LoadAll<Sprite>("Default/User");
                User.UserHeadSprite = sprites[Random.Range(0, sprites.Length)];
            }


            User.UserName = inputFieldName.text;
        }

        private static async UniTask SetAuthenticationCertificateAsync(string ciphertext)
        {
            // 验证秘钥正确性 密码格式错误时抛出异常
            var plaintext = await TryVerifyAsync(ciphertext);

            // 设置鉴权凭证，如果凭证过期则抛出异常
            Xiyu.AI.Client.AuthenticateManager.SetAuth(plaintext);
        }

        private static async UniTask<string> TryVerifyAsync(string keychain)
        {
            if (string.IsNullOrEmpty(keychain))
            {
                throw new DecryptFailException($"对象引用\"{nameof(keychain)}\"未设置对象实例。", "你的密码嘞？");
            }

            if (!keychain.Contains('*'))
            {
                throw new DecryptFailException("密码格式异常，缺少'*'", "艾？密码格式是不是错了？");
            }

            var split = keychain.Split('*');
            if (split.Length != 2)
            {
                throw new DecryptFailException($"分割后的元素数量不等于2({split.Length})", "艾？密码格式是不是有问题？");
            }

            var keyBase64 = split[0];
            var ciphertext = split[1];

            if (string.IsNullOrEmpty(keyBase64))
            {
                throw new DecryptFailException("秘钥串缺少秘钥", "人家没有找到“钥匙”哦！");
            }

            if (string.IsNullOrEmpty(ciphertext))
            {
                throw new DecryptFailException("秘钥串缺少秘钥", "人家没有找到“密码”哦！");
            }


            try
            {
                var keyBytes = AesEncryptionHelper.Base64ToKey(keyBase64);

                return await AesEncryptionHelper.DecryptAsync(ciphertext, keyBytes);
            }
            catch (ArgumentException)
            {
                // 密文长度不正确
                throw new DecryptFailException("密文格式不正确（长度有误，可能被截断）", "笨蛋，你的“密码”是不是不对呀~");
            }
            catch (FormatException)
            {
                // 秘钥的key不正确
                throw new DecryptFailException("秘钥格式不正确（不是一个正确的Base64）", "你的“钥匙”有问题耶...");
            }
            catch (CryptographicException e)
            {
                // 解密失败，秘钥无效。。。
                throw new DecryptFailException($"解密失败：{e.Message}", "喂喂，不要拿假的骗我嘛！！！");
            }
        }
    }
}