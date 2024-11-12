using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xiyu.LoggerSystem;
using Xiyu.VirtualLiveRoom.Component;

namespace Xiyu.Settings
{
    [CreateAssetMenu(fileName = "New PageInfoSetting", menuName = "ScriptableObject/WebPageInfoSetting", order = 0)]
    public class WebPageInfoSettingSo : ScriptableObject, IEnumerable<PageInfo>
    {
        [SerializeField] private List<PageInfo> webPageInfoCollections;

        private Dictionary<string, PageInfo> _webPageInfoSetMap;


        private void Awake()
        {
            _webPageInfoSetMap = new Dictionary<string, PageInfo>();
            foreach (var info in webPageInfoCollections.Where(info => !_webPageInfoSetMap.TryAdd(info.Url, info)))
            {
               LoggerManager.Instance.LogWarn($"重复的key:\"{info.Url}\"");
            }
        }


        public bool TryGetPageInfo(string url, out PageInfo webPageInfo) => _webPageInfoSetMap.TryGetValue(url, out webPageInfo);

        public bool Contains(string url) => _webPageInfoSetMap.ContainsKey(url);

        public IEnumerator<PageInfo> GetEnumerator()
        {
            if (_webPageInfoSetMap is null or { Count: 0 })
            {
                Awake();
            }

            return _webPageInfoSetMap!.Select(v => v.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}