using System;
using UnityEngine;

namespace Xiyu.VirtualLiveRoom
{
    public class VirtualIconControl : MonoBehaviour
    {
        [SerializeField] private Icon icon;

        public Icon Icon => icon;

        private Icon _currentIcon;
        private Color _startColor;


        private void Awake()
        {
            icon.Color = Color.clear;
        }

        public void SetClone(Icon item)
        {
            if (_currentIcon != null)
            {
                Release();
            }

            _startColor = icon.Color = (_currentIcon = item).Color;
            item.Color = Color.black;
            icon.transform.SetAsLastSibling();
        }


        public void Release()
        {
            if (_currentIcon == null)
            {
                return;
            }

            _currentIcon.Color = _startColor;
            icon.Color = Color.clear;
        }
    }
}