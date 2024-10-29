using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.Expand;
using Xiyu.VirtualLiveRoom.EventFunctionSystem;
using CharacterController = Xiyu.VirtualLiveRoom.Component.Character.CharacterController;

namespace Xiyu.VirtualLiveRoom.View
{
    public class LiveRoom : Component.WebViewContent
    {
        [SerializeField] private RectTransform window;

        [WebContentInit(FinalInitialization = false)]
        protected override async UniTask Initialization(CancellationToken cancellationToken = default)
        {
            // 伪加载
            await PseudoLoading();

            window ??= (RectTransform)transform;

            var roleControl = await CharacterController.CreateRole<CharacterController>(window.sizeDelta, window, "ai");

            await roleControl.Display("ai_a_0000", "ai_a_0010");

            roleControl.Geometry.Scale = new Vector3(0.5F, 0.5F, 0.5F);
        }

        private async UniTask PseudoLoading()
        {
            var taskQueue = new Queue<UniTask>();

            foreach (var image in baseCanvasGroup.transform.GetComponentsInChildren<Image>(false).Where(_ => RandomBooleResult(0.3F)))
            {
                var endValue = image.color.a;
                image.color = image.color.SetAlpha(0);

                taskQueue.Enqueue(
                    image.DOFade(endValue, Random.Range(1F, 3F))
                        .SetEase(Ease.Linear)
                        .AsyncWaitForCompletion()
                        .AsUniTask());
            }

            while (taskQueue.Count != 0)
            {
                await taskQueue.Dequeue();
            }
        }

        private static bool RandomBooleResult(float accountFor = 0.5F) => Random.value <= accountFor;
    }
}