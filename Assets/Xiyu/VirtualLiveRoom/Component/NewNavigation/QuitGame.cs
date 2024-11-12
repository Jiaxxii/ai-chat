using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.AI.LargeLanguageModel.Service.Request;
using Xiyu.Application;
using Xiyu.VirtualLiveRoom.View.LiveRoomView;

namespace Xiyu.VirtualLiveRoom.Component.NewNavigation
{
    public class QuitGame : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.AddListener(UniTask.UnityAction(OnClickAsync));
        }

        public static async UniTaskVoid OnClickAsync()
        {
            var chatProcessor = FindObjectOfType<ChatProcessor>();
            if (chatProcessor != null)
            {
                Directory.CreateDirectory(ApplicationData.LiveRoomInfoPath);
                var fileInfo = new FileInfo(Path.Combine(ApplicationData.LiveRoomInfoPath, $"chat history({DateTime.Now:yyyy-MM-dd}).json"));

                string historyJson;
                if (fileInfo.Exists)
                {
                    var oldHistory = JArray.Parse(await File.ReadAllTextAsync(fileInfo.FullName));
                    var newHistory = JArray.FromObject(new List<Message>(chatProcessor));

                    var combine = new JArray(oldHistory.Concat(newHistory));

                    historyJson = combine.ToString(Formatting.Indented);
                }
                else
                {
                    historyJson = JsonConvert.SerializeObject(new List<Message>(chatProcessor), Formatting.Indented);
                }


                await File.WriteAllTextAsync(fileInfo.FullName, historyJson);
            }
#if UNITY_EDITOR
            Debug.Log($"游戏已经退出!(\"<color=red>{Path.Combine(ApplicationData.LiveRoomInfoPath, $"chat history({DateTime.Now:yyyy-MM-dd}).json")}</color>\")");
#endif
            UnityEngine.Application.Quit();
        }
    }
}