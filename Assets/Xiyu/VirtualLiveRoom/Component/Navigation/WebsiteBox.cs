#if OldCode
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Xiyu.Constant;
using Xiyu.VirtualLiveRoom.Component.NewNavigation;
using Xiyu.VirtualLiveRoom.Tools.Addressabe;

namespace Xiyu.VirtualLiveRoom.Component.Navigation
{
    [Obsolete("组件已经弃用，请使用\"Xiyu.VirtualLiveRoom.Component.NewNavigation.Website\"",false)]
    public class WebsiteBox : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        public event UnityAction<PageInfo> OnWebsiteSubmit;


        public List<PageInfo> History { get; private set; }

        public PageInfo CurrentUrl => History.Last();


        private void Start()
        {
            inputField.onSubmit.AddListener(UniTask.UnityAction<string>(async url =>
            {
                inputField.text = url;
                var isEffectiveUrl = WebsiteFinder.TryFindWebsitePageInfo(url, out var pageInfo);

                await UniTask.WaitForEndOfFrame(this);
                throw new NotImplementedException($"组件已经弃用，请使用\"{typeof(Website).FullName}\"");
                // if (NavigationCenter.ContainsView(isEffectiveUrl ? pageInfo.Url : WebsiteCenter.TimeURL))
                // {
                //     await NavigationCenter.Instance.SwitchWebContentAsync(isEffectiveUrl ? pageInfo.Url : WebsiteCenter.TimeURL, default);
                // }
                // else
                // {
                //     await NavigationCenter.Instance.AppendViewAsync(isEffectiveUrl ? pageInfo.Url : WebsiteCenter.TimeURL);
                // }
                //
                // if (isEffectiveUrl)
                // {
                //     OnWebsiteSubmitEventHandler(pageInfo);
                // }
                // else
                // {
                //     _ = WebsiteFinder.TryFindWebsitePageInfo(url, out var timePageInfo);
                //     OnWebsiteSubmitEventHandler(timePageInfo);
                // }
            }));
        }

        public async UniTask SetWebsite(string url, bool triggerCallback = true)
        {
            inputField.text = url;
            if (triggerCallback)
            {
                var isEffectiveUrl = WebsiteFinder.TryFindWebsitePageInfo(url, out var pageInfo);
                await UniTask.WaitForEndOfFrame(this);
                throw new NotImplementedException($"组件已经弃用，请使用\"{typeof(Website).FullName}\"");
                // if (NavigationCenter.ContainsView(isEffectiveUrl ? pageInfo.Url : WebsiteCenter.TimeURL))
                // {
                //     await NavigationCenter.Instance.SwitchWebContentAsync(isEffectiveUrl ? pageInfo.Url : WebsiteCenter.TimeURL, default);
                // }
                // else
                // {
                //     await NavigationCenter.Instance.AppendViewAsync(isEffectiveUrl ? pageInfo.Url : WebsiteCenter.TimeURL);
                // }
                //
                // if (isEffectiveUrl)
                // {
                //     OnWebsiteSubmitEventHandler(pageInfo);
                // }
                // else
                // {
                //     _ = WebsiteFinder.TryFindWebsitePageInfo(url, out var timePageInfo);
                //     OnWebsiteSubmitEventHandler(timePageInfo);
                // }
            }
        }


        private void OnWebsiteSubmitEventHandler(PageInfo pageInfo)
        {
            History.Add(pageInfo);
            OnWebsiteSubmit?.Invoke(pageInfo);
        }
    }
}
#endif