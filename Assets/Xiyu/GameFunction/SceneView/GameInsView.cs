using System;
using UnityEngine;
using Xiyu.Constant;

namespace Xiyu.GameFunction.SceneView
{
    [Obsolete("",true)]
    public static class GameInsView
    {
        private static Canvas _mainCanvas;

        public static Canvas MainCanvas => _mainCanvas != null ? _mainCanvas : GameObject.Find(GameConstant.MainCanvasName).GetComponent<Canvas>();

        private static RectTransform _rectTransform;

        public static RectTransform RectTransform => _rectTransform != null ? _rectTransform : MainCanvas.transform as RectTransform ?? throw new InvalidCastException();

        public static Vector2 ScreenSize => RectTransform.sizeDelta;
    }
    
}