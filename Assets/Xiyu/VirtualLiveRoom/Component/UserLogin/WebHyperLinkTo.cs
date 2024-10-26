using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom.Component.UserLogin
{
    public interface IWebHyperLinkTo
    {
        string TargetUrl { get; set; }
        UniTaskVoid JumpTo();
    }

    public abstract class WebHyperLinkTo : MonoBehaviour, IWebHyperLinkTo
    {
        [SerializeField] private string targetUrl;


        public string TargetUrl
        {
            get => targetUrl;
            set => targetUrl = value;
        }

        public abstract UniTaskVoid JumpTo();
    }
}