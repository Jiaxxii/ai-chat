using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Xiyu.CharacterIllustrationResource;
using Xiyu.CharacterIllustrationResource.Expand;
using Xiyu.GameFunction.GeometricTransformations;

namespace Xiyu.VirtualLiveRoom.Component.Character
{
    public partial class CharacterController : MonoBehaviour, ICharacterControl
    {
        public string Type { get; private set; }
        public RoleBodyType Body { get; private set; }
        public RoleBodyType Faces { get; private set; }
        public IGeomTransforms Geometry { get; private set; }


        // 用于存放上一次的使用的 typeCode
        // 如果使用时有相等则不再重复加载
        private (string bodyCode, string faceCode) _lastTypeCode = (string.Empty, string.Empty);

        // protected SpriteAssetLoader SpriteAssetLoader;

        private AssetLoader<Sprite> _assetLoader;

        protected Vector2 WindowSize;

        // private UniTask? _bodyShowTask;
        // private UniTask? _bodyLoadTask;

        public async UniTask Display(string bodyCode, string faceCode)
        {
            // 如何 typeCode 与上一次一样就没必要设置了，同时也可以提供性能
            var result = IsEqualTypeCode(bodyCode, faceCode);
            if (result is { equalBody: false, equalFace: false })
            {
                var asset = await LoadAllSpriteAsset(bodyCode, faceCode);
                await UniTask.WhenAll(
                    Body.Display(asset.bodys.ToArray(), BodyInfoSettings.Main[bodyCode].Data),
                    Faces.Display(asset.faces.ToArray(), BodyInfoSettings.Main[faceCode].Data));
                return;
            }

            if (result is { equalBody: true, equalFace: false })
            {
                var asset = await LoadFacesSpriteAsset(faceCode);
                await Faces.Display(asset.ToArray(), BodyInfoSettings.Main[faceCode].Data);
                return;
            }

            if (result is { equalBody: false, equalFace: true })
            {
                var asset = await LoadBodySpriteAsset(bodyCode);
                await Body.Display(asset.ToArray(), BodyInfoSettings.Main[bodyCode].Data);
            }
        }

        private async UniTask<IEnumerable<Sprite>> LoadBodySpriteAsset(string bodyCode)
        {
            BodyInfo bodyInfo = BodyInfoSettings.Main[bodyCode];

            return await _assetLoader.LoadAssetsAsync(bodyInfo.Data);
        }

        private async UniTask<IEnumerable<Sprite>> LoadFacesSpriteAsset(string faceCode)
        {
            BodyInfo faceInfo = BodyInfoSettings.Main[faceCode];

            return await _assetLoader.LoadAssetsAsync(faceInfo.Data);
        }

        private async UniTask<(IEnumerable<Sprite> bodys, IEnumerable<Sprite> faces)> LoadAllSpriteAsset(string bodyCode, string faceCode)
        {
            // 设置身体
            BodyInfo bodyInfo = BodyInfoSettings.Main[bodyCode];
            BodyInfo faceInfo = BodyInfoSettings.Main[faceCode];


            return await UniTask.WhenAll(
                _assetLoader.LoadAssetsAsync(bodyInfo.Data),
                _assetLoader.LoadAssetsAsync(faceInfo.Data));
        }

        public async UniTask Display(string bodyCode, string faceCode, float duration, bool smoothSwitch = true)
        {
            // 如何 typeCode 与上一次一样就没必要设置了，同时也可以提供性能
            var result = IsEqualTypeCode(bodyCode, faceCode);
            if (result is { equalBody: false, equalFace: false })
            {
                var asset = await LoadAllSpriteAsset(bodyCode, faceCode);
                await UniTask.WhenAll(
                    Body.Display(asset.bodys.ToArray(), BodyInfoSettings.Main[bodyCode].Data, duration, smoothSwitch),
                    Faces.Display(asset.faces.ToArray(), BodyInfoSettings.Main[faceCode].Data, duration, smoothSwitch));
                return;
            }

            if (result is { equalBody: true, equalFace: false })
            {
                var asset = await LoadFacesSpriteAsset(faceCode);
                await Faces.Display(asset.ToArray(), BodyInfoSettings.Main[faceCode].Data, duration, smoothSwitch);
                return;
            }

            if (result is { equalBody: false, equalFace: true })
            {
                var asset = await LoadBodySpriteAsset(bodyCode);
                await Body.Display(asset.ToArray(), BodyInfoSettings.Main[bodyCode].Data, duration, smoothSwitch);
            }
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

            // 淡出动画序列  
            await DOTween.ToAlpha(() => property.Member, color => property.Member = color, 0, outDuration)
                .SetEase(Ease.OutExpo)
                .AsyncWaitForCompletion()
                .AsUniTask();

            var asset = await LoadFacesSpriteAsset(faceCode);
            await Faces.Display(asset.ToArray(), BodyInfoSettings.Main[faceCode].Data);

            // 创建新的序列以执行淡入动画  
            await DOTween.ToAlpha(() => property.Member, color => property.Member = color, 1, inDuration)
                .SetEase(Ease.InExpo)
                .AsyncWaitForCompletion()
                .AsUniTask();
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

            // 淡出动画序列  
            await DOTween.ToAlpha(() => property.Member, color => property.Member = color, 0, outDuration)
                .SetEase(Ease.OutExpo)
                .AsyncWaitForCompletion()
                .AsUniTask();

            var asset = await LoadBodySpriteAsset(bodyCode);
            await Body.Display(asset.ToArray(), BodyInfoSettings.Main[bodyCode].Data);

            // 创建新的序列以执行淡入动画  
            await DOTween.ToAlpha(() => property.Member, color => property.Member = color, 1, inDuration)
                .SetEase(Ease.InExpo)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }

        public async UniTask Init(Vector2 windowSize, string roleTye)
        {
            await InitCharacterController(windowSize, roleTye);


            await UniTask.WhenAll(
                Body.Init(nameof(Body), 1),
                Faces.Init(nameof(Faces), 4)
            );
        }

        public async UniTask Init(Vector2 windowSize, string roleType, string bodyCode, string faceCode, bool active = false)
        {
            await InitCharacterController(windowSize, roleType);

            var asset = await LoadAllSpriteAsset(bodyCode, faceCode);

            await UniTask.WhenAll(
                Body.Init(nameof(Body), 1, asset.bodys.ToArray(), BodyInfoSettings.Main[bodyCode].Data, active),
                Faces.Init(nameof(Faces), 4, asset.faces.ToArray(), BodyInfoSettings.Main[faceCode].Data, active)
            );
        }

        public async UniTask Init(Vector2 windowSize, string roleType, string bodyCode, string faceCode, float alpha = 0F)
        {
            await InitCharacterController(windowSize, roleType);

            var asset = await LoadAllSpriteAsset(bodyCode, faceCode);

            await UniTask.WhenAll(
                Body.Init(nameof(Body), 1, asset.bodys.ToArray(), BodyInfoSettings.Main[bodyCode].Data, alpha),
                Faces.Init(nameof(Faces), 4, asset.faces.ToArray(), BodyInfoSettings.Main[faceCode].Data, alpha)
            );
        }


        public async UniTask Init(Vector2 windowSize, string roleType, string bodyCode, string faceCode, float duration, float startAlpha)
        {
            await InitCharacterController(windowSize, roleType);

            var asset = await LoadAllSpriteAsset(bodyCode, faceCode);

            await UniTask.WhenAll(
                Body.Init(nameof(Body), 1, asset.bodys.ToArray(), BodyInfoSettings.Main[bodyCode].Data, duration, startAlpha),
                Faces.Init(nameof(Faces), 4, asset.faces.ToArray(), BodyInfoSettings.Main[faceCode].Data, duration, startAlpha)
            );
        }


        public void SetGroupAlpha(float alpha)
        {
            Body.AllAlpha = alpha;
            Faces.AllAlpha = alpha;
        }

        private async UniTask InitCharacterController(Vector2 windowSize, string roleTye)
        {
            Type = roleTye;

            var contentTf = transform.Find("Content");
            Body = contentTf.Find("type_body").GetComponent<RoleBodyType>();
            Faces = contentTf.Find("type_face").GetComponent<RoleBodyType>();

            WindowSize = windowSize;
            Geometry = PropertyLink();


            await BodyInfoSettings.LoadSettingsAsync();

            _assetLoader = await AssetLoaderCenter<Sprite>.LoadResourceLocations(roleTye);
        }


        protected virtual IGeomTransforms PropertyLink()
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

            return new GeomTransforms(WindowSize, positionProperty, sizeProperty, scaleProperty, rotateProperty, colorProperty);
        }

        public T GetThis<T>() where T : UnityEngine.Component, ICharacterControl
        {
            return this as T;
        }


        private (bool equalBody, bool equalFace) IsEqualTypeCode(string bodyCode, string faceCode)
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