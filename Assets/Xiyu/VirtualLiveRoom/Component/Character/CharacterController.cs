using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Xiyu.CharacterIllustrationResource;
using Xiyu.GameFunction.GeometricTransformations;
using Xiyu.Settings;

namespace Xiyu.VirtualLiveRoom.Component.Character
{
    public class CharacterController : MonoBehaviour, ICharacterControl
    {
        public string Type { get; private set; }
        public RoleBodyType Body { get; private set; }
        public RoleBodyType Faces { get; private set; }
        public IGeomTransforms Geometry { get; private set; }

        // 用于存放上一次的使用的 typeCode
        // 如果使用时有相等则不再重复加载
        private (string bodyCode, string faceCode) _lastTypeCode = (string.Empty, string.Empty);

        // protected SpriteAssetLoader SpriteAssetLoader;

        protected AssetLoader<Sprite> AssetLoader;

        private Vector2 _windowSize;


        public async UniTask Display(string bodyCode, string faceCode)
        {
            // 如何 typeCode 与上一次一样就没必要设置了，同时也可以提供性能
            var result = IsEqualTypeCode(bodyCode, faceCode);
            if (!result.equalBody)
            {
                // 设置身体
                BodyInfo bodyInfo = BodyInfoSettings.Main[bodyCode];

                var sprites = await AssetLoader.LoadAssetsAsync(bodyInfo.Data);

                await Body.Display(sprites.ToArray(), bodyInfo.Data);
            }

            if (result.equalFace)
            {
                return;
            }

            // 设置脸部
            BodyInfo faceBodyInfo = BodyInfoSettings.Main[faceCode];
            var faceSprites = await AssetLoader.LoadAssetsAsync(faceBodyInfo.Data);
            await Faces.Display(faceSprites.ToArray(), faceBodyInfo.Data);
        }

        public async UniTask DisplayFaceFade(string faceCode, float outDuration = 0.2F, float inDuration = 0.2F)
        {
            var property = new Property<Color>(() => Faces.RoleUnits[0].SpriteContent.color, value =>
            {
                foreach (var roleUnit in Faces.RoleUnits)
                {
                    roleUnit.SpriteContent.color = value;
                }
            });

            await DisplayFade(property, () => Faces, faceCode, outDuration, inDuration);
        }

        private async UniTask DisplayFade(Property<Color> property, Func<RoleBodyType> bodyType, string typeCode, float outDuration, float inDuration)
        {
            // 淡出动画序列  
            var fadeOutSequence = DOTween.Sequence();
            fadeOutSequence.Append(DOTween.ToAlpha(() => property.Member, color => property.Member = color, 0, outDuration)
                .SetEase(Ease.OutExpo));

            await fadeOutSequence.AsyncWaitForCompletion().AsUniTask();

            var result = IsEqualTypeCode(null, typeCode);

            if (!result.equalFace)
            {
                // 异步加载资源  

                BodyInfo bodyInfo = BodyInfoSettings.Main[typeCode];

                var sprites = await AssetLoader.LoadAssetsAsync(bodyInfo.Data);
                await bodyType.Invoke().Display(sprites.ToArray(), bodyInfo.Data);
            }

            // 创建新的序列以执行淡入动画  
            var fadeInSequence = DOTween.Sequence();
            fadeInSequence.Append(DOTween.ToAlpha(() => property.Member, color => property.Member = color, 1, inDuration)
                .SetEase(Ease.InExpo));

            await fadeInSequence.AsyncWaitForCompletion().AsUniTask();
        }


        public async UniTask DisplayBodyFade(string bodyCode, float outDuration = 0.25F, float inDuration = 0.33F)
        {
            var property = new Property<Color>(() => Faces.RoleUnits[0].SpriteContent.color, value =>
            {
                Body.RoleUnits[0].SpriteContent.color = value;
                foreach (var roleUnit in Faces.RoleUnits)
                {
                    roleUnit.SpriteContent.color = value;
                }
            });

            await DisplayFade(property, () => Body, bodyCode, outDuration, inDuration);
        }

        public async UniTask Init(Vector2 windowSize, string roleTye)
        {
            Type = roleTye;

            var contentTf = transform.Find("Content");
            Body = contentTf.Find("type_body").GetComponent<RoleBodyType>();
            Faces = contentTf.Find("type_face").GetComponent<RoleBodyType>();

            await Body.Init(nameof(Body), 1);
            await Faces.Init(nameof(Faces), 4);


            await BodyInfoSettings.LoadSettingsAsync();

            AssetLoader = await AssetLoaderCenter<Sprite>.LoadResourceLocations(roleTye);


            _windowSize = windowSize;
            Geometry = PropertyLink();
        }

        private IGeomTransforms PropertyLink()
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

            return new GeomTransforms(_windowSize, positionProperty, sizeProperty, scaleProperty, rotateProperty, colorProperty);
        }

        public T GetThis<T>() where T : UnityEngine.Component, ICharacterControl
        {
            return this as T;
        }


        /// <summary>
        /// 创建一个角色立绘控制器
        /// </summary>
        /// <param name="parent">立绘父节点 (*需要是 RectTransform)</param>
        /// <param name="roleTye">立绘控制器类型(名称) (*强烈建议您将这个控制器需要加载的标签名称作为参数名称)</param>
        /// <param name="windowSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> CreateRole<T>(Vector2 windowSize, Transform parent, string roleTye)
            where T : UnityEngine.Component, ICharacterControl
        {
            var webViewContentReferenceDeviceSo = (WebViewContentReferenceDeviceSo)await Resources.LoadAsync<WebViewContentReferenceDeviceSo>("Settings/RefPrefabricate");

            var template = await webViewContentReferenceDeviceSo.LoadGameObjectInstanceAssetAsync("Role Content Root Template", parent);
            template.name = $"RoleContentRoot#{roleTye}#";

            var gameObject = Instantiate(template, parent: parent);

            if (gameObject.AddComponent<T>() is not ICharacterControl contentRoot)
            {
                throw new InvalidCastException();
            }

            await contentRoot.Init(windowSize, roleTye);
            return (T)contentRoot;
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