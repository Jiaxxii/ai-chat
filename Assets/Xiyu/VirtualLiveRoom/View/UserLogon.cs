using System.Threading;
using Cysharp.Threading.Tasks;

namespace Xiyu.VirtualLiveRoom.View
{
    public class UserLogon : Component.WebViewContent
    {
        protected override UniTask Initialization(CancellationToken cancellationToken = default)
        {
            return UniTask.WaitForSeconds(3, cancellationToken: cancellationToken);
        }
    }
}