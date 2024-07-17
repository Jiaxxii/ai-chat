using System;
using UnityEngine;

namespace Xiyu.AI.LargeLanguageModel.Service.Request
{
    [Serializable]
    public class MemoryModule : SerializeParameterModule
    {
        [SerializeField] private bool enableSystemMemory;
        [SerializeField] private string systemMemoryId;

        public bool EnableSystemMemory
        {
            get => enableSystemMemory;
            set => enableSystemMemory = value;
        }

        public string SystemMemoryId
        {
            get => systemMemoryId;
            set => systemMemoryId = value;
        }

        public override bool IsDefault() => !enableSystemMemory && string.IsNullOrEmpty(systemMemoryId);

    }
}