using System.Collections;

namespace Xiyu.AIChat.LargeLanguageModel.Service
{
    public interface IServiceResponseResult<out T>
    {
        string Result { get; set; }

        IEnumerator SendResultCoroutine(System.Action<T> onComplete);
    }
}