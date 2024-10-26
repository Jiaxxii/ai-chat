#if OldCode
using System;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom.Component.Navigation
{
    [Obsolete("组件已经弃用，请使用\"Xiyu.VirtualLiveRoom.Component.NewNavigation.WebViewContent\"", false)]
    public class ViewContent : UIContainer
    {
        [SerializeField] private CanvasGroup canvasGroup;


        public TagPage TagPage { get; set; }


        public float Alpha
        {
            get => canvasGroup.alpha;
            set => canvasGroup.alpha = value;
        }


        public void DestroyCurrentWebView()
        {
            Destroy(gameObject);
        }
    }
}
#endif