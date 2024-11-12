using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using Xiyu.LoggerSystem;
using Xiyu.VirtualLiveRoom.AudioSystem;
using Xiyu.VirtualLiveRoom.Component;
using Xiyu.VirtualLiveRoom.Component.DanmuItem.Data;
using Xiyu.VirtualLiveRoom.EventFunctionSystem;

namespace Xiyu.VirtualLiveRoom.View.LiveRoomView
{
    public sealed class LiveRoomTopInfo : UIContainer
    {
        [SerializeField] private TextMeshProUGUI nameText;

        [SerializeField] private TextMeshProUGUI introduceText;

        [SerializeField] private TextMeshProUGUI visitorText;

        [SerializeField] private TextMeshProUGUI hotText;

        [SerializeField] private DanmuController danmuController;

        // [SerializeField] private 

        public int Hot
        {
            get => Convert.ToInt32(hotText.text.Substring(0, hotText.text.IndexOf('<')));
            set => hotText.text = $"{value}<space=10>热度";
        }

        public int Visitor
        {
            get => Convert.ToInt32(visitorText.text.Substring(0, visitorText.text.IndexOf('<')));
            set => visitorText.text = $"{value}<space=10>看过";
        }


        private void Awake()
        {
            danmuController.MessageSender.MessageBox.Interactable = false;
            DanmuController.OnDanmuSend += OnDanmuSendEventHandler;
        }

        private async void Start()
        {
            //====加载房间信息
            var roomInfo = await LoadLiveRoomInfo();
            if (roomInfo != LiveRoomInfo.None)
            {
                nameText.text = roomInfo.AnchorName;
                introduceText.text = roomInfo.AnchorPersonalIntroduction;
                Hot = roomInfo.Hot;
                Visitor = roomInfo.Visitor;
            }

            //---- 加载历史弹幕
            var history = await LoadLiveRoomHistory();
            if (history == HistoricalRecord.None || history.HistoricalRecords!.Length <= 0) return;

            foreach (var danmuInfo in history.HistoricalRecords)
            {
                var danmu = await danmuController.SendDanmu(danmuInfo.DanmuContent.Content, false);
                danmu.UpdateData(danmuInfo.Danmu);
                danmu.UserName.UpdateData(danmuInfo.DanmuUserName);
                danmu.Content.UpdateData(danmuInfo.DanmuContent);
                danmu.Head.UpdateData(danmuInfo.DanmuHead);
            }
        }


        [WebContentInit(ThenAfterInitialization = typeof(SubtitlesBar))]
        protected override UniTask Initialization(CancellationToken cancellationToken = default)
        {
            danmuController.MessageSender.MessageBox.Interactable = true;
            return UniTask.CompletedTask;
        }

        private void OnDanmuSendEventHandler(DanmuData danmuData)
        {
            Hot++;
            SaveLiveRoomInfoAndHistory().Forget();
        }


        private async UniTaskVoid SaveLiveRoomInfoAndHistory()
        {
            var liveRoomInfo = new LiveRoomInfo(Aishi.Name, Aishi.Introduce, Hot, Visitor);

            var jsonContent = JsonConvert.SerializeObject(liveRoomInfo, Formatting.Indented);


            Directory.CreateDirectory(Application.ApplicationData.LiveRoomInfoPath);

            var roomInfoFileName = $"room info {DateTime.Now:yyyy-MM-dd}.json";
            await File.WriteAllTextAsync(Path.Combine(Application.ApplicationData.LiveRoomInfoPath, roomInfoFileName), jsonContent);


            var danmus = danmuController.GetDanmuData().ToArray();
            var historicalRecord = new HistoricalRecord(danmus);

            var historyJsonContent = JsonConvert.SerializeObject(historicalRecord);

            var historicalRecordFileName = $"history {DateTime.Now:yyyy-MM-dd}.json";
            await File.WriteAllTextAsync(Path.Combine(Application.ApplicationData.LiveRoomInfoPath, historicalRecordFileName), historyJsonContent);


#if UNITY_EDITOR
            await LoggerManager.Instance.LogInfoAsync($"直播信息保存在：{Application.ApplicationData.LiveRoomInfoPath}");
#endif
        }

        public static async UniTask<LiveRoomInfo> LoadLiveRoomInfo()
        {
            var fileName = $"room info {DateTime.Now:yyyy-MM-dd}.json";
            var fileInfo = new FileInfo(Path.Combine(Application.ApplicationData.LiveRoomInfoPath, fileName));

            if (!fileInfo.Directory!.Exists)
            {
                fileInfo.Directory.Create();
                return LiveRoomInfo.None;
            }

            if (!fileInfo.Exists)
            {
                return LiveRoomInfo.None;
            }

            var jsonContent = await File.ReadAllTextAsync(fileInfo.FullName, Encoding.UTF8);
            return JsonConvert.DeserializeObject<LiveRoomInfo>(jsonContent);
        }

        public static async UniTask<HistoricalRecord> LoadLiveRoomHistory()
        {
            var fileName = $"history {DateTime.Now:yyyy-MM-dd}.json";
            var fileInfo = new FileInfo(Path.Combine(Application.ApplicationData.LiveRoomInfoPath, fileName));

            if (!fileInfo.Directory!.Exists)
            {
                fileInfo.Directory.Create();
                return HistoricalRecord.None;
            }

            if (!fileInfo.Exists)
            {
                return HistoricalRecord.None;
            }

            var jsonContent = await File.ReadAllTextAsync(fileInfo.FullName, Encoding.UTF8);
            return JsonConvert.DeserializeObject<HistoricalRecord>(jsonContent);
        }
    }
}