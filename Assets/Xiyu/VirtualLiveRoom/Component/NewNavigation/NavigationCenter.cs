using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using Xiyu.Settings;
using Xiyu.VirtualLiveRoom.EventFunctionSystem;
using Debug = UnityEngine.Debug;

namespace Xiyu.VirtualLiveRoom.Component.NewNavigation
{
    public class WebContentPair
    {
        public WebContentPair(Tab tab, WebViewContent webViewContent)
        {
            Tab = tab;
            WebViewContent = webViewContent;
        }

        public Tab Tab { get; }
        public WebViewContent WebViewContent { get; }
    }


    public class NavigationCenter : Expand.Singleton<NavigationCenter>
    {
        [SerializeField] private Transform viewContent;
        [SerializeField] private List<string> startUrlView;

        [SerializeField] private TabDragControl dragControl;

        [SerializeField] private Website website;

        private static readonly Dictionary<string, WebContentPair> WebPageMap = new();


        // private static string _lastWebUrl;

        private static WebContentPair _currentWebContentPair;

        private CancellationTokenSource _tagSwitchToCancellationTokenSource;

        protected override async void Awake()
        {
            base.Awake();
            // 加载配置文件
            await WebsiteFinder.LoadPageInfoSettingsAsync();
        }

        private async void Start()
        {
            var startUrls = new HashSet<string>(startUrlView);

            if (startUrlView.Count == 0)
            {
                Debug.LogError("初始网页网址必须包含至少一个的有效的网址！");
            }

            foreach (var url in startUrls)
            {
                await AppendViewAsync(url);
            }
        }

        private bool _websiteLoadingComplete;

        public async UniTask AppendViewAsync(string url)
        {
            await InitializedView(url);
        }

        private async UniTask InitializedView(string url)
        {
            // 加载网页内容加载器
            var webContentRefDev = (WebViewContentReferenceDeviceSo)await Resources.LoadAsync<WebViewContentReferenceDeviceSo>("Settings/WebViewContentRef");

            // 判断网址有效性
            if (!WebsiteFinder.TryFindWebsitePageInfo(url, out var webPageInfo))
            {
                Debug.LogWarning($"无效网址：{url}");
                // TODO
                return;
            }

            // 创建选项卡生成异步任务
            var createTabTask = dragControl.CreateTabAsync(webPageInfo);
            // 创建网页内容生成异步任务
            var loadWebViewContentTask = webContentRefDev.LoadComponentAssetAsync<WebViewContent>(webPageInfo.Url, viewContent);


            var loadAllTask = UniTask.WhenAll(createTabTask, loadWebViewContentTask);

            _websiteLoadingComplete = false;
            website.SendNewUrlForGet(webPageInfo, () => _websiteLoadingComplete).Forget();

            var (tab, webViewContent) = await loadAllTask;
            if (_currentWebContentPair == null)
            {
                webViewContent.SetCanvasGroupActive(false);
            }
            else
            {
                _currentWebContentPair.WebViewContent.SetCanvasGroupActive(false);
                webViewContent.SetCanvasGroupActive(false);
            }


            tab.EventCenter.OnTagPointerClick += OnTagPointerClickEventHandler;

            tab.EventCenter.OnTabClose += webInfo => CloseView(webInfo.Url);

            var webContentPair = new WebContentPair(tab, webViewContent);
            WebPageMap.Add(webPageInfo.Url, webContentPair);

            var valueTuple = GetInstanceAttributeAnsInitMethodsUniTask(webViewContent, tab.GetCancellationTokenOnDestroy());
            if (!valueTuple.initAttribute.ShouldInitialize)
            {
                _websiteLoadingComplete = true;
                FocusViewWindow(webPageInfo.Url);
                return;
            }

            // 如果为 true 表示在 初始化 发生在 网页内容 显示前 (先初始化后显示)
            if (valueTuple.initAttribute.FinalInitialization)
            {
                await valueTuple.initTask;
                _websiteLoadingComplete = true;
                FocusViewWindow(webPageInfo.Url);
            }
            else
            {
                FocusViewWindow(webPageInfo.Url);
                await valueTuple.initTask;

                _websiteLoadingComplete = true;
            }
        }


        private static (WebContentInitAttribute initAttribute, UniTask initTask) GetInstanceAttributeAnsInitMethodsUniTask(WebViewContent instance,
            CancellationToken cancellationToken)
        {
            var type = instance.GetType();

            // 非公开 非静态 返回值为 <UniTask> 参数包含 <CancellationToken> 的方法
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(info => info.ReturnType == typeof(UniTask)
                                        && info.GetParameters()
                                            .Any(p => p.ParameterType == typeof(CancellationToken)));

            if (methods is null)
            {
                throw new NotImplementedException(
                    $"类'{type}'缺少一个必要的方法，该方法应具有以下签名：'NotPublic {typeof(UniTask).FullName} UserInitializationName ({typeof(CancellationToken).FullName} cancellationToken)'。此外，建议为该方法添加特性'[{typeof(WebContentInitAttribute).FullName}]'。");
            }

            var initAttribute = methods.GetCustomAttribute<WebContentInitAttribute>();

            if (initAttribute is null or { ShouldInitialize: false })
            {
                return (initAttribute, UniTask.CompletedTask);
            }

            return (initAttribute, (UniTask)methods.Invoke(instance, new object[] { initAttribute.NotUsedCancellationToken ? default : cancellationToken }));
        }

        public static void FocusViewWindow(string url)
        {
            // 检测网址是否存在
            if (!ContainsView(url))
            {
                return;
            }

            if (_currentWebContentPair == null)
            {
                _currentWebContentPair = WebPageMap[url];
                _currentWebContentPair.WebViewContent.SetCanvasGroupActive(true);
                return;
            }

            // 点击的窗口是当前窗口
            if (_currentWebContentPair.Tab.PageInfo.Url == url)
            {
                return;
            }

            // 关闭当前网址 显示目标网址
            _currentWebContentPair.WebViewContent.SetCanvasGroupActive(false);
            _currentWebContentPair.WebViewContent.WebPageLostFocus();

            WebPageMap[url].WebViewContent.SetCanvasGroupActive(true);
            WebPageMap[url].WebViewContent.WebPageFocus();

            _currentWebContentPair = WebPageMap[url];
        }

        private static void OnTagPointerClickEventHandler(PageInfo targetPageInfo, PointerEventData eventData)
        {
            // 聚焦窗口
            FocusViewWindow(targetPageInfo.Url);
        }

        public static bool ContainsView(string url) => WebPageMap.ContainsKey(url);


        public async UniTask AppendOrOpenViewAsync(string url, CancellationToken cancellationToken)
        {
            if (ContainsView(url))
            {
                await SwitchWebContentAsync(url, cancellationToken);
            }
            else
            {
                await AppendViewAsync(url);
            }
        }

        public bool CloseView(string url)
        {
            if (WebPageMap.Count == 1)
            {
                return false;
            }

            if (WebPageMap.Remove(url, out var webContentPair) == false)
            {
                return false;
            }

            dragControl.RemoveTag(webContentPair.Tab);
            UnityEngine.Object.Destroy(webContentPair.WebViewContent.gameObject);


            FocusViewWindow(dragControl.LastTab.PageInfo.Url);

            return true;
        }


        public async UniTask SwitchWebContentAsync(string url, CancellationToken cancellationToken)
        {
            FocusViewWindow(url);
            await UniTask.CompletedTask;
        }
    }
}