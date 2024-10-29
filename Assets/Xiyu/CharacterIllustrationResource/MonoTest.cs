using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Xiyu.Expand;

namespace Xiyu.CharacterIllustrationResource
{
    public class MonoTest : MonoBehaviour
    {
        [SerializeField] private Image image;

        private AsyncOperationHandle<Sprite> _asyncOperationHandle;

        private async void Start()
        {
            await BodyInfoSettings.LoadSettingsAsync();

            var bodyInfo = BodyInfoSettings.Main["ai_a_0032"];

            var assetLoader = await AssetLoaderCenter<Sprite>.LoadResourceLocations("ai");

            var first = bodyInfo.Data.First();
            using (new TimeConsuming())
            {
                image.sprite = await assetLoader.LoadAssetAsync(first);
            }

            // assetLoader.Release(first.Path);
            

            using (new TimeConsuming())
            {
                image.sprite = await assetLoader.LoadAssetAsync(first);
            }

            

            // while (true)
            // {
            //     using var time = new TimeConsuming();
            //
            //     var sprites = await assetLoader.LoadAssetsAsync(bodyInfo.Data.Where(d => d.Path != first.Path));
            //
            //     time.Pause();
            //     Debug.Log(string.Join("  ", sprites.Select(s => s.name)));
            //
            //     await UniTask.WaitForSeconds(1);
            //     time.Continue();
            // }
        }
    }
}