using System;
using UnityEngine;
using Xiyu.GameFunction.GeometricTransformations;

namespace Xiyu.VirtualLiveRoom.Component.Character
{
    public class AishiCharacterController : CharacterController
    {
        protected override IGeomTransforms PropertyLink()
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

            return new AishiGeomTransform(WindowSize, positionProperty, sizeProperty, scaleProperty, rotateProperty, colorProperty);
        }
    }
}