using System;
using UnityEngine;

namespace Xiyu.AI.LargeLanguageModel.Service.Request
{
    [Serializable]
    public class PenaltyModule : SerializeParameterModule
    {
        [SerializeField] [Range(1, 2)] private float penaltyScore = 1F;
        [SerializeField] [Range(-2, 2)] private float frequencyPenalty = 0.1F;
        [SerializeField] [Range(-2, 2)] private float presencePenalty = 0F;

        public float PenaltyScore
        {
            get => penaltyScore;
            set => penaltyScore = value;
        }

        public float FrequencyPenalty
        {
            get => frequencyPenalty;
            set => frequencyPenalty = value;
        }

        public float PresencePenalty
        {
            get => presencePenalty;
            set => presencePenalty = value;
        }

        public override bool IsDefault() => Math.Abs(penaltyScore - 1F) < .0001F && Math.Abs(frequencyPenalty - 0.1F) < 0.0001F && presencePenalty == 0;
    }
}