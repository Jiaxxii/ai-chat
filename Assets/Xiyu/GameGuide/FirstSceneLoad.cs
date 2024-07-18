using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xiyu.AI.Client;
using Xiyu.Constant;
using Xiyu.Cryptography;
using Xiyu.GameFunction.GameWindows;

namespace Xiyu.GameGuide
{
    public class FirstSceneLoad : MonoBehaviour
    {
        private string _result = string.Empty;

        private IEnumerator Start()
        {
#if UNITY_EDITOR
            GUIUtility.systemCopyBuffer =
                "zAErBNgQo7jK75EL75azrGWOoEx6WpUx*Bfm/4Z4Dz31RqKyAnT7vZNFdex93oMPpYo0g9njAYh837Z3Xtk9oGOyYswWTD2p1zz+EX1I6ewM/24Evib9I/VigI6UYvubtk8NqUPRRIAU8doHKsTju2Be4rrBdSGKuS3cOdmIO8CYRh7FzTDUrIg==";
#endif
            while (true)
            {

                yield return InputDialogWindow.GetWindowWaitForSubmit(OnSubmitTo, new DialogParametersDefault("输入您的<color=#FE6389>激活秘钥</color>")
                {
                    ShowTweenParams = (0.25F, Ease.Linear),
                    HideTweenParams = (0.25F, Ease.Linear)
                });

                if (_result.StartsWith("Exception"))
                {
                    _result = string.Empty;

                    yield return InputDialogWindow.GetWindowWaitForSubmit(OnSubmitTo, new DialogParametersDefault("<color=#FE6389>激活秘钥无效!</color>")
                    {
                        ShowTweenParams = (0.25F, Ease.Linear),
                        HideTweenParams = (0.25F, Ease.Linear)
                    });

                    continue;
                }

                var auth = AuthenticateManager.SetAuth(_result);

                if (auth is
                    {
                        signatureState: SignatureState.Available,
                        signatureExpireState: not null,
                        signatureExpireState: SignatureExpireState.Available,
                    })
                {
                    // 验证成功 退出循环
                    break;
                }

                var keyFormat = auth.signatureState switch
                {
                    SignatureState.InvalidKey => "无效的秘钥",
                    SignatureState.NullEmptySecretKeyOrAccessKey => "空的 SecretKey 或 AccessKey",
                    SignatureState.Expire => "秘钥过期",
                    SignatureState.Available => "秘钥格式通过",
                    SignatureState.Unavailable => "秘钥不可用",
                    _ => throw new ArgumentOutOfRangeException()
                };

                var signatureState = auth.signatureExpireState.HasValue == false
                    ? "秘钥格式错误！"
                    : auth.signatureExpireState.Value switch
                    {
                        SignatureExpireState.Available => "签名有效",
                        SignatureExpireState.Expire => "签名已过期",
                        SignatureExpireState.CreateTicksOutNowTicks => "签名的创建时间大于当前时间",
                        SignatureExpireState.CreateTicksOverAffectiveTicks => "签名的创建时间大于签名过期时间",
                        SignatureExpireState.LifeTimeOutMaxEffectiveDay => $"签名生命周期大于最大生命周 (最大天数:{GameConstant.MaxEffectiveDay})",
                        _ => throw new ArgumentOutOfRangeException()
                    };


                var content = $"秘钥格式:{keyFormat}\r\n签名状态:{signatureState}";

                yield return SelectDialogWindow.GetWindowWaitForSelect(null, new SelectDialogWindow.Parameters("签名无效", content));

                _result = string.Empty;
            }

            var parameters = new SelectDialogWindow.Parameters("签名完成", $"有效时间:{AuthenticateManager.AuthenticateElectronAuth.GetAffectiveTime:yy-MM-dd hh:mm:ss}");
            yield return SelectDialogWindow.GetWindowWaitForSelect(_ => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1), parameters);
        }

        private async void OnSubmitTo(string auth)
        {
            var authSplit = auth.Split('*');
            try
            {
                var key = AesEncryptionHelper.Base64ToKey(authSplit[0]);
                var cipherText = authSplit[1];
                _result = await AesEncryptionHelper.DecryptAsync(cipherText, key);
            }
            catch (Exception e)
            {
                _result = $"Exception {e.Message}";
            }
        }
    }
}