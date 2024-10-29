using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Xiyu.CharacterIllustration
{
    public class SpriteResourceLoader
    {
        public SpriteResourceLoader(IResourceLocation resourceLocation)
        {
            Resource = resourceLocation;
            IsRead = false;
        }

        public IResourceLocation Resource { get; }
        public bool IsRead { get; private set; }

        private Sprite _target;

        // AsyncOperationHandle<Sprite>
        public IEnumerator GetAsync(Action<Sprite> onCompletion)
        {
            if (IsRead)
            {
                onCompletion.Invoke(_target);
                yield break;
            }

            var spriteHandle = Addressables.LoadAssetAsync<Sprite>(Resource);
            yield return spriteHandle;

            Debug.Assert(spriteHandle.Status == AsyncOperationStatus.Succeeded, $"加载精灵\"{Resource.PrimaryKey}\"时发生异常!");

            _target = spriteHandle.Result;
            IsRead = true;

            onCompletion.Invoke(spriteHandle.Result);
            // Addressables.Release(spriteHandle);
        }

        public async UniTask<Sprite> GetAsync()
        {
            if (IsRead)
            {
                return _target;
            }

            var spriteHandle = Addressables.LoadAssetAsync<Sprite>(Resource);
            await spriteHandle;

            Debug.Assert(spriteHandle.Status == AsyncOperationStatus.Succeeded, $"加载精灵\"{Resource.PrimaryKey}\"时发生异常!");

            _target = spriteHandle.Result;
            IsRead = true;

            return _target;
            // Addressables.Release(spriteHandle);
        }
    }
}