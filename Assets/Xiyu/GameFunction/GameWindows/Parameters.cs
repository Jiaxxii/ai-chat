using UnityEngine;
using Xiyu.Expand;
using Xiyu.LoggerSystem;

namespace Xiyu.GameFunction.GameWindows
{
    #region 选择对话框参数

    public class SelectWindowParams : DialogWindowParameters
    {
        public SelectWindowParams(string title, string content) : base(title)
        {
            Content = content;
        }

        public string Content { get; }
    }

    #endregion

    #region 文本输入对话框参数

    public class InputWindowParams : DialogWindowParameters
    {
        public InputWindowParams(string title) : base(title)
        {
        }
    }

    #endregion

    #region 单选对话框参数

    public class SingleWindowParams : SelectWindowParams
    {
        public SingleWindowParams(string title, string content) : base(title, content)
        {
            MessageType = MessageType.Message;
        }

        public SingleWindowParams(string title, string content, MessageType messageType) : base(title, content)
        {
            MessageType = messageType;
        }

        public MessageType MessageType { get; set; }


        public Color GetLevelColor() => MessageType switch
        {
            MessageType.Message => Color.white,
            MessageType.Waring => Color.yellow,
            MessageType.Error => Color.red,
            _ => Color.gray
        };

        public string GetLevelColorSting() => GetLevelColor().ToHexadecimalString();

        public string GetTitle() => $@"<color=#{GetLevelColorSting()}>{MessageType switch
        {
            MessageType.Message => "提示",
            MessageType.Waring => "警告",
            MessageType.Error => "错误",
            _ => "异常"
        }}</color>";
    }

    #endregion
}