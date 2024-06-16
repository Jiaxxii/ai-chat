using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace Xiyu.AIChat.SpeechSynthesisTechnology.Baidu
{
    public class BaiduCompositeService : SST
    {
        [FormerlySerializedAs("baiduSettingsTTS")] [SerializeField]
        private BaiduSettingsSST baiduSettingsSst;

        private readonly Dictionary<string, AudioClip> _history = new();

        public override IEnumerator Request(string text, Action<AudioClip> onAudioComplete)
        {
            if (_history.TryGetValue(text, out var value))
            {
                onAudioComplete.Invoke(value);
                yield break;
            }

            var form = GetForm(new RequestData
            {
                Tex = WebUtility.UrlEncode(text),
                Token = baiduSettingsSst.Token,
                CuID = baiduSettingsSst.Config.CuId,
                CTP = baiduSettingsSst.Config.Ctp,
                Language = baiduSettingsSst.Config.Language,
                Pitch = baiduSettingsSst.Config.Pitch,
                Volume = baiduSettingsSst.Config.Volume,
                SoundLibrary = baiduSettingsSst.Config.SoundLibrary,
                Aue = baiduSettingsSst.Config.Aue,
            });


            using var request = UnityWebRequest.Post("https://tsn.baidu.com/text2audio", form);

            request.SetRequestHeader("Content-Type", "audio/mp3");

            request.uploadHandler = new UploadHandlerRaw(form.data);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 保存到磁盘
                var filePath = SaveTo(request.downloadHandler.data, text, "mp3");
                yield return LoadAudioClip(filePath, AudioType.MPEG, audioClip =>
                {
                    _history.Add(text, audioClip);
                    onAudioComplete?.Invoke(audioClip);
                });
            }
            else
            {
                Debug.LogError($"{request.error}");
            }
        }

        private string SaveTo(byte[] data, string fileName, string extensionName)
        {
            var filePath = Path.Combine(baiduSettingsSst.SaveFolder, $"{fileName}.{extensionName}");
            Debug.Log($"<color=red>{filePath}</color>");
            File.WriteAllBytes(filePath, data);
            return filePath;
        }

        private IEnumerator LoadAudioClip(string filePath, AudioType audioType, Action<AudioClip> onAudioComplete)
        {
            // 加载
            using var www = UnityWebRequestMultimedia.GetAudioClip(filePath, audioType);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                var audioClip = DownloadHandlerAudioClip.GetContent(www);
                onAudioComplete?.Invoke(audioClip);
            }
            else
            {
                Debug.LogError($"{www.error}");
            }

        }

        private static WWWForm GetForm(RequestData requestData)
        {
            var form = new WWWForm();
            form.AddField("tex", requestData.Tex);
            form.AddField("tok", requestData.Token);
            form.AddField("cuid", requestData.CuID);
            form.AddField("ctp", requestData.CTP);
            form.AddField("lan", requestData.Language);

            form.AddField("spd", requestData.Speed);
            form.AddField("pit", requestData.Pitch);
            form.AddField("vol", requestData.Volume);
            form.AddField("per", requestData.SoundLibrary);
            form.AddField("aue", requestData.Aue);
            return form;
        }
    }
}