using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Xiyu.VirtualLiveRoom.EventFunctionSystem;

namespace Xiyu.VirtualLiveRoom.Component
{
    public class UIContainer : MonoBehaviour
    {
        [WebContentInit]
        [JetBrains.Annotations.UsedImplicitly]
        protected virtual UniTask Initialization(CancellationToken cancellationToken = default)
        {
            return UniTask.WaitForEndOfFrame(this, cancellationToken);
        }


        public virtual void WebPageFocus()
        {
        }


        public virtual void WebPageLostFocus()
        {
        }
    }
}

#region test code

/*public static Dictionary<string, Action> GetInstanceEvent(UIContainer uiContainer)
{
    var methodInfos = uiContainer.GetType()
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0 && m.IsDefined(typeof(EventHolderAttribute)))
        .ToList();

    if (methodInfos.Count == 0)
    {
        throw new NotImplementedException($"\"{uiContainer.GetType().Name}\"未定义签名为\"NonPublic void MethodsName()\"并且标注特性[\"{nameof(EventHolderAttribute)}\"]的方法！");
    }

    var funcMap = new Dictionary<string, Action>();
    foreach (var methodInfo in methodInfos)
    {
        var eventHolderAttribute = methodInfo.GetCustomAttribute<EventHolderAttribute>();
        funcMap.TryAdd(eventHolderAttribute.MethodName, () => methodInfo.Invoke(uiContainer, null));
    }

    return funcMap;
}*/

#endregion