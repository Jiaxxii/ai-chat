using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Xiyu.AIChat.LargeLanguageModel.Service;

namespace Xiyu.AIChat.LargeLanguageModel.Chat.ERNIE.ERNIE_4._0_8K_Preview_0518
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

        [SerializeField] private SearchInfo searchInfo;

        public SearchInfo SearchInfo
        {
            get => searchInfo;
            set => searchInfo = value;
        }
    }

    [Serializable]
    public class SearchInfo
    {
        [SerializeField] private List<SearchResult> searchResults;

        public List<SearchResult> SearchResults
        {
            get => searchResults;
            set => searchResults = value;
        }
    }

    [Serializable]
    public class SearchResult
    {
        [SerializeField] private int index;
        [SerializeField] private string url;
        [SerializeField] private string title;

        public int Index
        {
            get => index;
            set => index = value;
        }

        public string URL
        {
            get => url;
            set => url = value;
        }

        public string Title
        {
            get => title;
            set => title = value;
        }
    }
}