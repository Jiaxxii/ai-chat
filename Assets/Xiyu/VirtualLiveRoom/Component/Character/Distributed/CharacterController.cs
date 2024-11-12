using System;
using UnityEngine;
using Xiyu.Settings;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace
namespace Xiyu.VirtualLiveRoom.Component.Character
{
    public partial class CharacterController
    {
        /// <summary>
        /// 创建一个角色立绘控制器
        /// </summary>
        /// <param name="parent">立绘父节点 (*需要是 RectTransform)</param>
        /// <param name="roleTye">立绘控制器类型(名称) (*强烈建议您将这个控制器需要加载的标签名称作为参数名称)</param>
        /// <param name="windowSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> CreateRole<T>(Vector2 windowSize, [NotNull] Transform parent, [NotNull] string roleTye)
            where T : UnityEngine.Component, ICharacterControl
        {
            var webViewContentReferenceDeviceSo = (AddressableGameObjectLoaderSo)await Resources.LoadAsync<AddressableGameObjectLoaderSo>("Settings/RefPrefabricate");

            var gameObject = await webViewContentReferenceDeviceSo.LoadGameObjectInstanceAssetAsync("Role Content Root Template", parent);
            gameObject.name = $"RoleContentRoot#{roleTye}#";

            // var gameObject = Object.Instantiate(template, parent: parent);

            if (gameObject.AddComponent<T>() is not ICharacterControl contentRoot)
            {
                throw new InvalidCastException();
            }

            await contentRoot.Init(windowSize, roleTye);
            return (T)contentRoot;
        }

        /// <summary>
        /// 创建一个角色立绘控制器
        /// </summary>
        /// <param name="parent">立绘父节点 (*需要是 RectTransform)</param>
        /// <param name="roleTye">立绘控制器类型(名称) (*强烈建议您将这个控制器需要加载的标签名称作为参数名称)</param>
        /// <param name="windowSize"></param>
        /// <param name="bodyCode"></param>
        /// <param name="faceCode"></param>
        /// <param name="active"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> CreateRole<T>(Vector2 windowSize, [NotNull] Transform parent, [NotNull] string roleTye, [NotNull] string bodyCode, [NotNull] string faceCode,
            bool active = false)
            where T : UnityEngine.Component, ICharacterControl
        {
            var webViewContentReferenceDeviceSo = (AddressableGameObjectLoaderSo)await Resources.LoadAsync<AddressableGameObjectLoaderSo>("Settings/RefPrefabricate");

            var gameObject = await webViewContentReferenceDeviceSo.LoadGameObjectInstanceAssetAsync("Role Content Root Template", parent);
            gameObject.name = $"RoleContentRoot#{roleTye}#";

            // var gameObject = Instantiate(template, parent: parent);

            if (gameObject.AddComponent<T>() is not ICharacterControl contentRoot)
            {
                throw new InvalidCastException();
            }

            await contentRoot.Init(windowSize, roleTye, bodyCode, faceCode, active);
            return (T)contentRoot;
        }

        /// <summary>
        /// 创建一个角色立绘控制器
        /// </summary>
        /// <param name="parent">立绘父节点 (*需要是 RectTransform)</param>
        /// <param name="roleTye">立绘控制器类型(名称) (*强烈建议您将这个控制器需要加载的标签名称作为参数名称)</param>
        /// <param name="windowSize"></param>
        /// <param name="bodyCode"></param>
        /// <param name="faceCode"></param>
        /// <param name="alpha"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> CreateRole<T>(Vector2 windowSize, [NotNull] Transform parent, [NotNull] string roleTye, [NotNull] string bodyCode, [NotNull] string faceCode,
            float alpha = 0F)
            where T : UnityEngine.Component, ICharacterControl
        {
            var webViewContentReferenceDeviceSo = (AddressableGameObjectLoaderSo)await Resources.LoadAsync<AddressableGameObjectLoaderSo>("Settings/RefPrefabricate");

            var gameObject = await webViewContentReferenceDeviceSo.LoadGameObjectInstanceAssetAsync("Role Content Root Template", parent);
            gameObject.name = $"RoleContentRoot#{roleTye}#";

            // var gameObject = Instantiate(template, parent: parent);

            if (gameObject.AddComponent<T>() is not ICharacterControl contentRoot)
            {
                throw new InvalidCastException();
            }

            await contentRoot.Init(windowSize, roleTye, bodyCode, faceCode, alpha);
            return (T)contentRoot;
        }


        /// <summary>
        /// 创建一个角色立绘控制器
        /// </summary>
        /// <param name="parent">立绘父节点 (*需要是 RectTransform)</param>
        /// <param name="roleTye">立绘控制器类型(名称) (*强烈建议您将这个控制器需要加载的标签名称作为参数名称)</param>
        /// <param name="windowSize"></param>
        /// <param name="bodyCode"></param>
        /// <param name="faceCode"></param>
        /// <param name="duration"></param>
        /// <param name="startAlpha"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> CreateRole<T>(Vector2 windowSize, [NotNull] Transform parent, [NotNull] string roleTye, [NotNull] string bodyCode, [NotNull] string faceCode,
            float duration, float startAlpha)
            where T : UnityEngine.Component, ICharacterControl
        {
            var webViewContentReferenceDeviceSo = (AddressableGameObjectLoaderSo)await Resources.LoadAsync<AddressableGameObjectLoaderSo>("Settings/RefPrefabricate");

            var gameObject = await webViewContentReferenceDeviceSo.LoadGameObjectInstanceAssetAsync("Role Content Root Template", parent);
            gameObject.name = $"RoleContentRoot#{roleTye}#";

            // var gameObject = Instantiate(template, parent: parent);

            if (gameObject.AddComponent<T>() is not ICharacterControl contentRoot)
            {
                throw new InvalidCastException();
            }

            await contentRoot.Init(windowSize, roleTye, bodyCode, faceCode, duration, startAlpha);
            return (T)contentRoot;
        }
    }
}