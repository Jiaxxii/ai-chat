using System;
using Newtonsoft.Json;
using Xiyu.Cryptography;

namespace Xiyu.AI.Client
{
    public static class AuthenticateManager
    {
        public static ElectronAuth AuthenticateElectronAuth { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <param name="tolerateHours"></param>
        /// <exception cref="global::Xiyu.VirtualLiveRoom.Component.UserLogin.DecryptFailException"></exception>
        /// <returns></returns>
        public static void SetAuth(string jsonContent, int tolerateHours = 0)
        {
            ElectronAuth electronAuth;
            try
            {
                electronAuth = JsonConvert.DeserializeObject<ElectronAuth>(jsonContent);

                AuthenticateElectronAuth = electronAuth;
            }
            catch (JsonSerializationException e)
            {
                throw new DecryptFailException($"Json序列化失败:{e.Message}", "哎呀！密码后台对应不上艾！");
            }
            catch (Exception e)
            {
                throw new DecryptFailException($"序列化失败:{e}", "呜呜呜，我也布吉岛发生了什么...");
            }


            var expireError = electronAuth.IsExpire(tolerateHours);


            if (electronAuth.IsNull())
            {
                throw new DecryptFailException($"Json序列化失败!空的{nameof(ElectronAuth.AccessKey)}或{nameof(ElectronAuth.SecretKey)}",
                    "你是不是被骗了鸭~");
            }

            if (expireError.HasFlag(SignatureState.Expire)
                || expireError.HasFlag(SignatureState.CreateTicksOutNowTicks)
                || expireError.HasFlag(SignatureState.CreateTicksOverAffectiveTicks)
                || expireError.HasFlag(SignatureState.LifeTimeOutMaxEffectiveDay))
            {
                throw new DecryptFailException($"秘钥过期\"{electronAuth}\"", "过期了呢，人家不认！");
            }

        }
    }
}