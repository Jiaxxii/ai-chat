using System;
using Newtonsoft.Json;
using Xiyu.Constant;

namespace Xiyu.AI.Client
{
    [Flags]
    public enum SignatureState
    {
        None = 0,

        // /// <summary>
        // /// 无效的秘钥
        // /// </summary>
        // InvalidKey = 1 << 0,

        /// <summary>
        /// 空的 SecretKey 或 AccessKey
        /// </summary>
        NullEmptySecretKeyOrAccessKey = 1 << 0,

        // /// <summary>
        // /// 秘钥过期
        // /// </summary>
        // SecretKeyExpire = 1 << 2,
        //
        // /// <summary>
        // /// 秘钥可用
        // /// </summary>
        // SecretKeyAvailable = 1 << 3,
        //
        // /// <summary>
        // /// 秘钥无法使用
        // /// </summary>
        // SecretKeyUnavailable = 1 << 4,

        /// <summary>
        /// 有效签名
        /// </summary>
        Available = 1 << 1,

        /// <summary>
        /// 过期签名
        /// </summary>
        Expire = 1 << 2,

        /// <summary>
        /// 签名生成时间大于当前时间
        /// </summary>
        CreateTicksOutNowTicks = 1 << 3,

        /// <summary>
        /// 签名生成时间大于激活时间
        /// </summary>
        CreateTicksOverAffectiveTicks = 1 << 4,

        /// <summary>
        /// 签名生命周期大于最大生命周期
        /// </summary>
        LifeTimeOutMaxEffectiveDay = 1 << 5,

    }

    // public enum SignatureState
    // {
    //     /// <summary>
    //     /// 无效的秘钥
    //     /// </summary>
    //     InvalidKey,
    //
    //     /// <summary>
    //     /// 空的 SecretKey 或 AccessKey
    //     /// </summary>
    //     NullEmptySecretKeyOrAccessKey,
    //
    //     /// <summary>
    //     /// 秘钥过期
    //     /// </summary>
    //     Expire,
    //
    //     /// <summary>
    //     /// 可用
    //     /// </summary>
    //     Available,
    //
    //     /// <summary>
    //     /// 无法使用
    //     /// </summary>
    //     Unavailable
    // }

    public class ElectronAuth
    {
        [JsonProperty(PropertyName = "ak")] public string AccessKey { get; set; }

        [JsonProperty(PropertyName = "sk")] public string SecretKey { get; set; }

        [JsonProperty(PropertyName = "create_ticks")]
        public long CreateTicks { get; set; }

        [JsonProperty(PropertyName = "effective_ticks")]
        public long AffectiveTicks { get; set; }


        public DateTime GetAffectiveTime => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(AffectiveTicks);


        public bool IsNull() => string.IsNullOrEmpty(AccessKey) && string.IsNullOrEmpty(SecretKey);


        public SignatureState IsExpire(int tolerateHours = 0)
        {
            var createTimeSpan = new TimeSpan(CreateTicks);
            var effectiveTimeSpan = new TimeSpan(AffectiveTicks).Add(TimeSpan.FromHours(tolerateHours));

            // 判断时间戳是否在 NOW 之后创建
            if (GameConstant.GetUnixTimeOriginToNow().Ticks < createTimeSpan.Ticks)
            {
                return SignatureState.CreateTicksOutNowTicks;
            }

            // 判断创建时间是否大于过期时间
            if (createTimeSpan.Ticks > effectiveTimeSpan.Ticks)
            {
                return SignatureState.CreateTicksOverAffectiveTicks;
            }

            // 判断签名的生命周期是否大于'GameConstant.MaxEffectiveDay'
            if ((effectiveTimeSpan - createTimeSpan).Days > GameConstant.MaxEffectiveDay)
            {
                return SignatureState.LifeTimeOutMaxEffectiveDay;
            }

            // 计算从有效时间到现在的时间间隔  
            var fallSpan = effectiveTimeSpan - GameConstant.GetUnixTimeOriginToNow();

            return fallSpan.TotalSeconds > 0 ? SignatureState.Available : SignatureState.Expire;
        }
    }
}