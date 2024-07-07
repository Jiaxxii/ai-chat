using System;
using System.Collections;
using DG.Tweening;
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

        public GeomTransforms(Property<Vector2> positionSetter, Property<Vector2> sizeSetter, Property<Vector3> scaleSetter, Property<Vector3> rotateSetter,
            Property<Color> colorProperty)
        {
            _positionProperty = positionSetter;
            _sizeProperty = sizeSetter;
            _scaleProperty = scaleSetter;
            _rotateProperty = rotateSetter;
            _colorProperty = colorProperty;
        }

        private readonly Property<Vector2> _positionProperty;
        private readonly Property<Vector2> _sizeProperty;
        private readonly Property<Vector3> _scaleProperty;
        private readonly Property<Vector3> _rotateProperty;

        private readonly Property<Color> _colorProperty;

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

        public Color Color
        {
            get => _colorProperty.GetValue();
            set => _colorProperty.SetValue(value);
        }

        public GeomTransforms MoveTo(Vector2 target)
        {
            _positionProperty.SetValue(target);
            return this;
        }

        public GeomTransforms Offset(Vector2 offset)
        {
            _positionProperty.Member += offset;
            return this;
        }

        public GeomTransforms SetScale(Vector2 scale)
        {
            var z = _scaleProperty.Member.z;
            _scaleProperty.SetValue(new Vector3(scale.x, scale.y, z));
            return this;
        }

        public GeomTransforms SetScale(float scale)
        {
            var z = _scaleProperty.Member.z;
            _scaleProperty.SetValue(new Vector3(scale, scale, z));
            return this;
        }

        public GeomTransforms SetAngle(Vector3 angle)
        {
            _rotateProperty.SetValue(angle);
            return this;
        }

        public GeomTransforms SetAngle(float angle)
        {
            var raw = _rotateProperty.Member;
            _rotateProperty.SetValue(new Vector3(raw.x, raw.y, angle));
            return this;
        }

        public GeomTransforms MoveTo(ViewHorizontalAlign xAlign, ViewVerticalAlign yAlign)
        {
            var x = xAlign == ViewHorizontalAlign.Ignore ? Position.x : GetViewHorizontalAlignPoint(xAlign);

            var y = yAlign == ViewVerticalAlign.Ignore ? Position.y : GetViewVerticalAlignPoint(yAlign);

            Position = new Vector2(x, y);
            return this;
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

        public Tween DoFade(Color endColor, float duration)
        {
            return DOTween.To(() => Color, color => Color = color, endColor, duration);
        }

        public YieldInstruction WaitForDoNod(float offsetY = 100, float duration = 1.5F, int loops = 1, Ease ease = Ease.OutBounce)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position - new Vector2(0, offsetY), duration / 2F));
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position, duration / 2F));
            sequence.SetEase(ease);
            sequence.SetLoops(loops <= 1 ? 1 : loops, LoopType.Yoyo);
            return sequence.WaitForCompletion();
        }

        public YieldInstruction WaitForDoShake(float duration = 0.75F, float strength = 35F, int vibrato = 30, float randomness = 20f, bool fadeOut = true)
        {
            return DOTween.Shake(() => Position.ToV3(), pos => Position = pos.ToV2(), duration, strength, vibrato, randomness, true, fadeOut).WaitForCompletion();
        }

        public YieldInstruction WaitForDoShakeHead(float offsetX = 100, float duration = 0.75F, int loops = 1, Ease ease = Ease.InOutSine)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position - new Vector2(offsetX, 0), duration * .25F));
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position + new Vector2(offsetX, 0), duration * .5F));
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position, duration * .25F));
            sequence.SetEase(ease);
            sequence.SetLoops(loops <= 1 ? 1 : loops, LoopType.Yoyo);
            return sequence.WaitForCompletion();
        }

        public YieldInstruction WaitForDoJump(float offsetY = 150F, float duration = 0.75F, int loops = 1, Ease ease = Ease.OutBounce)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position + new Vector2(0, offsetY), duration * .5F));
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position, duration * .5F));
            sequence.SetEase(ease);
            sequence.SetLoops(loops <= 1 ? 1 : loops, LoopType.Yoyo);
            return sequence.WaitForCompletion();
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