#nullable enable
using System;

namespace Xiyu.VirtualLiveRoom.EventFunctionSystem
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class WebContentInitAttribute : Attribute
    {
        /// <summary>
        /// 标注方法为初始化方法 签名要求：
        /// <code language="CSharp">
        /// [private | protected] [other] Cysharp.Threading.Tasks.UniTask UserInitializationName (System.Threading.CancellationToken cancellationToken){
        ///     // YOU CODE
        /// }
        /// </code>
        /// </summary>
        /// <param name="shouldInitialize">是否进行初始化 (默认:true)</param>
        public WebContentInitAttribute(bool shouldInitialize = true)
        {
            ShouldInitialize = shouldInitialize;
        }

        /// <summary>
        /// 如果为 false 表示不执行初始化
        /// </summary>
        public bool ShouldInitialize { get; }

        /// <summary>
        /// 如果为 true 表示不接受<see cref="System.Threading.CancellationToken"/>
        /// </summary>
        public bool NotUsedCancellationToken { get; set; }

        /// <summary>
        /// 如果为 true 表示在初始化发生在网页内容显示之前
        /// </summary>
        public bool FinalInitialization { get; set; } = true;


        public Type? ThenAfterInitialization { get; set; } = null;
    }
}