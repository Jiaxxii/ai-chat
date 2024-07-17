using System;
using UnityEngine;

namespace Xiyu.Expand
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static readonly Lazy<T> Lazy = new(FindObjectOfType<T>);

        public static T Instance => Lazy.Value;


        protected virtual void Awake()
        {
            if (Lazy.IsValueCreated) return;

            DontDestroyOnLoad(gameObject);
            _ = Instance;
        }
    }
}