using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Xiyu.CharacterIllustrationResource
{
    public class BodyInfoSettings
    {
        public static readonly string MainSettingsFilePath = Path.Combine(Application.ApplicationData.BodyInfoSettingsPath, "main settings.json");

        private static Dictionary<string, BodyInfo> _bodyInfos = new();

        public static IReadOnlyDictionary<string, BodyInfo> Main => _bodyInfos;

        public static async UniTask LoadSettingsAsync()
        {
            _bodyInfos.Clear();

            var directoryInfo = new DirectoryInfo(Application.ApplicationData.BodyInfoSettingsPath);
            string jsonContent;

            if (directoryInfo.Exists)
            {
                if (File.Exists(MainSettingsFilePath))
                {
                    jsonContent = await File.ReadAllTextAsync(MainSettingsFilePath, Encoding.UTF8);
                    _bodyInfos = JsonConvert.DeserializeObject<Dictionary<string, BodyInfo>>(jsonContent);
                    return;
                }
            }
            else
            {
                directoryInfo.Create();
            }

            var handle = Resources.LoadAsync<TextAsset>("Data/new_ai_body");
            await handle;
            jsonContent = ((TextAsset)handle.asset).text;

            await File.WriteAllTextAsync(MainSettingsFilePath, jsonContent);

            _bodyInfos = JsonConvert.DeserializeObject<Dictionary<string, BodyInfo>>(jsonContent);
        }
    }
}