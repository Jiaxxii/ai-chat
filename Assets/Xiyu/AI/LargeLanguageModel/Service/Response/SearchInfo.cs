using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Xiyu.AI.LargeLanguageModel.Service.Response
{
    [Serializable]
    public class SearchInfo
    {
        public List<SearchResult> SearchResults
        {
            get => searchResults;
            set => searchResults = value;
        }

        [SerializeField] private List<SearchResult> searchResults;

        public bool IsDefaultOrNull() => SearchResults is null || SearchResults.Count == 0;
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

        [JsonProperty(PropertyName = "url")]
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