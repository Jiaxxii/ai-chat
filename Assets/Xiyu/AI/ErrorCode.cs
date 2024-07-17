using Newtonsoft.Json.Linq;

namespace Xiyu.AI
{
    public interface IRequestError
    {
        int ErrorCode { get; set; }

        string ErrorMessage { get; set; }
    }

    public class RequestError : IRequestError
    {
        private const string ConstErrorCode = "error_code";
        private const string ConstErrorMessage = "error_msg";
        
        public static IRequestError JsonAnalysisToObject(string jsonContent)
        {
            var jObject = JObject.Parse(jsonContent);
            if (!jObject.TryGetValue(ConstErrorCode, out var codeToken) || !jObject.TryGetValue(ConstErrorMessage, out var msgToken))
            {
                return null;
            }

            var code = codeToken.Value<int>();
            var message = msgToken.Value<string>();

            if (string.IsNullOrEmpty(message))
            {
                return null;
            }

            return new RequestError
            {
                ErrorCode = code,
                ErrorMessage = message
            };
        }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}