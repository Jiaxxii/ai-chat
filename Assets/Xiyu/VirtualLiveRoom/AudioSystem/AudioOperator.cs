using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Xiyu.CharacterIllustrationResource;
using Xiyu.Expand;

namespace Xiyu.VirtualLiveRoom.AudioSystem
{
    public abstract class AudioOperator : IAudioOperator
    {
        protected AudioSource AudioSource;

        protected AssetLoader<AudioClip> AssetLoader;

        protected float PlayStartTime;
        protected bool IsLoop;
        protected float Volume = 1;


        /// <summary>
        /// 但有资源被释放后执行回调
        /// </summary>
        internal event Action<IAudioOperator> OnDispose;

        [NotNull] public abstract string OperatorType { get; }

        public bool IsPlaying => AudioSource.isPlaying;


        public virtual async UniTask<IAudioOperator> Init(AudioSource audioSource, string audioLabelType, Action<IAudioOperator> onDispose)
        {
            AudioSource = audioSource;
            AssetLoader = await AssetLoaderCenter<AudioClip>.LoadResourceLocations(audioLabelType ?? OperatorType);

            OnDispose = onDispose;

            return this;
        }

        public virtual async UniTask<IAudioOperator> SetClip(string audioName)
        {
            AudioSource.clip = await AssetLoader.LoadAssetAsync(audioName);
            return this;
        }

        public virtual void Play()
        {
            if (AudioSource.clip != null)
            {
                AudioSource.time = PlayStartTime;
                AudioSource.loop = IsLoop;
                AudioSource.volume = Volume;
                AudioSource.Play();
            }
            else
            {
                throw new NullReferenceException(nameof(AudioSource.clip));
            }
        }

        public virtual async UniTask Play(string audioName)
        {
            (await SetClip(audioName)).Play();
        }

        public virtual async UniTask PlayFadein(float duration)
        {
            Play();
            AudioSource.volume = 0F;
            await DOTween.To(() => AudioSource.volume, v => AudioSource.volume = v, Volume, duration)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }

        public virtual void Pause()
        {
            AudioSource.Pause();
        }

        public virtual void Stop()
        {
            AudioSource.Stop();
        }


        public virtual IAudioOperator SetVolume(float volume)
        {
            Volume = volume;
            return this;
        }

        public virtual IAudioOperator SetTime(float time)
        {
            PlayStartTime = time;
            return this;
        }

        public virtual IAudioOperator SetLoop(bool loop)
        {
            IsLoop = loop;
            return this;
        }

        public virtual IAudioOperator Reset()
        {
            Volume = 1;
            PlayStartTime = 0;
            IsLoop = false;
            return this;
        }


        public virtual IAudioOperator SetNormalizeTim(float time)
        {
            if (AudioSource.clip == null)
                throw new NullReferenceException(nameof(AudioSource.clip));

            PlayStartTime = time.MapFloat(0, 1, 0, AudioSource.clip.length);
            return this;
        }


        public virtual void Release(string audioName)
        {
            if (AudioSource.clip == null)
            {
                return;
            }

            AudioSource.Stop();
            AssetLoader.Release(audioName);
            AudioSource.clip = null;
        }

        public virtual void Dispose()
        {
            if (AudioSource.clip == null)
            {
                AudioSource.Stop();
                AudioSource.clip = null;
            }

            AssetLoader.ReleaseAll();
            OnDispose?.Invoke(this);
        }
    }
}