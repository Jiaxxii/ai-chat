using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Xiyu.AI.Client;
using Xiyu.Constant;
using Xiyu.Cryptography;
using Xiyu.GameFunction.GameWindows;
using Xiyu.LoggerSystem;
using Random = UnityEngine.Random;

namespace Xiyu.GameGuide
{
    public class FirstSceneLoad : MonoBehaviour
    {
        private string _result = string.Empty;

        private IEnumerator Start()
        {
#if UNITY_EDITOR
            GUIUtility.systemCopyBuffer =


                "3//syr6OGynUUlwLBpulXvTNWimHcyCM*4MusrM8zw8497xM/xcxDePRGXF/RmxPaNPI7n6eJ4K5vw95JQfOpE582ihG72K9ApkqM3yBu+T06kVx4RZ44B6r8ryJE6qmf+tPo29lkDYzysFw9aqmgAWuIOSMmvZeNjQxfYogrJOvpZOh84wpd5NPeESipnn2693t6fzDpJ8drRQUrUNosyiPWxSsXMwCjYBWA0QT8kgOcfyx/ONbSog==";
#endif

            while (true)
            {
                yield return InputDialogWindow.GetWindowWaitForSubmit(OnSubmitTo, new InputWindowParams("输入您的<color=#FE6389>激活秘钥</color>")
                {
                    ShowTweenParams = (0.25F, Ease.Linear),
                    HideTweenParams = (0.25F, Ease.Linear)
                });

                if (_result.StartsWith("Exception"))
                {
                    _result = string.Empty;

                    yield return
                        SingleDialogWindow.GetWindowWaitForClick(null, new SingleWindowParams("签名无效", "<color=#FE6389>激活秘钥无效!</color>", MessageType.Error));

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


                var content = $"秘钥格式：{keyFormat}\r\n签名状态：{signatureState}";

                yield return SingleDialogWindow.GetWindowWaitForClick(null, new SingleWindowParams("签名无效", content, MessageType.Error));

                _result = string.Empty;
            }

            var parameters = new SingleWindowParams("签名完成", $"有效时间:{AuthenticateManager.AuthenticateElectronAuth.GetAffectiveTime:yy-MM-dd hh:mm:ss}", MessageType.Message);
            yield return SingleDialogWindow.GetWindowWaitForClick(_ => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1), parameters);
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

        private IEnumerator Sample()
        {
            UnityAction<bool> resultAction = result => LoggerManager.Instance.LogInfo($"用户选择：{(result ? '是' : '否')}");
            var selectWindowParams = new SelectWindowParams("时间", "2024年7月20日00:42:54");

            // SingleDialogWindow   SelectDialogWindow  InputDialogWindow 等继承自 : DialogWindow<TResult> : DialogWindowBase

            // UI型-自动回收窗口 (在用户  点击任意按钮  窗口淡出后会自动回收窗口)
            yield return SelectDialogWindow.GetWindowWaitForSelect(resultAction, selectWindowParams);


            // 条件型-自动回收窗口 (在用户  点击键盘[ESC]或[回车]  窗口淡出后会自动回收窗口)
            yield return SelectDialogWindow.GetWindowWaitForSelect(resultAction, () => Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter),
                selectWindowParams);


            // 手动型-作用域回收窗口
            using (var windowFirst = (SelectDialogWindow)SelectDialogWindow.GetWindow(SelectDialogWindow.GetTypeName(), autoClose: false))
            {
                yield return windowFirst.DisplayWindow(resultAction, selectWindowParams);
            }


            // 手动型-手动回收窗口
            var windowLast = (SelectDialogWindow)SelectDialogWindow.GetWindow(SelectDialogWindow.GetTypeName(), autoClose: false);

            yield return windowLast.DisplayWindow(resultAction, selectWindowParams);

            yield return windowLast.DoHide(null /* 可选 ()=> windowLast.Dispose() */);

            windowLast.Dispose();
        }
    }
}