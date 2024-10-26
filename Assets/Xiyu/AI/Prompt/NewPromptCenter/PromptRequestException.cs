using System;

namespace Xiyu.AI.Prompt.NewPromptCenter
{
    public class PromptRequestException : Exception
    {
        public string RequestId { get; }
        public int Code { get; }
        public override string Message { get; }

        public PromptRequestException(string requestId, string code, string message)
        {
            RequestId = requestId;
            Code = int.Parse(code);
            Message = message;
        }

        public PromptRequestException(string message)
        {
            Message = message;
        }
    }
}