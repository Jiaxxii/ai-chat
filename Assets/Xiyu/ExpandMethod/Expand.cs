using UnityEngine;

namespace Xiyu.ExpandMethod
{
    public static class Expand
    {
        /// <returns><see cref="value"/> / 2F</returns>
        public static float Half(this float value) => value / 2f;

        /// <returns><see cref="value"/> / 2F</returns>
        public static Vector2 Half(this Vector2 value) => value / 2f;

        /// <returns><see cref="value"/> / 2F</returns>
        public static Vector3 Half(this Vector3 value) => value / 2f;
        
        public static Vector3 ToV3(this Vector2 vector, float z = 0F) => new(vector.x, vector.y, z);
        public static Vector2 ToV2(this Vector3 vector) => new(vector.x, vector.y);
    }
}