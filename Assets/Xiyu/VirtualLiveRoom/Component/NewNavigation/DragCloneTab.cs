using UnityEngine;
using UnityEngine.Serialization;

namespace Xiyu.VirtualLiveRoom.Component.NewNavigation
{
    public class DragCloneTab : UIContainer
    {
        [SerializeField] private Tab cloneTab;

        [SerializeField] private RectTransform basePanel;

        [SerializeField] private CanvasGroup canvasGroup;


        public Vector2 AnchoredPosition
        {
            get => basePanel.anchoredPosition;
            set => basePanel.anchoredPosition = value;
        }

        public Vector2 SizeDelta
        {
            get => basePanel.sizeDelta;
            set => basePanel.sizeDelta = value;
        }
        
        private Tab _current;

        private void Awake()
        {
            basePanel = GetComponent<RectTransform>();
            SetActive(false);
        }

        public void SetDrag(Tab tagPage)
        {
            if (_current != null)
            {
                ReleaseDrag();
            }

            _current = tagPage;


            cloneTab.SizeDelta = tagPage.SizeDelta;
            cloneTab.AnchoredPosition = tagPage.AnchoredPosition;

            cloneTab.Icon = tagPage.Icon;
            cloneTab.Content = tagPage.Content;


            basePanel.SetAsLastSibling();

            tagPage.SetContentActiveAndRay(false);
            SetActive(true);
        }

        public void ReleaseDrag()
        {
            _current.SetContentActiveAndRay(true);
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