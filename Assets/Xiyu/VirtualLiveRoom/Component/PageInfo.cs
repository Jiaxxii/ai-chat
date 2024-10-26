using System;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom.Component
{
    [Serializable]
    public struct PageInfo : IEquatable<PageInfo>
    {
        public PageInfo(string title, string url, Sprite icon, WebsiteSecurityLevel wsl)
        {
            this.title = title;
            this.url = url;
            this.icon = icon;
            securityLevel = wsl;
        }

        [Header("网页")] [Space] [TextArea(1, 3)] [SerializeField]
        private string url;

        public string Url => url;


        [TextArea(1, 2)] [SerializeField] private string title;
        public string Title => title;


        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;

        [SerializeField] private WebsiteSecurityLevel securityLevel;
        public WebsiteSecurityLevel SecurityLevel => securityLevel;


        public static PageInfo NotImplemented()
        {
            UnityEngine.Debug.LogWarning("PageInfo未实现!");
            return new PageInfo("未命名标题", "https://www.notimplemented.com", null, WebsiteSecurityLevel.Null);
        }

        #region 相等成员比较实现 （Rider生成）

        public bool Equals(PageInfo other)
        {
            return Url == other.Url;
        }

        public override bool Equals(object obj)
        {
            return obj is PageInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Url != null ? Url.GetHashCode() : 0);
        }

        public static bool operator ==(PageInfo left, PageInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PageInfo left, PageInfo right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}