using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom.Component
{
    public class WebViewContent : UIContainer
    {
        [SerializeField] protected CanvasGroup baseCanvasGroup;


        public float Alpha
        {
            get => baseCanvasGroup.alpha;
            set => baseCanvasGroup.alpha = value;
        }

        public void SetCanvasGroupActive(bool active)
        {
            baseCanvasGroup.alpha = active ? 1 : 0;
            baseCanvasGroup.interactable = active;
            baseCanvasGroup.blocksRaycasts = active;
        }


        public PageInfo PageInfo { get; private set; }


        [JetBrains.Annotations.UsedImplicitly]
        public virtual UniTaskVoid WebPageFocus()
        {
            return default;
        }


        [JetBrains.Annotations.UsedImplicitly]
        public virtual UniTaskVoid WebPageLostFocus()
        {
            return default;
        }
    }
}