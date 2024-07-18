using System;
using UnityEngine;
using Xiyu.Expand;

namespace Xiyu.GameFunction.GameWindows
{
    public class DialogWindowManager : Singleton<DialogWindowManager>
    {
        public Transform Parent { get; private set; }

        [SerializeField] private Canvas baseCanvas;


        public Canvas BaseCanvas => baseCanvas;

        protected override void Awake()
        {
            base.Awake();
            Parent = FindObjectOfType<DialogWindowManager>().transform;
        }

        // [Obsolete("建议使用具体的对话窗口类型来获取实例", false)]
        // public static DialogWindowBase GetWindow<TResult>(string typeName, Transform parent = null)
        // {
        //     return DialogWindowBase.GetWindow<TResult>(typeName, parent);
        // }
    }
}