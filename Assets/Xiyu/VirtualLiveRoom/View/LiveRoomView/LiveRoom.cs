using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.Expand;
using Xiyu.VirtualLiveRoom.AudioSystem;
using Xiyu.VirtualLiveRoom.EventFunctionSystem;

namespace Xiyu.VirtualLiveRoom.View.LiveRoomView
{
    public class LiveRoom : Component.WebViewContent
    {
        [SerializeField] private AishiBehaviour aishiBehaviour;


        [WebContentInit(FinalInitialization = false)]
        protected override async UniTask Initialization(CancellationToken cancellationToken = default)
        {
            // 伪加载
            await UniTask.WhenAll(PseudoLoading(), aishiBehaviour.InitCharacter());

            var audioOperator = await AudioManager.Instance.GetAudioOperatorPlayer("bgm").SetClip("bgm09");

            await audioOperator.SetLoop(true).SetVolume(0.3F).PlayFadein(3F);
        }

        private async UniTask PseudoLoading()
        {
            var taskQueue = new Queue<UniTask>();

            foreach (var image in baseCanvasGroup.transform.GetComponentsInChildren<Image>(false).Where(_ => RandomBooleResult(0.3F)))
            {
                var endValue = image.color.a;
                image.color = image.color.SetAlpha(0);

                taskQueue.Enqueue(
                    image.DOFade(endValue, Random.Range(0.25F, 1F))
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