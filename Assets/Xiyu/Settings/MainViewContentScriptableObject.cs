#if OldCode
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Xiyu.VirtualLiveRoom.Component.Navigation;

namespace Xiyu.Settings
{
    [CreateAssetMenu(fileName = "New MainViewContent", menuName = "ScriptableObject/MainViewContent")]
    public class MainViewContentScriptableObject : ScriptableObject
    {
        [SerializeField] private List<ViewContentLoader> viewContents;

        [CanBeNull]
        public ViewContentLoader Find(string preformName)
            => viewContents.Find(item => item.PreformName == preformName);
    }
}
#endif