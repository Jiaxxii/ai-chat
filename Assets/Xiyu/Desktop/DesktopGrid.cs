using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Xiyu.Desktop.FiniteStateMachine;

namespace Xiyu.Desktop
{
    public class DesktopGrid : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite Sprite;
        [SerializeField] private RectTransform content;
        [SerializeField] private Color selectIConColor;
        [SerializeField] private Color highlightIConColor;

        [SerializeField] private DesktopTaskbar taskbar;

        private StateMachine _machine = new();

        private void Awake()
        {
            var dsp = new DesktopIconOrganizer(10, 10, 100);
            taskbar.Init(dsp.TaskbarHeight);

            for (var i = 0; i < 10; i++)
            {
                dsp.CreateAndAddDesktopIcon(content, Sprite, $"图标 {i}");
            }

            var desktopIcon = DesktopIcon.Create(content, Sprite, "名称1");

            desktopIcon.DesktopMatrix = new Vector2Int(5, 8);

            dsp.AddDesktopIcon(desktopIcon);
            
            dsp.CreateAndAddDesktopIcon(content, Sprite, $"你好");
            
        }


        public void OnPointerClick(PointerEventData eventData)
        {
        }
    }
}