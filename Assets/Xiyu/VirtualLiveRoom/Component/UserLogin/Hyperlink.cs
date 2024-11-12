using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Xiyu.LoggerSystem;
using Xiyu.VirtualLiveRoom.Component.NewNavigation;

namespace Xiyu.VirtualLiveRoom.Component.UserLogin
{
    public class Hyperlink : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI hyperLinkText;

        [SerializeField] private WebHyperLinkTo webHyperLinkTo;

        public IWebHyperLinkTo WebHyperLinkTo
        {
            get => webHyperLinkTo;
            set => webHyperLinkTo = value as WebHyperLinkTo;
        }


        [SerializeField] private Color highlightColor = new(1F, 0.04F, 0.38F);
        [SerializeField] private UnityEvent<PageInfo> onHyperLinkClick;

        /// <summary>
        /// PageInfo 目标网页
        /// </summary>
        public event UnityAction<PageInfo> OnHyperLinkClick
        {
            add => onHyperLinkClick.AddListener(value);
            remove => onHyperLinkClick.RemoveListener(value);
        }

        private Color _rawColor;

        private void Start()
        {
            _rawColor = hyperLinkText.color;
            hyperLinkText.fontStyle = FontStyles.Underline | FontStyles.Italic;
            if (string.IsNullOrEmpty(hyperLinkText.text))
                hyperLinkText.text = webHyperLinkTo.TargetUrl;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hyperLinkText.color = highlightColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hyperLinkText.color = _rawColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            hyperLinkText.color = _rawColor;

            if (webHyperLinkTo == null || string.IsNullOrEmpty(webHyperLinkTo.TargetUrl))
            {
                throw new NullReferenceException($"对象\'{typeof(WebHyperLinkTo)}\' is null:{webHyperLinkTo == null}");
            }

            if (!WebsiteFinder.TryFindWebsitePageInfo(webHyperLinkTo.TargetUrl, out var currentPageInfo))
            {
              LoggerManager.Instance.LogError($"链接\"{webHyperLinkTo.TargetUrl}\"不存在！");
                return;
            }

            onHyperLinkClick?.Invoke(currentPageInfo);
            WebHyperLinkTo.JumpTo();
        }
    }
}