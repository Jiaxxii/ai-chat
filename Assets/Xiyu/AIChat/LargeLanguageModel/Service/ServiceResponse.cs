using System;
using System.Collections;

namespace Xiyu.AIChat.LargeLanguageModel.Service
{
    public abstract class ServiceResponse<TResult> : IServiceResponseResult<TResult>
    {
        public abstract string Result { get; set; }

        public virtual IEnumerator SendResultCoroutine( Action<TResult> onComplete)
        {
            yield break;
        }
    }
}