using UnityEngine;

namespace Xiyu.GameFunction.GeometricTransformations
{
    public interface IGeomTransforms
    {
        Vector2 RawSize { get; }

        Vector2 Position { get; set; }

        Vector3 Scale { get; set; }

        Vector3 Rotate { get; set; }

         Vector2 SizeScaling { get; }
    }
}