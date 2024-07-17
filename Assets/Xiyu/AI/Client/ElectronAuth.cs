using System;
using Newtonsoft.Json;
using Xiyu.Constant;

namespace Xiyu.AI.Client
{
    public enum SignatureExpireState
    {
        /// <summary>
        /// 有效
        /// </summary>
        Available,

        /// <summary>
        /// 过期
        /// </summary>
        Expire,

        /// <summary>
        /// 签名生成时间大于当前时间
        /// </summary>
        CreateTicksOutNowTicks,

        /// <summary>
        /// 签名生成时间大于激活时间
        /// </summary>
        CreateTicksOverAffectiveTicks,

        /// <summary>
        /// 签名生命周期大于最大生命周期
        /// </summary>
        LifeTimeOutMaxEffectiveDay
    }

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


        public SignatureExpireState IsExpire(int tolerateHours = 0)
        {
            var createTimeSpan = new TimeSpan(CreateTicks);
            var effectiveTimeSpan = new TimeSpan(AffectiveTicks).Add(TimeSpan.FromHours(tolerateHours));

            // 判断时间戳是否在 NOW 之后创建
            if (GameConstant.GetUnixTimeOriginToNow().Ticks < createTimeSpan.Ticks)
            {
                return SignatureExpireState.CreateTicksOutNowTicks;
            }

            // 判断创建时间是否大于过期时间
            if (createTimeSpan.Ticks > effectiveTimeSpan.Ticks)
            {
                return SignatureExpireState.CreateTicksOverAffectiveTicks;
            }

            // 判断签名的生命周期是否大于'GameConstant.MaxEffectiveDay'
            if ((effectiveTimeSpan - createTimeSpan).Days > GameConstant.MaxEffectiveDay)
            {
                return SignatureExpireState.LifeTimeOutMaxEffectiveDay;
            }

            // 计算从有效时间到现在的时间间隔  
            var fallSpan = effectiveTimeSpan - GameConstant.GetUnixTimeOriginToNow();

            return fallSpan.TotalSeconds > 0 ? SignatureExpireState.Available : SignatureExpireState.Expire;
        }
    }
}