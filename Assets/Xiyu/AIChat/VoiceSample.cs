using UnityEngine;
using UnityEngine.UI;
using Xiyu.AIChat.SpeechSynthesisTechnology;

namespace Xiyu.AIChat
{
    public class VoiceSample : MonoBehaviour
    {
        [SerializeField] private SST voiceService;
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                StartCoroutine(voiceService.Request("你好，我叫爱实！", au =>
                {
                    audioSource.clip = au;
                    audioSource.Play();
                }));
            });
        }
    }
}