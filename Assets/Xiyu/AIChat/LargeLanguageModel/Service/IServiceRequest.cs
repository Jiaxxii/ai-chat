using System.Collections.Generic;
using Xiyu.AIChat.Client;

namespace Xiyu.AIChat.LargeLanguageModel.Service
{
    public interface IServiceRequest<TRequestBody>
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// 请求方法 （多为POST
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// 请求体
        /// </summary>
        public TRequestBody Body { get; set; }

        RequestOptions RequestOptions { get; }
    }
}