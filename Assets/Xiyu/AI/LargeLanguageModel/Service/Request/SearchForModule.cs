using System;
using UnityEngine;

namespace Xiyu.AI.LargeLanguageModel.Service.Request
{
    [Serializable]
    public class SearchForModule : SerializeParameterModule
    {
        [SerializeField] private bool disableSearch;
        [SerializeField] private bool enableCitation;
        [SerializeField] private bool enableTrace;

        public bool DisableSearch
        {
            get => disableSearch;
            set => disableSearch = value;
        }

        public bool EnableCitation
        {
            get => enableCitation;
            set => enableCitation = value;
        }

        public bool EnableTrace
        {
            get => enableTrace;
            set => enableTrace = value;
        }

        public override bool IsDefault() => !disableSearch && !enableCitation && !enableTrace;
    }
}