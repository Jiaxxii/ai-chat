using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Xiyu.AI.LargeLanguageModel.Service.Request
{
    public abstract class SerializeParameterModule
    {
        [JsonIgnore]
        public virtual JsonSerializerSettings JsonSerializerSettings { get; } = new()
        {
            Formatting = Formatting.None,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };
 
        public abstract bool IsDefault();

        public JObject ToJObject() => JObject.FromObject(this, JsonSerializer.Create(JsonSerializerSettings));

        public string ToJson(JsonSerializerSettings jss = null) => JsonConvert.SerializeObject(this, jss ?? JsonSerializerSettings);
    }
}