using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Xiyu.LoggerSystem;
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
                await LoggerManager.Instance.LogErrorAsync("初始网页网址必须包含至少一个的有效的网址！");
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
            var webContentRefDev = (AddressableGameObjectLoaderSo)await Resources.LoadAsync<AddressableGameObjectLoaderSo>("Settings/WebViewContentRef");

            // 判断网址有效性
            if (!WebsiteFinder.TryFindWebsitePageInfo(url, out var webPageInfo))
            {
                await LoggerManager.Instance.LogWarnAsync($"无效网址：{url}");
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

            tab.EventCenter.OnTagPointerEnter += (_, _) =>
            {
                tab.transform.DOScaleX(1.05F, 0.15F)
                    .OnComplete(() => tab.transform.DOScaleX(1F, 0.15F));
            };

            var webContentPair = new WebContentPair(tab, webViewContent);
            WebPageMap.Add(webPageInfo.Url, webContentPair);

            var first = true;
            foreach (var methodInfo in Reflection.GetComponentsInitInChildren(webViewContent.transform, webViewContent.GetCancellationTokenOnDestroy()))
            {
                if (!methodInfo.initAttribute.ShouldInitialize)
                {
                    _websiteLoadingComplete = true;

                    if (first)
                        FocusViewWindow(webPageInfo.Url);
                }

                if (methodInfo.initAttribute.FinalInitialization)
                {
                    await methodInfo.initTask.Invoke();

                    _websiteLoadingComplete = true;
                    if (first)
                        FocusViewWindow(webPageInfo.Url);
                }
                else
                {
                    if (first)
                        FocusViewWindow(webPageInfo.Url);
                    await methodInfo.initTask.Invoke();

                    _websiteLoadingComplete = true;
                }

                first = false;
            }
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


        public static class Reflection
        {
            public static IEnumerable<(WebContentInitAttribute initAttribute, Func<UniTask> initTask)> GetComponentsInitInChildren(Transform parent,
                CancellationToken cancellationToken)
            {
                var list = new List<(WebContentInitAttribute attribute, Func<UniTask> task, Type type)>(4);
                foreach (var uiContainer in parent.GetComponentsInChildren<UIContainer>())
                {
                    var methods = uiContainer.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                        .FirstOrDefault(m => m.ReturnType == typeof(UniTask) &&
                                             m.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken)));

                    if (methods is null)
                    {
                        throw new NotImplementedException(
                            $"类'{uiContainer.GetType()}'缺少一个必要的方法，该方法应具有以下签名：'NotPublic {typeof(UniTask).FullName} UserInitializationName ({typeof(CancellationToken).FullName} cancellationToken)'。此外，建议为该方法添加特性'[{typeof(WebContentInitAttribute).FullName}]'。");
                    }


                    var initAttribute = methods.GetCustomAttribute<WebContentInitAttribute>();

                    if (initAttribute is not null)
                    {
                        Func<UniTask> task = () => (UniTask)methods.Invoke(uiContainer, new object[] { initAttribute.NotUsedCancellationToken ? default : cancellationToken });
                        list.Add((initAttribute, task, uiContainer.GetType()));
                    }
                }

                return Sort(list);
            }


            private static LinkedList<(WebContentInitAttribute, Func<UniTask>)> Sort(List<(WebContentInitAttribute attribute, Func<UniTask> task, Type type)> list)
            {
                // 创建一个字典来存储类型到应该插入的索引的映射，以及LinkedList节点  
                var typeToNodeInfo = new Dictionary<Type, (LinkedListNode<(WebContentInitAttribute, Func<UniTask>)> node, bool isInserted)>();
                var sortedList = new LinkedList<(WebContentInitAttribute, Func<UniTask>)>();

                foreach (var element in list)
                {
                    var node = new LinkedListNode<(WebContentInitAttribute, Func<UniTask>)>((element.attribute, element.task));
                    typeToNodeInfo[element.type] = (node, false); // 初始时标记为未插入  

                    if (element.attribute.ThenAfterInitialization == null)
                    {
                        sortedList.AddLast(node); // 没有依赖，直接添加到末尾  
                        typeToNodeInfo[element.type] = (node, true); // 标记为已插入  
                    }
                    else
                    {
                        // 尝试找到依赖项的位置并插入之后  
                        if (typeToNodeInfo.TryGetValue(element.attribute.ThenAfterInitialization, out var depNodeInfo) && depNodeInfo.isInserted)
                        {
                            sortedList.AddAfter(depNodeInfo.node, node); // 插入到依赖项之后  
                            typeToNodeInfo[element.type] = (node, true); // 标记为已插入  
                        }
                        // 否则，将保持在LinkedList外部，稍后可能添加到末尾（存在循环依赖或依赖项不存在时）  
                    }
                }

                // 添加所有未插入的节点到LinkedList末尾（处理循环依赖或缺失依赖）  
                foreach (var kvp in typeToNodeInfo.Where(kvp => !kvp.Value.isInserted))
                {
                    sortedList.AddLast(kvp.Value.node);
                }

                return sortedList;
            }
        }
    }
}