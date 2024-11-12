using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;
using Xiyu.LoggerSystem;

namespace Xiyu.VirtualLiveRoom.AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        private static readonly Lazy<AudioManager> Lazy = new(FindObjectOfType<AudioManager>);
        public static AudioManager Instance => Lazy.Value;

        private ObjectPool<AudioSource> _audioSourcePool;

        private readonly Dictionary<string, IAudioOperator> _audioPlayOperatorMap = new();

        private void Awake()
        {
            _audioSourcePool = new ObjectPool<AudioSource>(() => gameObject.AddComponent<AudioSource>());
            AppendAudioOperator(new Bgm(), null);
            AppendAudioOperator(new Sound(), null);
        }

        public void AppendAudioOperator([NotNull] IAudioOperator audioOperator, [CanBeNull] string label)
        {
            if (!_audioPlayOperatorMap.TryAdd(audioOperator.OperatorType, audioOperator))
            {
                LoggerManager.Instance.LogWarn($"重复添加的\"{audioOperator.OperatorType}\"");
            }

            var audioSource = _audioSourcePool.Get();
            audioOperator.Init(audioSource, label, ao =>
            {
                _audioPlayOperatorMap.Remove(ao.OperatorType);
                _audioSourcePool.Release(audioSource);
            });
        }

        [NotNull]
        public IAudioOperator GetAudioOperatorPlayer([NotNull] string type)
        {
            return _audioPlayOperatorMap[type];
        }
    }
}