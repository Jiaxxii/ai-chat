using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom.AudioSystem
{
    public interface IAudioOperator : IDisposable
    {
        UniTask<IAudioOperator> Init([NotNull] AudioSource audioSource, [CanBeNull] string audioLabelType, [CanBeNull] Action<IAudioOperator> onDispose);

        string OperatorType { get; }

        bool IsPlaying { get; }


        UniTask<IAudioOperator> SetClip([NotNull] string audioName);


        void Play();
        UniTask Play([NotNull] string audioName);

        IAudioOperator SetTime(float time);
        IAudioOperator SetNormalizeTim(float time);


        IAudioOperator SetLoop(bool loop);


        UniTask PlayFadein(float duration);


        IAudioOperator SetVolume(float volume);


        IAudioOperator Reset();

        void Pause();
        void Stop();
        void Release([NotNull] string audioName);
    }
}