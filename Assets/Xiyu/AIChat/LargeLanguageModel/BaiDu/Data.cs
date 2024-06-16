using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Xiyu.AIChat.LargeLanguageModel.BaiDu
{
    #region 预配置

    [Serializable]
    public class Config
    {
        [Tooltip("(可选)是否以流式接口的形式返回数据，默认false")] [SerializeField]
        private bool stream;


        [Tooltip("可选)说明：" +
                 "1）较高的数值会使输出更加随机，而较低的数值会使其更加集中和确定" +
                 "（2）默认0.95，范围 (0, 1.0]，不能为0")]
        [SerializeField]
        [Range(0f, 1f)]
        private float temperature = 0.95F;


        [Tooltip("(可选)说明：" +
                 "（1）影响输出文本的多样性，取值越大，生成文本的多样性越强" +
                 "（2）默认0.7，取值范围 [0, 1.0]")]
        [SerializeField]
        [Range(0f, 1f)]
        private float top_p = 0.7F;


        [Tooltip("(可选) 通过对已生成的token增加惩罚，减少重复生成的现象。说明：" +
                 "（1）值越大表示惩罚越大" +
                 "（2）默认1.0，取值范围：[1.0, 2.0]")]
        [SerializeField]
        [Range(1f, 2f)]
        private float penalty_score = 1;


        [Tooltip("(可选) 模型人设，主要用于人设设定，例如：你是xxx公司制作的AI助手，说明：" +
                 "长度限制，message中的content总长度和system字段总内容不能超过24000个字符，且不能超过6144 tokens")]
        [SerializeField]
        [TextArea(3, 10)]
        private string system;


        [Tooltip("(可选) 生成停止标识，当模型生成结果以stop中某个元素结尾时，停止文本生成。说明：" +
                 "（1）每个元素长度不超过20字符" +
                 "（2）最多4个元素")]
        [SerializeField]
        private List<string> stop;


        [Tooltip("(可选) 指定模型最大输出token数，说明：" +
                 "（1）如果设置此参数，范围[2, 1024]" +
                 "（2）如果不设置此参数，最大输出token数为1024")]
        [SerializeField]
        [Range(2, 1024)]
        private int max_output_tokens = 512;
        
        
        [Tooltip("(可选)是否以流式接口的形式返回数据，默认false")] [SerializeField]
        private string user_id;

        public bool Stream => stream;

        public float Temperature => temperature;

        public float TopP => top_p;

        public float PenaltyScore => penalty_score;

        public string System => system;

        public List<string> Stop => stop;

        public int MaxOutputTokens => max_output_tokens;

        public string UserID => user_id;
    }

    #endregion

    #region 数据定义

    /// <summary>
    /// 请求数据 Body
    /// </summary>
    [Serializable]
    public class RequestData
    {
        /// <summary>
        /// 发送的消息
        /// </summary>
        [JsonProperty(propertyName: "messages")]
        public List<Message> Messages { get; set; }

        /// <summary>
        /// (可选)是否以流式接口的形式返回数据，默认false
        /// </summary>
        [JsonProperty(PropertyName = "stream")]
        public bool Stream { get; set; }

        /// <summary>
        /// (可选)说明：
        /// （1）较高的数值会使输出更加随机，而较低的数值会使其更加集中和确定
        /// （2）默认0.95，范围 (0, 1.0]，不能为0
        /// </summary>
        [JsonProperty(PropertyName = "temperature")]
        public float Temperature { get; set; } = 0.95F;

        /// <summary>
        /// (可选)说明：
        /// （1）影响输出文本的多样性，取值越大，生成文本的多样性越强
        /// （2）默认0.7，取值范围 [0, 1.0]
        /// </summary>
        [JsonProperty(PropertyName = "top_p")]
        public float TopP { get; set; } = 0.7F;

        /// <summary>
        /// (可选) 通过对已生成的token增加惩罚，减少重复生成的现象。说明：
        /// （1）值越大表示惩罚越大
        /// （2）默认1.0，取值范围：[1.0, 2.0]
        /// </summary>
        [JsonProperty(PropertyName = "penalty_score")]
        public float PenaltyScore { get; set; } = 1;

        /// <summary>
        /// (可选) 模型人设，主要用于人设设定，例如：你是xxx公司制作的AI助手，说明：
        /// 长度限制，message中的content总长度和system字段总内容不能超过24000个字符，且不能超过6144 tokens
        /// </summary>
        [JsonProperty(PropertyName = "system")]
        public string System { get; set; } = string.Empty;

        /// <summary>
        /// (可选) 生成停止标识，当模型生成结果以stop中某个元素结尾时，停止文本生成。说明：
        /// （1）每个元素长度不超过20字符
        /// （2）最多4个元素
        /// </summary>
        [JsonProperty(PropertyName = "stop")]
        public List<string> Stop { get; set; }

        /// <summary>
        /// (可选) 指定模型最大输出token数，说明：
        /// <para>（1）如果设置此参数，范围[2, 1024]</para>
        /// <para>（2）如果不设置此参数，最大输出token数为1024</para>
        /// </summary>
        [JsonProperty(PropertyName = "max_output_tokens")]
        public int MaxOutputTokens { get; set; } = 2 << 8;

        /// <summary>
        /// (可选)表示最终用户的唯一标识符
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserID { get; set; } = string.Empty;
    }

    /// <summary>
    /// 返回消息
    /// </summary>
    [Serializable]
    public class Message
    {
        /// <summary>
        /// 角色
        /// <para>user: 表示用户</para>
        /// <para>assistant: 表示对话助手</para>
        /// </summary>
        [JsonProperty(propertyName: "role")]
        public string Role { get; set; }

        /// <summary>
        /// 对话内容
        /// </summary>
        [JsonProperty(propertyName: "content")]
        public string Content { get; set; }

        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }

        public Message(RoleType role, string content)
        {
            Role = role.ToString().ToLower();
            Content = content;
        }

        public enum RoleType
        {
            // ReSharper disable once InconsistentNaming
            user,

            // ReSharper disable once InconsistentNaming
            assistant
        }
    }


    /// <summary>
    /// 响应结果
    /// </summary>
    [Serializable]
    public class ResponseData
    {
        /// <summary>
        /// 本轮对话的id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        /// <summary>
        /// 回包类型  chat.completion：多轮对话返回
        /// </summary>
        [JsonProperty(PropertyName = "object")]
        public string Object { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public int Created { get; set; }

        /// <summary>
        /// 表示当前子句的序号。只有在流式接口模式下会返回该字段
        /// </summary>
        [JsonProperty(PropertyName = "sentence_id")]
        public int SentenceId { get; set; }

        /// <summary>
        /// 表示当前子句是否是最后一句。只有在流式接口模式下会返回该字段
        /// </summary>
        [JsonProperty(PropertyName = "is_end")]
        public bool IsEnd { get; set; }

        /// <summary>
        /// 当前生成的结果是否被截断
        /// </summary>
        [JsonProperty(PropertyName = "is_truncated")]
        public bool IsTruncated { get; set; }

        /// <summary>
        /// 对话返回结果
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public string Result { get; set; }

        /// <summary>
        /// 表示用户输入是否存在安全风险，是否关闭当前会话，清理历史会话信息。
        /// <para>true：是，表示用户输入存在安全风险，建议关闭当前会话，清理历史会话信息。</para>
        /// <para>false：否，表示用户输入无安全风险</para>
        /// </summary>
        [JsonProperty(PropertyName = "need_clear_history")]
        public bool NeedClearHistory { get; set; }

        /// <summary>
        /// 当<see cref="NeedClearHistory"/>为true时，此字段会告知第几轮对话有敏感信息，如果是当前问题，<see cref="BanRound"/> = -1
        /// </summary>
        [JsonProperty(PropertyName = "ban_round")]
        public int BanRound { get; set; }


        /// <summary>
        /// token统计信息，token数 = 汉字数 + 单词数 * 1.3 
        /// </summary>
        [JsonProperty(PropertyName = "usage")]
        public Usage Usage { get; set; }
    }

    /// <summary>
    /// token统计
    /// </summary>
    [Serializable]
    public class Usage
    {
        /// <summary>
        /// 问题tokens数
        /// </summary>
        [JsonProperty(PropertyName = "prompt_tokens")]
        public int PromptTokens { get; set; }

        /// <summary>
        /// 回答tokens数
        /// </summary>
        [JsonProperty(PropertyName = "completion_tokens")]
        public int CompletionTokens { get; set; }

        /// <summary>
        /// tokens总数
        /// </summary>
        [JsonProperty(PropertyName = "total_tokens")]
        public int TotalTokens { get; set; }
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    [Serializable]
    public class RequestError
    {
        [JsonProperty(PropertyName = "error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty(PropertyName = "error_msg")]
        public string ErrorMsg { get; set; }
    }

    #endregion
}