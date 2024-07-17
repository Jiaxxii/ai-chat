using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Xiyu.AI.LargeLanguageModel.Service.Response
{
    [Serializable]
    public class ResponseModule : DeserializeParameterModule
    {
        [SerializeField] private string id;

        // object
        [SerializeField] private string type;

        [SerializeField] private int created;

        [SerializeField] private bool isTruncated;

        [SerializeField] private string result;

        [SerializeField] private bool needClearHistory;

        [SerializeField] private int banRound;

        [SerializeField] private SearchInfo searchInfo;

        public SearchInfo SearchInfo
        {
            get => searchInfo;
            set => searchInfo = value;
        }

        public string ID
        {
            get => id;
            set => id = value;
        }

        [JsonProperty(PropertyName = "object")]
        public string Type
        {
            get => type;
            set => type = value;
        }

        public int Created
        {
            get => created;
            set => created = value;
        }

        public bool IsTruncated
        {
            get => isTruncated;
            set => isTruncated = value;
        }

        public override string Result
        {
            get => result;
            set => result = value;
        }

        public bool NeedClearHistory
        {
            get => needClearHistory;
            set => needClearHistory = value;
        }

        public int BanRound
        {
            get => banRound;
            set => banRound = value;
        }

        public override bool IsDefaultOrNull() => string.IsNullOrEmpty(Result);
    }
}