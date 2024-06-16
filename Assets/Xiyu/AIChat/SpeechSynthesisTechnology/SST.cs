using System;
using System.Collections;
using UnityEngine;

namespace Xiyu.AIChat.SpeechSynthesisTechnology
{
    public abstract class SST : MonoBehaviour
    {
        // public virtual void PostMessage(RequestData requestData, Action<string, AudioClip> onAudioDataComplete)
        // {
        //     StartCoroutine(Request(requestData, onAudioDataComplete));
        // }

        public abstract IEnumerator Request(string text, Action<AudioClip> onAudioComplete);
    }
}