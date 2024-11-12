using System.Threading;
using Cysharp.Threading.Tasks;
using Xiyu.VirtualLiveRoom.EventFunctionSystem;

namespace Xiyu.VirtualLiveRoom.View
{
    public class UserLogon : Component.WebViewContent
    {
        [WebContentInit(false)]
        protected override UniTask Initialization(CancellationToken cancellationToken = default) => UniTask.CompletedTask;
        
    }
}