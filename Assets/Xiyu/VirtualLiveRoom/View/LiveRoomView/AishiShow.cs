using System;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CharacterController = Xiyu.VirtualLiveRoom.Component.Character.CharacterController;

namespace Xiyu.VirtualLiveRoom.View.LiveRoomView
{
    public class AishiShow : MonoBehaviour
    {
        private CharacterController _characterController;
        [SerializeField] private Button next, last;

        [SerializeField] [Range(10, 56)] private int index;

        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        private readonly ConcurrentQueue<int> _queue = new();
        private bool _isPlaying;

        private void Awake()
        {
            // next.onClick.AddListener(() => UniTask.UnityAction(index += index % 2 == 0 ? 2 : 1, Load));
            // last.onClick.AddListener(() => UniTask.UnityAction(index -= index % 2 == 0 ? 2 : 1, Load));
            next.onClick.AddListener(() => Load(index += index % 2 == 0 ? 2 : 1).Forget());
            last.onClick.AddListener(() => Load(index -= index % 2 == 0 ? 2 : 1).Forget());
        }


        private async UniTaskVoid Load(int targetIndex)
        {
            _queue.Enqueue(index = Mathf.Clamp(targetIndex, 10, 56));
            if (_isPlaying && _queue.TryPeek(out var peek))
            {
                textMeshProUGUI.text = $"序号：<color=red>{index:0000}</color> -> <color=red>{peek:0000}</color> 加载中...";
                return;
            }

            _isPlaying = true;

            while (_queue.TryDequeue(out var num))
            {
                textMeshProUGUI.text = $"序号：<color=red>{num:0000}</color>";
                await _characterController.DisplayFaceFade($"ai_a_{num:0000}");
                await UniTask.WaitForSeconds(1F, delayTiming: PlayerLoopTiming.Update);
            }

            _isPlaying = false;
        }

        private async void Start()
        {
            var rect = (RectTransform)transform;
            _characterController = await CharacterController.CreateRole<CharacterController>(rect.sizeDelta, rect, "ai", "ai_a_0000", "ai_a_0010", true);

            next.transform.SetAsLastSibling();
            last.transform.SetAsLastSibling();
        }
    }
}