using UnityEngine;

namespace Xiyu.Desktop
{
    public partial class DesktopIcon
    {
        public static DesktopIcon Create(Transform parent, Sprite icon, string iconName)
        {
            var prefab = global::Xiyu.GameFunction.CharacterComponent.CharacterContentRoot.PreformScriptableObject.Table["Desktop ICON Item"].Preform;
            var instance = Instantiate(prefab, parent: parent).GetComponent<DesktopIcon>();

            return instance.Init(icon, iconName);
        }

        public static DesktopIcon Create(Transform parent, Icon icon) => Create(parent, icon.IconSprite, icon.IconName);
    }
}