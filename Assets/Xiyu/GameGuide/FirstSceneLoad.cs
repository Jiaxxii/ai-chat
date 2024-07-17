using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xiyu.AI.Client;
using Xiyu.Cryptography;
using Xiyu.GameFunction.GameWindows;

namespace Xiyu.GameGuide
{
    public class FirstSceneLoad : MonoBehaviour
    {
        private string _result = string.Empty;

//         private IEnumerator Start()
//         {
// #if UNITY_EDITOR
//             GUIUtility.systemCopyBuffer =
//                 "zAErBNgQo7jK75EL75azrGWOoEx6WpUx*Bfm/4Z4Dz31RqKyAnT7vZNFdex93oMPpYo0g9njAYh837Z3Xtk9oGOyYswWTD2p1zz+EX1I6ewM/24Evib9I/VigI6UYvubtk8NqUPRRIAU8doHKsTju2Be4rrBdSGKuS3cOdmIO8CYRh7FzTDUrIg==";
// #endif
//
//             // var inputWindow = InputDialogWindow.GetWindow(InputDialogWindow.GetTypeName());
//             //
//             // yield return inputWindow.DisplayWindow(OnSubmitTo, new DialogParametersDefault("输入您的<color=#FE6389>激活秘钥</color>")
//             // {
//             //     ShowTweenParams = (0.25F, Ease.Linear),
//             //     HideTweenParams = (0.25F, Ease.Linear)
//             // }).WaitForCompletion();
//
//             yield return InputDialogWindow.GetWindowWaitForSubmit(OnSubmitTo, new DialogParametersDefault("输入您的<color=#FE6389>激活秘钥</color>")
//             {
//                 ShowTweenParams = (0.25F, Ease.Linear),
//                 HideTweenParams = (0.25F, Ease.Linear)
//             });
//
//             while (true)
//             {
//                 yield return new WaitUntil(() => !string.IsNullOrEmpty(_result));
//
//                 if (_result.StartsWith("Exception"))
//                 {
//                     _result = string.Empty;
//                     yield return InputDialogWindow.GetWindow(InputDialogWindow.GetTypeName()).DisplayWindow(OnSubmitTo,
//                         new DialogParametersDefault("<color=#FE6389>激活秘钥无效!</color>")
//                         {
//                             ShowTweenParams = (0.25F, Ease.Linear),
//                             HideTweenParams = (0.25F, Ease.Linear)
//                         }).WaitForCompletion();
//                     continue;
//                 }
//
//                 var auth = AuthenticateManager.SetAuth(_result);
//
//                 if (authState == AuthenticateManager.SignatureState.Available)
//                 {
//                     break;
//                 }
//
//                 var content = authState switch
//                 {
//                     AuthenticateManager.SignatureState.InvalidKey or AuthenticateManager.SignatureState.NullEmptySecretKeyOrAccessKey => "<color=red>秘钥或格式不正确无效</color>",
//                     AuthenticateManager.SignatureState.Expire => "<color=red>秘钥已经过期！</color>",
//
//                     _ => throw new ArgumentOutOfRangeException()
//                 };
//
//                 yield return SelectDialogWindow.GetWindow(SelectDialogWindow.GetTypeName())
//                     .DisplayWindow(null, new SelectDialogWindow.Parameters("签名无效", content))
//                     .WaitForCompletion();
//
//                 _result = string.Empty;
//             }
//
//
//             yield return SelectDialogWindow.GetWindow(SelectDialogWindow.GetTypeName())
//                 .DisplayWindow(_ => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1),
//                     new SelectDialogWindow.Parameters("签名完成", $"有效时间:{AuthenticateManager.AuthenticateElectronAuth.GetAffectiveTime:yy-MM-dd hh:mm:ss}"))
//                 .WaitForCompletion();
//         }

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