using System;
using UnityEngine;

namespace Xiyu.AI.LargeLanguageModel.Service.Response
{
    [Serializable]
    public class Usage
    {
        [SerializeField] private int promptTokens;
        [SerializeField] private int completionTokens;
        [SerializeField] private int totalTokens;

        public int PromptTokens
        {
            get => promptTokens;
            set => promptTokens = value;
        }

        public int CompletionTokens
        {
            get => completionTokens;
            set => completionTokens = value;
        }

        public int TotalTokens
        {
            get => totalTokens;
            set => totalTokens = value;
        }
    }
}