using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Xiyu.AIChat.LargeLanguageModel.BaiDu;
using Config = Xiyu.AIChat.SpeechSynthesisTechnology.Baidu.Config;

namespace Xiyu.AIChat.SpeechSynthesisTechnology.Baidu
{
    // ReSharper disable once InconsistentNaming
    public class BaiduSettingsSST : MonoBehaviour
    {
        [SerializeField] private string apiKey;
        [SerializeField] private string secretKey;
        [SerializeField] private string token;
        [SerializeField] private string saveFolder;
        [SerializeField] private Config config;

        public string APIKey => apiKey;

        public string SecretKey => secretKey;
        public string Token => token;

        public Config Config => config;

        public string SaveFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, saveFolder);

        private void Awake()
        {
            StartCoroutine(GetToken());
            if (!Directory.Exists(SaveFolder))
            {
                Directory.CreateDirectory(SaveFolder);
            }
        }


        private IEnumerator GetToken()
        {
            yield return BaiduSettings.GeToken(apiKey, secretKey, result => token = result);
        }
    }
}