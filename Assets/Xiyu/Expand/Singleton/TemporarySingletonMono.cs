using System;
using UnityEngine;
using Xiyu.LoggerSystem;

namespace Xiyu.Expand.Singleton
{
    /// <summary>
    /// 泛型单例
    /// </summary>
    /// <typeparam name="TSingleton"></typeparam>
    public class TemporarySingletonMono<TSingleton> : MonoBehaviour where TSingleton : TemporarySingletonMono<TSingleton>
    {
        private static readonly Lazy<TSingleton> LazyInstance = new(FactorTemporarySingletonMono);

        public static TSingleton Instance => LazyInstance.Value;

        protected virtual void Init()
        {
            DontDestroyOnLoad(gameObject);
        }

        private static TSingleton FactorTemporarySingletonMono()
        {
            var logger = LoggerManager.Instance;

            var types = FindObjectsOfType<TSingleton>();
            if (types.Length > 0)
            {
                logger.LogWarning($"在尝试创建单例时，场景中已经存在{types.Length}个{typeof(TSingleton).Name}实例。");
                // 这里可以选择返回已存在的实例，或者抛出异常，取决于你的需求。  
                // 例如，返回第一个已存在的实例：  
                return types[0];
            }

            var singletonObj = new GameObject($"Singleton-{typeof(TSingleton).Name}")
                .AddComponent<TSingleton>();

            singletonObj.Init();

            return singletonObj;
        }
    }
}