using System.Collections.Generic;
using UnityEngine;
using Xiyu.CharacterIllustration;

namespace Xiyu.GameFunction.CharacterComponent
{
    public class RoleBodyType : MonoBehaviour
    {
        public string Type { get; private set; }
        public List<RoleUnit> RoleUnits { get; private set; }

        public RoleBodyType Init(string roleBodyTyp, int reserveNumber)
        {
            Type = roleBodyTyp;
            RoleUnits = new List<RoleUnit>(RoleUnit.CreateRole(transform, reserveNumber));
            return this;
        }

        public void Display(Sprite[] sprite, TransformInfo[] transformInfo)
        {
            if (sprite.Length != transformInfo.Length)
            {
                Debug.LogWarning("精灵图与位置信息的长度不是一致的，这可能不是预期的!");
            }

            _ = AutoFill(sprite.Length);

            for (var i = 0; i < sprite.Length; i++)
            {
                RoleUnits[i].Display(sprite[i], transformInfo[i]);
            }
        }

        private int AutoFill(int count)
        {
            // 判断是否足够显示
            var distance = Distance(count);

            switch (distance)
            {
                case > 0:
                {
                    // 多了，需要取消激活
                    for (var i = 1; i < distance; i++)
                        RoleUnits[^i].Hide();

                    break;
                }
                case < 0:
                    // 少了，得补充新的
                    RoleUnits.AddRange(RoleUnit.CreateRole(transform, Mathf.Abs(distance)));
                    break;
            }

            return distance;
        }

        private int Distance(int count) => RoleUnits.Count - count;
    }
}