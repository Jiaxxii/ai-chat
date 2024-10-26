using System.Threading;
using Cysharp.Threading.Tasks;

namespace Xiyu.VirtualLiveRoom.View
{
    public class LiveRoom : Component.WebViewContent
    {
        protected override UniTask Initialization(CancellationToken cancellationToken = default)
        {
            return UniTask.WaitForSeconds(3, cancellationToken: cancellationToken);
        }
    }
}