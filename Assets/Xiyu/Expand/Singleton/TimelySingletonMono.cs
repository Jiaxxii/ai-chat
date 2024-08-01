using System;
using UnityEngine;
using Xiyu.LoggerSystem;

namespace Xiyu.Expand.Singleton
{
    public class TimelySingletonMono<TSingle> : MonoBehaviour where TSingle : TimelySingletonMono<TSingle>
    {
        // 一开始就在场景中的单例

        private static readonly Lazy<TSingle> LazyInstance = new(FactoryTimelySingletonMono);

        public static TSingle Instance => LazyInstance.Value;


        protected virtual void Awake()
        {
            // 如果单例还没有被访问
            if (LazyInstance.IsValueCreated == false)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private static TSingle FactoryTimelySingletonMono()
        {
            // 注意 * _lazyInstance 无论什么情况下只会实例化一次（第一次访问时）
            // 首先 继承这个泛型单例就表明了脚本已经挂载到了场景中
            var types = FindObjectsOfType<TSingle>();

            var logger = LoggerManager.Instance;

            // 访问实例时没有找到 TSingle 表示场景中没有游戏对象挂载 TSingle
            if (types == null || types.Length == 0)
            {
                logger.ThrowFail($"场景中不存在<{typeof(TSingle).Namespace}>实例!");
                return null;
            }

            // 如果找到多个 TSingle 对象，表示在第一次访问示例时场景中有多个 TSingle 对象实例
            // 可能：Scene1未访问实例A，直到在 Scene2 时才访问实例A，但是Scene2也挂载了相同的脚步
            if (types.Length > 1)
            {
                // 反转我们可以保证“_lazyInstance”只能被实例化一次，那么 TSingle 到现在还没有被访问过，
                // 我们可以直接删除多出来的对象 并且给予一个警告
                for (var i = 1; i < types.Length; i++)
                {
                    var gameObject = types[i].gameObject;
                    logger.LogWarning($"存在多个{typeof(TSingle).Name}对象实例[at {gameObject.name}]");
                    Destroy(gameObject);
                }
            }
            else
            {
                DontDestroyOnLoad(types[0].gameObject);
            }


            return types[0];
        }
    }
}