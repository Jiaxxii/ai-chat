using System.Drawing;
using UnityEngine;
using Xiyu.GameFunction.CharacterComponent;

namespace Xiyu.GameFunction.GeometricTransformations
{
    public interface IGeomTransforms
    {
        Vector2 Size { get; }

        Vector2 Position { get; set; }

        Vector3 Scale { get; set; }

        Vector3 Rotate { get; set; }

        void MoveTo(Vector2 target);

        void Offset(Vector2 offset);

        void SetScale(Vector2 scale);

        void SetScale(float scale);

        void SetAngle(Vector3 angle);

        void SetAngle(float angle);

        public void MoveTo(ViewHorizontalAlign xAlign, ViewVerticalAlign yAlign);
        public float GetViewHorizontalAlignPoint(ViewHorizontalAlign align);

        public float GetViewVerticalAlignPoint(ViewVerticalAlign align);
    }
}