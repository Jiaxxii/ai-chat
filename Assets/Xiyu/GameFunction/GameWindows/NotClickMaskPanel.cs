using UnityEngine;
using UnityEngine.UI;
using Xiyu.Expand;

namespace Xiyu.GameFunction.GameWindows
{
    public class NotClickMaskPanel : Singleton<NotClickMaskPanel>
    {
        [SerializeField] private Image basePanel;


        protected override void Awake()
        {
            // Not Implemented Exception;
        }

        public Color Color
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }

        public float Alpha
        {
            get => basePanel.color.a;
            set => basePanel.color = basePanel.color.SetAlpha(value);
        }

        private Transform _lastTransform;

        public void SetActive(bool value) => basePanel.gameObject.SetActive(value);


        public void SetAsChildAndLast(Transform sibling)
        {
            if (_lastTransform != null)
            {
                ClearParent(_lastTransform);
            }

            _lastTransform = sibling;

            sibling.SetParent(basePanel.transform);
            basePanel.transform.SetAsLastSibling();

            basePanel.gameObject.SetActive(true);
        }


        public void ClearParent(Transform sibling)
        {
            sibling.SetParent(transform);
            basePanel.gameObject.SetActive(true);
        }

        // public void TrySetAsLastSibling()
        // {
        //     if (transform.childCount == 1)
        //     {
        //         return;
        //     }
        //
        //     transform.GetSiblingIndex()
        //     transform.SetSiblingIndex(transform.childCount - 2);
        //     SetActive(true);
        // }
    }
}