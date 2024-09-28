using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xiyu.Settings;
using Xiyu.VirtualLiveRoom.Component.Navigation;
using Xiyu.VirtualLiveRoom.Tools.Addressabe;
using Object = UnityEngine.Object;

namespace Xiyu.VirtualLiveRoom.View
{
    public class NavigationController : MonoBehaviour
    {
        [SerializeField] private Transform basePanel;

        [SerializeField] private WebsiteBox websiteBox;

        [SerializeField] private TagPageDragControl tagPageDragControl;

        // [SerializeField] private TagPageEventSender eventSender;

        [SerializeField] private string test;


        private readonly List<ViewContentLoader> _webViewContents = new();

        private IEnumerator Start()
        {
            yield return null;
            yield return LoadAsync(test);
            yield return LoadAsync("View-BILIBILI");

            yield return new WaitForSeconds(1);
            yield return SwitchViewContentAsync(v => v.PreformName == test);

            // yield return LoadAsync("View-A");
        }


        public ViewContent ViewContent { get; private set; }

        public IEnumerator LoadAsync(string preformName, string folder = "Settings", string fileName = "New MainViewContent")
        {
            var handel = Resources.LoadAsync<MainViewContentScriptableObject>($"{folder}/{fileName}");
            yield return handel;

            var viewContent = ((MainViewContentScriptableObject)handel.asset)
                .Find(preformName);

            yield return viewContent!.LoadAsync(basePanel, tagView => ViewContent = tagView);

            yield return AssetReferenceGameObject.Instance.TryLoadAssetAsync();
            var tagPage = TagPage.Create(AssetReferenceGameObject.Instance.AssetInstance, basePanel);

            yield return tagPageDragControl.Create(tagPage, viewContent,
                tagPage =>
                {
                    // tagPage.TagPageEventSender.OnTagPagePointerClick

                    tagPage.TagPageEventSender.OnTagPagePointerClick += _ =>
                        StartCoroutine(SwitchViewContentAsync(v => v.PreformName == viewContent.PreformName || v.URL == viewContent.URL));

                    tagPage.TagPageEventSender.OnTagPageViewClose += () =>
                    {
                        StartCoroutine(OnViewContentCloseEventHandler(v => v.PreformName == viewContent.PreformName || v.URL == viewContent.URL));
                        tagPageDragControl.Remove(tagPage);
                        tagPage.DestroyContent();
                    };
                });


            // Object.Instantiate(viewContent.TagViewItem, basePanel);


            websiteBox.SetWebsite(new Uri(viewContent.URL));

            _webViewContents.Add(viewContent);
        }

        public IEnumerator SwitchViewContentAsync(Predicate<ViewContentLoader> predicate)
        {
            var viewContent = _webViewContents.Find(predicate);


            // basePanel.color = active ? new Color(0.35F, 0.35F, 0.35F) : new Color(0.23F, 0.23F, 0.23F);


            viewContent.ViewContent.transform.SetAsLastSibling();

            yield return null;
        }

        public IEnumerator OnViewContentCloseEventHandler(Predicate<ViewContentLoader> predicate)
        {
            var viewContent = _webViewContents.Find(predicate);

            viewContent.ViewContent.DestroyCurrentWebView();


            // tagPageDragControl.Remove(viewContent.);

            yield return null;
        }


        // private ViewContent FindWebViewContents(Predicate<ViewContent> predicate) => _webViewContents.Find(predicate);
    }
}