using System;
using DG.Tweening;
using UnityEngine;
using Xiyu.Expand;

namespace Xiyu.GameFunction.GeometricTransformations
{
    public class GeomTransforms : IGeomTransforms
    {
        public GeomTransforms(Vector2 windowSize, Property<Vector2> positionSetter, Property<Vector2> sizeSetter, Property<Vector3> scaleSetter, Property<Vector3> rotateSetter)
        {
            WindowSize = windowSize;
            PositionProperty = positionSetter;
            SizeProperty = sizeSetter;
            ScaleProperty = scaleSetter;
            RotateProperty = rotateSetter;
        }

        public GeomTransforms(Vector2 windowSize, Property<Vector2> positionSetter, Property<Vector2> sizeSetter, Property<Vector3> scaleSetter, Property<Vector3> rotateSetter,
            Property<Color> colorProperty)
        {
            WindowSize = windowSize;
            PositionProperty = positionSetter;
            SizeProperty = sizeSetter;
            ScaleProperty = scaleSetter;
            RotateProperty = rotateSetter;
            ColorProperty = colorProperty;
        }

        protected readonly Property<Vector2> PositionProperty;
        protected readonly Property<Vector2> SizeProperty;
        protected readonly Property<Vector3> ScaleProperty;
        protected readonly Property<Vector3> RotateProperty;

        protected readonly Property<Color> ColorProperty;

        /// <summary>
        /// (重要)
        /// </summary>
        public Vector2 WindowSize { get; }

        public Vector2 Position
        {
            get => PositionProperty.GetValue();
            set => PositionProperty.SetValue(value);
        }

        public Vector3 Scale
        {
            get => ScaleProperty.GetValue();
            set => ScaleProperty.SetValue(value);
        }

        public Vector3 Rotate
        {
            get => RotateProperty.GetValue();
            set => RotateProperty.SetValue(value);
        }

        public Vector2 RawSize => SizeProperty.GetValue();

        public Color Color
        {
            get => ColorProperty.GetValue();
            set => ColorProperty.SetValue(value);
        }

        public Vector2 SizeScaling => RawSize * Scale.ToV2();

        public GeomTransforms MoveTo(Vector2 target)
        {
            PositionProperty.SetValue(target);
            return this;
        }

        public GeomTransforms Offset(Vector2 offset)
        {
            PositionProperty.Member += offset;
            return this;
        }

        public GeomTransforms SetScale(Vector2 scale)
        {
            var z = ScaleProperty.Member.z;
            ScaleProperty.SetValue(new Vector3(scale.x, scale.y, z));
            return this;
        }

        public GeomTransforms SetScale(float scale)
        {
            var z = ScaleProperty.Member.z;
            ScaleProperty.SetValue(new Vector3(scale, scale, z));
            return this;
        }

        public GeomTransforms SetAngle(Vector3 angle)
        {
            RotateProperty.SetValue(angle);
            return this;
        }

        public GeomTransforms SetAngle(float angle)
        {
            var raw = RotateProperty.Member;
            RotateProperty.SetValue(new Vector3(raw.x, raw.y, angle));
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
                ViewHorizontalAlign.Left => -WindowSize.x.Half() + (RawSize.x.Half() * Scale.x),
                ViewHorizontalAlign.Right => WindowSize.x.Half() - (RawSize.x.Half() * Scale.x),

                _ => throw new ArgumentOutOfRangeException(nameof(align), align, null)
            };
        }

        public float GetViewVerticalAlignPoint(ViewVerticalAlign align)
        {
            return align switch
            {
                ViewVerticalAlign.Ignore => throw new ArgumentException(),

                ViewVerticalAlign.Center => 0,
                ViewVerticalAlign.Top => WindowSize.y.Half() - (RawSize.y.Half() * Scale.y),
                ViewVerticalAlign.Bottom => -WindowSize.y.Half() + (RawSize.y.Half() * Scale.y),

                _ => throw new ArgumentOutOfRangeException(nameof(align), align, null)
            };
        }

        public Tween DoFade(Color endColor, float duration)
        {
            return DOTween.To(() => Color, color => Color = color, endColor, duration);
        }

        public Sequence DoNod(float offsetY = 100, float duration = 1.5F, int loops = 1, Ease ease = Ease.OutBounce)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position - new Vector2(0, offsetY), duration / 2F));
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position, duration / 2F));
            sequence.SetEase(ease);
            sequence.SetLoops(loops <= 1 ? 1 : loops, LoopType.Yoyo);
            return sequence;
        }

        public Tween DoShake(float duration = 0.75F, float strength = 35F, int vibrato = 30, float randomness = 20f, bool fadeOut = true)
        {
            return DOTween.Shake(() => Position.ToV3(), pos => Position = pos.ToV2(), duration, strength, vibrato, randomness, true, fadeOut);
        }

        public Sequence DoShakeHead(float offsetX = 100, float duration = 0.75F, int loops = 1, Ease ease = Ease.InOutSine)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position - new Vector2(offsetX, 0), duration * .25F));
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position + new Vector2(offsetX, 0), duration * .5F));
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position, duration * .25F));
            sequence.SetEase(ease);
            sequence.SetLoops(loops <= 1 ? 1 : loops, LoopType.Yoyo);
            return sequence;
        }

        public Sequence DoJump(float offsetY = 150F, float duration = 0.75F, int loops = 1, Ease ease = Ease.OutBounce)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position + new Vector2(0, offsetY), duration * .5F));
            sequence.Append(DOTween.To(() => Position, pos => Position = pos, Position, duration * .5F));
            sequence.SetEase(ease);
            sequence.SetLoops(loops <= 1 ? 1 : loops, LoopType.Yoyo);
            return sequence;
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