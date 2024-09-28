using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Xiyu
{
    public class AsyncLoading : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<GameObject> assetReference;


        private IEnumerator Start()
        {
            var handle = assetReference.LoadAssetAsync();
            yield return handle;


            // if (handle.Status == AsyncOperationStatus.Succeeded)
            // {
            //     Instantiate(handle.Result).GetComponent<ScriptTemple>().Alpha = 0.1f;
            // }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var st = handle.Result.GetComponent<ScriptTemple>();
                st.Alpha = 0.1F;

                Instantiate(st);
            }
        }
    }
}