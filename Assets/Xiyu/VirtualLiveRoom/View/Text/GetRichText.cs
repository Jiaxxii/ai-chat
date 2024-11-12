using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Xiyu.VirtualLiveRoom.View.Text
{
    public static class GetRichText
    {
        private static readonly Stack<string> RichStack = new();

        public static readonly HashSet<char> SymbolTable = new()
        {
            ' ', ',', '，', '.', '。', '?', '？', '(', '（', ')', '）', '~', '+', '-', '*', '/', '《', '》', '\n', '\t', '\r'
        };

        // private static readonly 

        public static async UniTask Print(this TextMeshProUGUI textMeshProUGUI, string content, float nextTime, bool role, HashSet<char> textSymbols = null)
        {
            // 定义可以存储整个显示文本的StringBuilder
            var displayedText = new StringBuilder();

            var index = 0;
            textMeshProUGUI.text = string.Empty;
            while (index < content.Length)
            {
                // 判断当前的字符是不是应该被忽略
                if (textSymbols != null && textSymbols.Contains(content[index]))
                {
                    displayedText.Append(content[index]);
                    index++;
                    continue;
                }

                // 判断是否已经读到HTML标签
                if (content[index] == '<')
                {
                    var tag = GetTag(content, index);

                    if (tag == null) // 如果没有关闭标签 '>'
                    {
                        // 可能你需要在这里处理错误或退出循环
                        return;
                    }

                    // 将标签加入到栈中管理
                    RichStack.Push(tag);

                    // 将标签添加到最终显示文本中
                    displayedText.Append(tag);
                    index += tag.Length;

                    // 如果是闭合标签，需要移除对应的开标签
                    if (IsEndTagWith(tag))
                    {
                        // 假设每个闭合标签都有一个相应的开标签，且解析顺序正确
                        RichStack.Pop(); // 移除开标签
                    }

                    continue;
                }

                // 将当前字符加入到最终显示文本中
                displayedText.Append(content[index]);
                textMeshProUGUI.text = role ? $"\"{displayedText}\"" : displayedText.ToString();
                index++;
                await UniTask.WaitForSeconds(nextTime);
            }
        }
        
        private static string GetTag(string str, int index)
        {
            var closeTag = str.IndexOf('>', index);

            return closeTag == -1
                ? null
                : // 没有找到闭合的 '>'，返回null
                str.Substring(index, closeTag - index + 1);
        }


        private static bool IsEndTagWith(string str)
        {
            return str.StartsWith("</");
        }
    }
}