using Cysharp.Threading.Tasks;
using UnityEngine;
using Xiyu.VirtualLiveRoom.Component.NewNavigation;
using Xiyu.VirtualLiveRoom.Tools.Addressabe;

namespace Xiyu.VirtualLiveRoom.Component.UserLogin
{
    [RequireComponent(typeof(Hyperlink))]
    public class HyperLinkToAiRoom : WebHyperLinkTo
    {
        public override async UniTaskVoid JumpTo()
        {
            NavigationCenter.Instance.CloseView(WebsiteCenter.LinkForm1);
            await NavigationCenter.Instance.AppendOrOpenViewAsync(TargetUrl, default);
        }
    }
}