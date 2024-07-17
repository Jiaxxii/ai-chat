using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Xiyu.AI.LargeLanguageModel.Service.Request
{
    [Serializable]
    public class RequestResource
    {
        [SerializeField] private RequestModule requestRequestModule;
        [SerializeField] private PenaltyModule penaltyModule;
        [SerializeField] private MemoryModule memoryModule;
        [SerializeField] private SearchForModule searchForModule;

        public RequestModule RequestRequestModule
        {
            get => requestRequestModule;
            set => requestRequestModule = value;
        }

        public PenaltyModule PenaltyModule
        {
            get => penaltyModule;
            set => penaltyModule = value;
        }

        public MemoryModule MemoryModule
        {
            get => memoryModule;
            set => memoryModule = value;
        }

        public SearchForModule SearchForModule
        {
            get => searchForModule;
            set => searchForModule = value;
        }


        public string ToJson()
        {
            // 主模块是一定存在的
            var jsonObject = JObject.Parse(RequestRequestModule.ToJson());

            foreach (var module in new SerializeParameterModule[] { PenaltyModule, MemoryModule, SearchForModule }
                         .Where(m => !m.IsDefault()))
            {
                jsonObject.Merge(JObject.Parse(module.ToJson()), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union,
                    PropertyNameComparison = StringComparison.Ordinal,
                    MergeNullValueHandling = MergeNullValueHandling.Ignore
                });
            }


            return jsonObject.ToString(RequestRequestModule.JsonSerializerSettings.Formatting);
        }

        public byte[] ToJson(Encoding encoding) => encoding.GetBytes(ToJson());
        
        
    }
}