using System;
using System.Collections;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Xiyu.CharacterIllustration;
using Xiyu.GameFunction.GeometricTransformations;
using Xiyu.Settings;

namespace Xiyu.GameFunction.CharacterComponent
{
    public class CharacterContentRoot : MonoBehaviour, ICharacterControl
    {
        /// <summary>
        /// 一些预制体资源
        /// </summary>
        private static PreformScriptableObject _preformScriptableObject;


        // 用于存放上一次的使用的 typeCode
        // 如果使用时有相等则不再重复加载
        private (string bodyCode, string faceCode) _lastTypeCode = (string.Empty, string.Empty);


        protected SpriteAssetLoader SpriteAssetLoader;


        /// <summary>
        /// 强烈建议 <see cref="Type"/> 名称与要加载的资源标签名称相同
        /// </summary>
        public string Type { get; private set; }


        /// <summary>
        /// Body 节点控制器
        /// </summary>
        public RoleBodyType Body { get; private set; }

        /// <summary>
        /// Faces 节点控制器
        /// </summary>
        public RoleBodyType Faces { get; private set; }

        public IGeomTransforms Geometry { get; private set; }

        /// <summary>
        /// 一些预制体资源
        /// </summary>
        public static PreformScriptableObject PreformScriptableObject
        {
            get
            {
                if (_preformScriptableObject != null)
                {
                    return _preformScriptableObject;
                }

                return _preformScriptableObject = Resources.Load<PreformScriptableObject>("Settings/Main PreformData");
            }
        }

        private GeomTransforms PropertyLink()
        {
            var rt = transform as RectTransform;

            if (rt == null)
            {
                throw new InvalidCastException();
            }

            var positionProperty = new Property<Vector2>(() => rt.anchoredPosition, value => rt.anchoredPosition = value);
            var sizeProperty = new Property<Vector2>(() => Body.RoleUnits[0].SpriteContent.rectTransform.sizeDelta, null);
            var scaleProperty = new Property<Vector3>(() => rt.localScale, value => rt.localScale = value);
            var rotateProperty = new Property<Vector3>(() => rt.eulerAngles, value => rt.eulerAngles = value);

            var colorProperty = new Property<Color>(() => Body.RoleUnits[0].SpriteContent.color, value =>
            {
                Body.RoleUnits[0].SpriteContent.color = value;
                foreach (var roleUnit in Faces.RoleUnits)
                {
                    roleUnit.SpriteContent.color = value;
                }
            });

            return new GeomTransforms(positionProperty, sizeProperty, scaleProperty, rotateProperty, colorProperty);
        }


        public virtual IEnumerator Init(string roleTye, JObject transformInfoDataJson, bool isLoadRefAssets)
        {
            Type = roleTye;

            var contentTf = transform.Find("Content");
            Body = contentTf.Find("type_body").GetComponent<RoleBodyType>();
            Faces = contentTf.Find("type_face").GetComponent<RoleBodyType>();

            Body.Init(nameof(Body), 1);
            Faces.Init(nameof(Faces), 4);

            yield return SpriteResourceManager.CreateAssetAsync(roleTye, transformInfoDataJson);

            SpriteAssetLoader = SpriteResourceManager.Get(roleTye);

            if (isLoadRefAssets)
            {
                yield return SpriteAssetLoader.LoadRefAssetsAsync(roleTye);
            }

            Geometry = PropertyLink();
        }


        public T GetThis<T>() where T : Component, ICharacterControl
        {
            return this as T;
        }


        /// <summary>
        /// 显示立绘
        /// </summary>
        /// <param name="bodyCode">身体</param>
        /// <param name="faceCode">脸部</param>
        /// <returns>此方法需要配合协程使用</returns>
        public virtual IEnumerator Display(string bodyCode, string faceCode)
        {
            // 如何 typeCode 与上一次一样就没必要设置了，同时也可以提供性能
            var result = IsEqualTypeCode(bodyCode, faceCode);
            if (!result.equalBody)
            {
                // 设置身体
                var bodyData = SpriteAssetLoader.GetSpriteAsset(bodyCode).Data.TransformInfoData;
                yield return SpriteAssetLoader.LoadAssetAsync(bodyCode, bodySprites => Body.Display(bodySprites, bodyData));
            }

            if (result.equalFace)
            {
                yield break;
            }

            // 设置脸部
            var faceData = SpriteAssetLoader.GetSpriteAsset(faceCode).Data.TransformInfoData;
            yield return SpriteAssetLoader.LoadAssetAsync(faceCode, facesSprites => Faces.Display(facesSprites, faceData));
        }

        public IEnumerator DisplayFaceFade(string faceCode, float outDuration, float inDuration)
        {
            var property = new Property<Color>(() => Faces.RoleUnits[0].SpriteContent.color, value =>
            {
                foreach (var roleUnit in Faces.RoleUnits)
                {
                    roleUnit.SpriteContent.color = value;
                }
            });

            yield return DisplayFade(property, () => Faces, faceCode, outDuration, inDuration);
        }

        public IEnumerator DisplayBodyFade(string bodyCode, float outDuration, float inDuration)
        {
            var property = new Property<Color>(() => Faces.RoleUnits[0].SpriteContent.color, value =>
            {
                Body.RoleUnits[0].SpriteContent.color = value;
                foreach (var roleUnit in Faces.RoleUnits)
                {
                    roleUnit.SpriteContent.color = value;
                }
            });

            yield return DisplayFade(property, () => Body, bodyCode, outDuration, inDuration);
        }


        private IEnumerator DisplayFade(Property<Color> property, Func<RoleBodyType> bodyType, string typeCode, float outDuration, float inDuration)
        {
            // 淡出动画序列  
            var fadeOutSequence = DOTween.Sequence();
            fadeOutSequence.Append(DOTween.ToAlpha(() => property.Member, color => property.Member = color, 0, outDuration)
                .SetEase(Ease.OutExpo));
            yield return fadeOutSequence.WaitForCompletion();

            var result = IsEqualTypeCode(null, typeCode);

            if (!result.equalFace)
            {
                // 异步加载资源  
                var faceData = SpriteAssetLoader.GetSpriteAsset(typeCode).Data.TransformInfoData;
                yield return SpriteAssetLoader.LoadAssetAsync(typeCode, facesSprites => bodyType.Invoke().Display(facesSprites, faceData));
            }

            // 创建新的序列以执行淡入动画  
            var fadeInSequence = DOTween.Sequence();
            fadeInSequence.Append(DOTween.ToAlpha(() => property.Member, color => property.Member = color, 1, inDuration)
                .SetEase(Ease.InExpo));
            yield return fadeInSequence.WaitForCompletion();
        }

        /// <summary>
        /// 创建一个角色立绘控制器
        /// </summary>
        /// <param name="parent">立绘父节点 (*需要是 RectTransform)</param>
        /// <param name="roleTye">立绘控制器类型(名称) (*强烈建议您将这个控制器需要加载的标签名称作为参数名称)</param>
        /// <param name="transformInfoDataJson">立绘图集初始化参数信息 JSON</param>
        /// <param name="complete">在 <see cref="CharacterContentRoot"/> 完成初始化时</param>
        /// <param name="isLoadRefAssets">在初始化时建立引用</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerator CreateRole<T>(Transform parent, string roleTye, JObject transformInfoDataJson, Action<ICharacterControl> complete, bool isLoadRefAssets = false)
            where T : Component, ICharacterControl
        {
            var template = PreformScriptableObject.Table["Role Content Root Template"].Preform;
            template.name = $"RoleContentRoot#{roleTye}#";

            var gameObject = Instantiate(template, parent: parent);

            if (gameObject.AddComponent<T>() is not ICharacterControl contentRoot)
            {
                throw new InvalidCastException();
            }

            yield return contentRoot.Init(roleTye, transformInfoDataJson, isLoadRefAssets);
            complete.Invoke(contentRoot);
        }


        protected (bool equalBody, bool equalFace) IsEqualTypeCode(string bodyCode, string faceCode)
        {
            var hasBody = false;
            var hasFace = false;

            if (!string.IsNullOrEmpty(bodyCode) && bodyCode == _lastTypeCode.bodyCode)
            {
                hasBody = true;
            }
            else _lastTypeCode.bodyCode = bodyCode;

            // ReSharper disable once InvertIf
            if (!string.IsNullOrEmpty(faceCode) && faceCode == _lastTypeCode.faceCode)
            {
                hasFace = true;
            }
            else _lastTypeCode.faceCode = faceCode;

            return (hasBody, hasFace);
        }
    }
}