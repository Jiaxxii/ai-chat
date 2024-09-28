using UnityEngine;

namespace Xiyu
{
    public class ScriptTemple : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        public float Alpha
        {
            get => canvasGroup.alpha;
            set => canvasGroup.alpha = value;
        }
    }
}