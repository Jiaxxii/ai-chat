using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Xiyu.CharacterIllustrationResource;

namespace Xiyu.VirtualLiveRoom.Component.Character
{
    public class RoleBodyType : MonoBehaviour
    {
        public string Type { get; private set; }
        public List<RoleUnit> RoleUnits { get; private set; }

        public float AllAlpha
        {
            get => RoleUnits[0].Alpha;
            set
            {
                foreach (var roleUnit in RoleUnits) roleUnit.Alpha = value;
            }
        }

        public async UniTask<RoleBodyType> Init(string roleBodyTyp, int reserveNumber)
        {
            Type = roleBodyTyp;
            RoleUnits = new List<RoleUnit>(await RoleUnit.CreateRoleAsync(transform, reserveNumber));
            return this;
        }

        public async UniTask<RoleBodyType> Init(string roleBodyTyp, int reserveNumber, Sprite[] sprite, DataItem[] bodyInfos, bool active)
        {
            await Init(roleBodyTyp, reserveNumber);
            await Display(sprite, bodyInfos);

            foreach (var roleUnit in RoleUnits)
            {
                roleUnit.Active = active;
            }

            return this;
        }

        public async UniTask<RoleBodyType> Init(string roleBodyTyp, int reserveNumber, Sprite[] sprite, DataItem[] bodyInfos, float alpha)
        {
            await Init(roleBodyTyp, reserveNumber);
            await Display(sprite, bodyInfos);

            foreach (var roleUnit in RoleUnits)
            {
                roleUnit.Alpha = alpha;
            }

            return this;
        }

        public async UniTask<RoleBodyType> Init(string roleBodyTyp, int reserveNumber, Sprite[] sprite, DataItem[] bodyInfos, float duration, float startAlpha)
        {
            await Init(roleBodyTyp, reserveNumber);

            foreach (var roleUnit in RoleUnits)
            {
                roleUnit.Alpha = startAlpha;
            }

            await Display(sprite, bodyInfos, duration, false);

            return this;
        }

        public async UniTask Display(Sprite[] sprite, DataItem[] bodyInfos, float duration, bool smoothSwitch)
        {
            if (smoothSwitch)
            {
                await DOTween.To(() => RoleUnits[0].Alpha, v =>
                    {
                        foreach (var roleUnit in RoleUnits) roleUnit.Alpha = v;
                    }, 0, duration)
                    .AsyncWaitForCompletion()
                    .AsUniTask();
            }

            await Display(sprite, bodyInfos);

            await DOTween.To(() => RoleUnits[0].Alpha, v =>
                {
                    foreach (var roleUnit in RoleUnits) roleUnit.Alpha = v;
                }, 1, duration)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }

        public async UniTask Display(Sprite[] sprite, DataItem[] bodyInfos)
        {
            if (sprite.Length != bodyInfos.Length)
            {
                // ReSharper disable once MethodHasAsyncOverload
                Xiyu.LoggerSystem.LoggerManager.Instance.LogWarn("精灵图与位置信息的长度不是一致的，这可能不是预期的!");
            }

            _ = await AutoFill(sprite.Length);

            for (var i = 0; i < sprite.Length; i++)
            {
                RoleUnits[i].Display(sprite[i], bodyInfos[i]);
            }
        }

        private async UniTask<int> AutoFill(int count)
        {
            // 判断是否足够显示
            var distance = Distance(count);

            switch (distance)
            {
                case > 0:
                {
                    // 多了，需要取消激活
                    for (var i = 1; i <= distance; i++)
                        RoleUnits[^i].Hide();

                    break;
                }
                case < 0:
                    // 少了，得补充新的
                    var enumerable = await RoleUnit.CreateRoleAsync(transform, Mathf.Abs(distance));
                    RoleUnits.AddRange(enumerable);
                    break;
            }

            return distance;
        }

        private int Distance(int count) => RoleUnits.Count - count;
    }
}