using UnityEngine;
using UnityEngine.UI;

namespace Xiyu.Desktop
{
    public class DesktopTaskbar : MonoBehaviour
    {
        [SerializeField] private Image content;

        public void Init(float height)
        {
            content.rectTransform.sizeDelta = new Vector2(content.rectTransform.sizeDelta.x, height);
        }
    }
}