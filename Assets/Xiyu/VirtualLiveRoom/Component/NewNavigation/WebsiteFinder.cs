using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Xiyu.Settings;

namespace Xiyu.VirtualLiveRoom.Component.NewNavigation
{
    /// <summary>
    /// 网址查询器
    /// </summary>
    public class WebsiteFinder
    {
        // private static readonly Lazy<WebsiteFinder> Lazy = new(FindObjectOfType<WebsiteFinder>);
        // public static WebsiteFinder Instance => Lazy.Value;

        // 网页配置文件
        // [SerializeField] private WebPageConfigSo webPageConfigs;

        private static readonly List<PageInfo> PageInfosCollector = new();

        private static readonly HashSet<string> SingleUrlMap = new();


        public static IEnumerator LoadPageInfoSettings(string folder = "Settings", string fileName = "WebPageInfoSettings")
        {
            var resHandle = Resources.LoadAsync<WebPageInfoSettingSo>($"{folder}/{fileName}");
            yield return resHandle;

            var settings = (WebPageInfoSettingSo)resHandle.asset;
            PageInfosCollector.Clear();
            SingleUrlMap.Clear();
            foreach (var info in settings)
            {
                PageInfosCollector.Add(info);
                SingleUrlMap.Add(info.Url);
            }
        }

        public static async UniTask LoadPageInfoSettingsAsync(string folder = "Settings", string fileName = "WebPageInfoSettings")
        {
            var settings = (WebPageInfoSettingSo)await Resources.LoadAsync<WebPageInfoSettingSo>($"{folder}/{fileName}");
            PageInfosCollector.Clear();
            SingleUrlMap.Clear();
            foreach (var info in settings)
            {
                PageInfosCollector.Add(info);
                SingleUrlMap.Add(info.Url);
            }
        }


        /// <summary>
        /// 检查网址的安全等级
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static WebsiteSecurityLevel CheckWebsiteSecurityAuth(string url)
        {
            return SingleUrlMap.Contains(url) ? PageInfosCollector.Find(v => v.Url == url).SecurityLevel : WebsiteSecurityLevel.Null;
        }


        /// <summary>
        /// 检查网址的安全等级
        /// </summary>
        /// <param name="url"></param>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async UniTask<WebsiteSecurityLevel> CheckWebsiteSecurityAuthAsync(string url, string folder = "Settings", string fileName = "WebPageInfoSettings")
        {
            if (PageInfosCollector.Count == 0 || SingleUrlMap.Count == 0)
            {
                await LoadPageInfoSettingsAsync(folder, fileName);
            }

            return SingleUrlMap.Contains(url) ? PageInfosCollector.Find(v => v.Url == url).SecurityLevel : WebsiteSecurityLevel.Null;
        }

        /// <summary>
        /// 尝试查找网址来获取<see cref="PageInfo"/>配置文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public static bool TryFindWebsitePageInfo(string url, out PageInfo pageInfo)
        {
            if (!SingleUrlMap.Contains(url))
            {
                pageInfo = PageInfo.NotImplemented();
                return false;
            }

            var info = PageInfosCollector.Find(v => v.Url == url);

            pageInfo = info;
            return true;
        }
    }
}