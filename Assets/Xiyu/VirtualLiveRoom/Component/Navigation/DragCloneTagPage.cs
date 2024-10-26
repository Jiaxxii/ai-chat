#if OldCode
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Xiyu.VirtualLiveRoom.Component.Navigation
{
    [Obsolete("组件已经弃用，请使用\"Xiyu.VirtualLiveRoom.Component.NewNavigation.TabDragControl\"",false)]
    public class DragCloneTagPage : MonoBehaviour
    {
        [SerializeField] private TagPage cloneTagPage;

        [SerializeField] private RectTransform basePanel;

        [SerializeField] private CanvasGroup canvasGroup;


        public RectTransform RT => basePanel;

        private TagPage _current;

        private void Awake()
        {
            basePanel = GetComponent<RectTransform>();
            SetActive(false);
        }

        public void SetDrag(TagPage tagPage)
        {
            if (_current != null)
            {
                ReleaseDrag();
            }

            _current = tagPage;


            cloneTagPage.RT.sizeDelta = tagPage.RT.sizeDelta;
            cloneTagPage.RT.anchoredPosition = tagPage.RT.anchoredPosition;

            cloneTagPage.Icon = tagPage.Icon;
            cloneTagPage.Title = tagPage.Title;


            basePanel.SetAsLastSibling();

            tagPage.SetContentActive(false);
            SetActive(true);
        }

        public void ReleaseDrag()
        {
            _current.SetContentActive(true);
            SetActive(false);
            _current = null;
        }


        private void SetActive(bool active)
        {
            canvasGroup.alpha = active ? 1 : 0;
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
        }
    }
}
#endif