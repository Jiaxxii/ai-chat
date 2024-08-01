using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Xiyu.AI.LargeLanguageModel.Service.Response
{
    public interface IResult
    {
        [JsonProperty(PropertyName = "result")]
        string Result { get; }
    }

    public abstract class DeserializeParameterModule : IResult
    {
        public RequestError Error { get; private set; }

        public virtual string Result { get; set; }

        public abstract bool IsDefaultOrNull();

        [NotNull]
        public static T Deserialize<T>(string value, JsonSerializerSettings jsonSerializerSettings) where T : DeserializeParameterModule, new()
        {
            var instance = JsonConvert.DeserializeObject<T>(value, jsonSerializerSettings);

            if (instance is null)
            {
                return new T
                {
                    Error = new RequestError
                    {
                        ErrorCode = -1,
                        ErrorMessage = $"json 无法被有效序列化为 {typeof(T).Name}、{nameof(IRequestError)}!\n{value}"
                    }
                };
            }

            if (!instance.IsDefaultOrNull())
            {
                return instance;
            }

            var requestError = RequestError.JsonAnalysisToObject(value);

            instance.Error = requestError != null
                ? requestError as RequestError
                : new RequestError
                {
                    ErrorCode = -1,
                    ErrorMessage = $"json 无法被有效序列化为 {typeof(T).Name}、{nameof(IRequestError)}!\n{value}"
                };

            return instance;
        }
    }
}