using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.VirtualLiveRoom.Component;
using Xiyu.VirtualLiveRoom.Component.DanmuItem;
using Xiyu.VirtualLiveRoom.Component.DanmuMsgSender;
using Xiyu.VirtualLiveRoom.EventFunctionSystem;

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
            SendDanmu(danmu);
        }


        // 发送一条弹幕到弹幕区域
        public void SendDanmu(Danmu danmu)
        {
            _danmuCollects.Add(danmu);
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