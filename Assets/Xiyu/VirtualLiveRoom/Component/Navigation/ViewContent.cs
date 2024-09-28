using UnityEngine;

namespace Xiyu.VirtualLiveRoom.Component.Navigation
{
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