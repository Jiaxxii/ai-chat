using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xiyu.Expand
{
    public static class Expand
    {
        /// <returns><see cref="value"/> / 2F</returns>
        // [MethodImpl(MethodImplOptions.InternalCall)]
        public static float Half(this float value) => value / 2f;

        /// <returns><see cref="value"/> / 2F</returns>
        // [MethodImpl(MethodImplOptions.InternalCall)]
        public static Vector2 Half(this Vector2 value) => value / 2f;

        /// <returns><see cref="value"/> / 2F</returns>
        // [MethodImpl(MethodImplOptions.InternalCall)]
        public static Vector3 Half(this Vector3 value) => value / 2f;

        // [MethodImpl(MethodImplOptions.InternalCall)]
        public static Vector3 ToV3(this Vector2 vector, float z = 0F) => new(vector.x, vector.y, z);


        // [MethodImpl(MethodImplOptions.InternalCall)]
        public static Vector2 ToV2(this Vector3 vector) => new(vector.x, vector.y);


        // [MethodImpl(MethodImplOptions.InternalCall)]
        public static Color SetAlpha(this Color color, float a) => new(color.r, color.g, color.b, a);

        // [MethodImpl(MethodImplOptions.InternalCall)]
        public static string ToHexadecimalString(this Color color) => $"{Mathf.CeilToInt(color.r * 255):x2}{Mathf.CeilToInt(color.g * 255):x2}{Mathf.CeilToInt(color.b * 255):x2}";

        public static TMP_InputField InputText(this TextMeshProUGUI text)
        {
            var inputField = text.gameObject.AddComponent<TMP_InputField>();
            inputField.transition = Selectable.Transition.None;
            inputField.textComponent = text;
            inputField.text = text.text;

            inputField.ActivateInputField();

            return inputField;
        }

        public static float MapFloat(this float value, float inMin, float inMax, float outMin, float outMax)
        {
            return (outMax - outMin) / (inMax - inMin) * (value - inMin) + outMin;
        }


        public static float InverseMapFloat(this float value, float outMin, float outMax, float inMin, float inMax)
        {
            return (inMax - inMin) / (outMax - outMin) * (value - outMin) + inMin;
        }


#if UNITY_EDITOR
        public static T GetRandom<T>(this IEnumerable<T> collect)
        {
            var array = collect.ToArray();
            return array[Random.Range(0, array.Length)];
        }
#endif

        // [MethodImpl(MethodImplOptions.InternalCall)]
        public static RectTransform AsRectTransform(this Transform transform) => (RectTransform)transform;

        // public static MessageProcessor.MessageJson ToMessageJson(this ResponseData responseData)
        // {
        //     return JsonConvert.DeserializeObject<MessageProcessor.MessageJson>(responseData.Result);
        // }
    }
}