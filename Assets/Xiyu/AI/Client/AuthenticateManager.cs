using Newtonsoft.Json;

namespace Xiyu.AI.Client
{
    public enum SignatureState
    {
        /// <summary>
        /// 无效的秘钥
        /// </summary>
        InvalidKey,

        /// <summary>
        /// 空的 SecretKey 或 AccessKey
        /// </summary>
        NullEmptySecretKeyOrAccessKey,

        /// <summary>
        /// 秘钥过期
        /// </summary>
        Expire,

        /// <summary>
        /// 可用
        /// </summary>
        Available,

        /// <summary>
        /// 无法使用
        /// </summary>
        Unavailable
    }

    public static class AuthenticateManager
    {
        public static ElectronAuth AuthenticateElectronAuth { get; set; }

        public static (SignatureState signatureState, SignatureExpireState? signatureExpireState) SetAuth(string jsonContent, int tolerateHours = 0)
        {
            try
            {
                var electronAuth = JsonConvert.DeserializeObject<ElectronAuth>(jsonContent);

                if (electronAuth.IsNull())
                {
                    return (SignatureState.NullEmptySecretKeyOrAccessKey, null);
                }

                var signatureExpireState = electronAuth.IsExpire(tolerateHours);

                if (signatureExpireState == SignatureExpireState.Available)
                {
                    AuthenticateElectronAuth = electronAuth;
                    return (SignatureState.Available, SignatureExpireState.Available);
                }

                if (signatureExpireState == SignatureExpireState.Expire)
                {
                    return (SignatureState.Expire, SignatureExpireState.Expire);
                }

                return (SignatureState.Unavailable, signatureExpireState);
            }
            catch (JsonSerializationException)
            {
                return (SignatureState.InvalidKey, null);
            }
        }
    }
}