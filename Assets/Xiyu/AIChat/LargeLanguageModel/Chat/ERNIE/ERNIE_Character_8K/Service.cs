﻿using System;
using Unity.Collections;
using UnityEngine;
using Xiyu.AIChat.LargeLanguageModel.Service;

namespace Xiyu.AIChat.LargeLanguageModel.Chat.ERNIE.ERNIE_Character_8K
{
    public class Service : LargeLanguageModel<RequestBody, ResponseResult>
    {
        // private IEnumerator Start()
        // {
        //     yield return Request(response =>
        //     {
        //         Debug.Log($"耗时:{Timer.Elapsed.TotalSeconds}s 是否敏感对话:{response.NeedClearHistory && response.BanRound == -1} 回复:{response.Result}");
        //         configSetting.Body.Messages.Add(new Message(RoleType.Assistant, response.Result));
        //     }, Debug.LogError);
        // }
    }


    [Serializable]
    public class ResponseResult : ServiceResponse<string>
    {
        [ReadOnly] [SerializeField] private string result;
        [ReadOnly] [SerializeField] private bool needClearHistory;
        [ReadOnly] [SerializeField] private int banRound;

        public override string Result
        {
            get => result;
            set => result = value;
        }

        public bool NeedClearHistory
        {
            get => needClearHistory;
            set => needClearHistory = value;
        }

        public int BanRound
        {
            get => banRound;
            set => banRound = value;
        }

        // public override IEnumerator SendResultCoroutine(string content, Action<string> onComplete)
        // {
        //     Result = content;
        //     onComplete?.Invoke(Result);
        //     yield break;
        // }
    }
}