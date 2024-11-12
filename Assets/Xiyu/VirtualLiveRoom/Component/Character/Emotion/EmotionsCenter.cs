using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom.Component.Character.Emotion
{
    public static class EmotionsCenter
    {
        private static Dictionary<CharacterBasicEmotions, int[]> _emotionsMap = new();
        public static IReadOnlyDictionary<CharacterBasicEmotions, int[]> ReadOnlyDictionary => _emotionsMap;

        public static async UniTask LoadSettingsAsync()
        {
            Directory.CreateDirectory(Application.ApplicationData.EmotionsPath);
            var fileInfo = new FileInfo(Path.Combine(Application.ApplicationData.EmotionsPath, "emotions.json"));


            string jsonContent;
            if (fileInfo.Exists)
            {
                jsonContent = await File.ReadAllTextAsync(fileInfo.FullName, Encoding.UTF8);
            }
            else
            {
                var textAsset = (TextAsset)await Resources.LoadAsync<TextAsset>("Data/emotion");
                jsonContent = textAsset.text;

                await File.WriteAllTextAsync(fileInfo.FullName, jsonContent);
            }

            _emotionsMap = JsonConvert.DeserializeObject<Dictionary<CharacterBasicEmotions, int[]>>(jsonContent, new StringEnumConverter());
        }
    }
}