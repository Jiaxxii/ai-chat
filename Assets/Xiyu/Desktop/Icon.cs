using UnityEngine;

namespace Xiyu.Desktop
{
    [System.Serializable]
    public class Icon
    {
        [SerializeField] private Sprite iconSprite;
        [SerializeField] private string iconName;

        public Icon(Sprite iconSprite, string iconName)
        {
            this.iconSprite = iconSprite;
            this.iconName = iconName;
        }

        public Sprite IconSprite => iconSprite;

        public string IconName => iconName;
    }
}