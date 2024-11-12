using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.NewNavigation
{
    /// <summary>
    /// 网址栏
    /// </summary>
    public class Website : UIContainer, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image basePanel;
        [SerializeField] private TMP_InputField addressInputField;
        [SerializeField] private Image safeLogoImage;
        [SerializeField] private Color highlightColor;

        // [SerializeField] [Range(0.05F, 1)] private float fadeColorSeconds = 0.75F;

        [SerializeField] [Range(0.5F, 3)] private float logoFillSeconds = 1.5F;


        private Color _rawColor;

        /// <summary>
        /// 在网站成功发送时调用（参数分别为 当前，目标）
        /// </summary>
        public event UnityAction<PageInfo, PageInfo> OnUrlSubmit;

        /// <summary>
        /// 当前网站信息
        /// </summary>
        public PageInfo CurrentPageInfo { get; private set; }

        /// <summary>
        /// 当前网站的安全等级
        /// </summary>
        public WebsiteSecurityLevel CurrentSecurity => WebsiteFinder.CheckWebsiteSecurityAuth(CurrentPageInfo.Url);

        /// <summary>
        /// 默认的加载退出条件 （为空时返回true）
        /// </summary>
        public Predicate DefaultLoadingExpression { get; set; }


        private void Awake()
        {
            // 将网址输入框的提交事件绑定网址检测方法
            // addressInputField.onSubmit.AddListener(OnAddressInputFieldSubmitEventHandler);
            addressInputField.onSubmit.AddListener(UniTask.UnityAction<string>(OnAddressInputFieldSubmitEventHandlerAsync));

            _rawColor = basePanel.color;
        }

        public void SetFist(PageInfo pageInfo)
        {
            if (pageInfo.Url == CurrentPageInfo.Url)
            {
                return;
            }

            CurrentPageInfo = pageInfo;
            addressInputField.text = pageInfo.Url;
        }


        public async UniTask SendNewUrlAsync(PageInfo targetPageInfo, Predicate loadingExpression)
        {
            // 输入的网址与原来网址相同
            if (CurrentPageInfo.Url == targetPageInfo.Url)
            {
                return;
            }


            // 检测网址安全性
            var level = await WebsiteFinder.CheckWebsiteSecurityAuthAsync(targetPageInfo.Url);

            if (level == WebsiteSecurityLevel.Null)
                return;


            addressInputField.text = targetPageInfo.Url;

            // if (CurrentSecurity != level)
            // 切换到对应颜色
            safeLogoImage.color = level switch
            {
                WebsiteSecurityLevel.Undefined => Color.gray,
                WebsiteSecurityLevel.Safe => Color.green,
                WebsiteSecurityLevel.Warn => Color.yellow,
                WebsiteSecurityLevel.Dangerous => Color.red,
                _ => Color.gray
            };


            await LoadingAsync(loadingExpression ?? DefaultLoadingExpression ?? (() => true));

            CurrentPageInfo = targetPageInfo;
            OnUrlSubmit?.Invoke(CurrentPageInfo, targetPageInfo);
        }

        public async UniTaskVoid SendNewUrlForGet(PageInfo targetPageInfo, Predicate loadingExpression)
        {
            await SendNewUrlAsync(targetPageInfo, loadingExpression);
        }


        private async UniTaskVoid OnAddressInputFieldSubmitEventHandlerAsync(string newUrl)
        {
            // 查找是否有网址信息
            if (!WebsiteFinder.TryFindWebsitePageInfo(newUrl, out var websiteInfo))
            {
                // 网址是无效的
                addressInputField.text = CurrentPageInfo.Url;
                return;
            }

            // 有网址信息就启动网址发生协程
            await SendNewUrlAsync(websiteInfo, DefaultLoadingExpression);
        }


        private async UniTask LoadingAsync(Predicate loadingExpression)
        {
            while (true)
            {
                safeLogoImage.fillAmount = 0F;
                await DOTween.To(() => safeLogoImage.fillAmount, v => safeLogoImage.fillAmount = v, 1, logoFillSeconds)
                    .SetEase(Ease.OutQuart)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

                if (!loadingExpression.Invoke()) continue;

                safeLogoImage.fillAmount = 1F;
                return;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            basePanel.DOColor(highlightColor, 0.3F);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            basePanel.DOColor(_rawColor, 0.3F);
        }
    }
}