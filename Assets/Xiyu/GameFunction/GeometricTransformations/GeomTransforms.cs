using System;
using TMPro;
using UnityEngine;
using Xiyu.ExpandMethod;
using Xiyu.GameFunction.SceneView;

namespace Xiyu.GameFunction.GeometricTransformations
{
    public class GeomTransforms : IGeomTransforms
    {
        public GeomTransforms(Property<Vector2> positionSetter, Property<Vector2> sizeSetter, Property<Vector3> scaleSetter, Property<Vector3> rotateSetter)
        {
            _positionProperty = positionSetter;
            _sizeProperty = sizeSetter;
            _scaleProperty = scaleSetter;
            _rotateProperty = rotateSetter;
        }

        private readonly Property<Vector2> _positionProperty;
        private readonly Property<Vector2> _sizeProperty;
        private readonly Property<Vector3> _scaleProperty;
        private readonly Property<Vector3> _rotateProperty;

        public Vector2 Position
        {
            get => _positionProperty.GetValue();
            set => _positionProperty.SetValue(value);
        }

        public Vector3 Scale
        {
            get => _scaleProperty.GetValue();
            set => _scaleProperty.SetValue(value);
        }

        public Vector3 Rotate
        {
            get => _rotateProperty.GetValue();
            set => _rotateProperty.SetValue(value);
        }

        public Vector2 Size => _sizeProperty.GetValue();

        public void MoveTo(Vector2 target) => _positionProperty.SetValue(target);

        public void Offset(Vector2 offset) => _positionProperty.Member += offset;

        public void SetScale(Vector2 scale)
        {
            var z = _scaleProperty.Member.z;
            _scaleProperty.SetValue(new Vector3(scale.x, scale.y, z));
        }

        public void SetScale(float scale)
        {
            var z = _scaleProperty.Member.z;
            _scaleProperty.SetValue(new Vector3(scale, scale, z));
        }

        public void SetAngle(Vector3 angle) => _rotateProperty.SetValue(angle);

        public void SetAngle(float angle)
        {
            var raw = _rotateProperty.Member;
            _rotateProperty.SetValue(new Vector3(raw.x, raw.y, angle));
        }

        public void MoveTo(ViewHorizontalAlign xAlign, ViewVerticalAlign yAlign)
        {
            var x = xAlign == ViewHorizontalAlign.Ignore ? Position.x : GetViewHorizontalAlignPoint(xAlign);

            var y = yAlign == ViewVerticalAlign.Ignore ? Position.y : GetViewVerticalAlignPoint(yAlign);

            Position = new Vector2(x, y);
        }

        public float GetViewHorizontalAlignPoint(ViewHorizontalAlign align)
        {
            return align switch
            {
                ViewHorizontalAlign.Ignore => throw new ArgumentException(),

                ViewHorizontalAlign.Center => 0,
                ViewHorizontalAlign.Left => -GameInsView.ScreenSize.x.Half() + (Size.x.Half() * Scale.x),
                ViewHorizontalAlign.Right => GameInsView.ScreenSize.x.Half() - (Size.x.Half() * Scale.x),

                _ => throw new ArgumentOutOfRangeException(nameof(align), align, null)
            };
        }

        public float GetViewVerticalAlignPoint(ViewVerticalAlign align)
        {
            return align switch
            {
                ViewVerticalAlign.Ignore => throw new ArgumentException(),

                ViewVerticalAlign.Center => 0,
                ViewVerticalAlign.Top => GameInsView.ScreenSize.y.Half() - (Size.y.Half() * Scale.y),
                ViewVerticalAlign.Bottom => -GameInsView.ScreenSize.y.Half() + (Size.y.Half() * Scale.y),

                _ => throw new ArgumentOutOfRangeException(nameof(align), align, null)
            };
        }
    }


    /// <summary>
    /// 水平对齐方式
    /// </summary>
    public enum ViewHorizontalAlign
    {
        Ignore,
        Center,
        Left,
        Right
    }

    /// <summary>
    /// 垂直对齐方式
    /// </summary>
    public enum ViewVerticalAlign
    {
        Ignore,
        Center,
        Top,
        Bottom
    }


}