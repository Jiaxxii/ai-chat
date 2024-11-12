using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.VirtualLiveRoom.Component.DanmuItem.Data;
using Xiyu.VirtualLiveRoom.Component.DanmuMsgSender;
using Danmu = Xiyu.VirtualLiveRoom.Component.DanmuItem.Danmu;

namespace Xiyu.VirtualLiveRoom.View
{
    public class DanmuController : MonoBehaviour
    {
        [SerializeField] private ScrollRect danmuListScrollRect;
        [SerializeField] private RectTransform danmuBoxRectTransform;
        [SerializeField] private DanmuMessageSender messageSender;

        [SerializeField] private float rollDurationSeconds;
        [SerializeField] private Ease rollMoveEase = Ease.OutQuart;


        private readonly List<Danmu> _danmuCollects = new();

        public int Count => _danmuCollects.Count;

        public static event Action<DanmuData> OnDanmuSend;

        public DanmuMessageSender MessageSender => messageSender;


        private bool _rollMove;
        private float _current;


        private void Start()
        {
            messageSender.MessageBox.OnMessageSend += UniTask.UnityAction<string>(OnSendMessageEventHandler);
        }

        private async UniTaskVoid OnSendMessageEventHandler(string msg)
        {
            // 这里的消息已经被 messageSender.MessageBox.SubmitCheck 筛选过了
            var danmu = await Danmu.CreateAsync(danmuBoxRectTransform, User.UserHeadSprite, User.UserName, msg);
            SendDanmu(danmu, true);
        }

        public IEnumerable<DanmuData> GetDanmuData()
        {
            return _danmuCollects.Select(danmu => new DanmuData(danmu.ReadOnlyData, danmu.Content.ReadOnlyData, danmu.Head.ReadOnlyData, danmu.UserName.ReadOnlyData));
        }

        public IEnumerable<DanmuData> GetDanmuData(int lastCount)
        {
            return _danmuCollects
                .TakeLast(lastCount)
                .Select(danmu => new DanmuData(danmu.ReadOnlyData, danmu.Content.ReadOnlyData, danmu.Head.ReadOnlyData, danmu.UserName.ReadOnlyData));
        }

        public async UniTask<Danmu> SendDanmu(string message, bool triggerAction = true)
        {
            var danmu = await Danmu.CreateAsync(danmuBoxRectTransform, User.UserHeadSprite, User.UserName, message);
            SendDanmu(danmu, triggerAction);

            return danmu;
        }

        // 发送一条弹幕到弹幕区域
        public void SendDanmu(Danmu danmu, bool triggerAction = true)
        {
            _danmuCollects.Add(danmu);
            var danmuData = new DanmuData(danmu.ReadOnlyData, danmu.Content.ReadOnlyData, danmu.Head.ReadOnlyData, danmu.UserName.ReadOnlyData);

            if (triggerAction)
                OnDanmuSend?.Invoke(danmuData);

            if (_rollMove)
            {
                return;
            }

            _rollMove = true;
            MoveRollCoroutine();
        }

        private void MoveRollCoroutine()
        {
            DOTween.To(() => danmuListScrollRect.verticalNormalizedPosition, v => danmuListScrollRect.verticalNormalizedPosition = v
                    , 0, rollDurationSeconds)
                .SetEase(rollMoveEase)
                .OnComplete(() => _rollMove = false);
        }
    }
}