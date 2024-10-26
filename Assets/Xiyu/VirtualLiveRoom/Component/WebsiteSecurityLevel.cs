namespace Xiyu.VirtualLiveRoom.Component
{
    /// <summary>
    /// 网站安全等级
    /// </summary>
    public enum WebsiteSecurityLevel
    {
        /// <summary>
        /// 未定义的网址安全等级，这类网站多见于临时测试的“本地”网址
        /// </summary>
        Undefined,

        /// <summary>
        /// 安全的网址，几乎全年龄段适宜
        /// </summary>
        Safe,

        /// <summary>
        /// 网站是未认证的或存在部分18+内容
        /// </summary>
        Warn,

        /// <summary>
        /// 危险的网址，这可能对您或您的财产、计算机等照成威胁
        /// </summary>
        Dangerous,

        /// <summary>
        /// 无效网址
        /// </summary>
        Null
    }
}