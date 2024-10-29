using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Xiyu.CharacterIllustration;
using Xiyu.CharacterIllustrationResource;
using Xiyu.GameFunction.CharacterComponent;
using Xiyu.Settings;

namespace Xiyu.VirtualLiveRoom.Component.Character
{
    public class RoleUnit : MonoBehaviour
    {
        public Image SpriteContent { get; private set; }

        // public TransformInfo TransformInfo { get; private set; }

        public bool Active
        {
            get => SpriteContent.gameObject.activeSelf;
            set => SpriteContent.gameObject.SetActive(value);
        }

        private RoleUnit Init()
        {
            SpriteContent = GetComponent<Image>();
            return this;
        }


        public void Display(Sprite sprite, DataItem dataItem)
        {
            SetStyle(sprite, dataItem.Size, dataItem.Position);
            Active = true;
        }

        public void Hide(bool isClear = false)
        {
            Active = false;
            if (isClear)
            {
                Clear();
            }
        }

        public void Clear() => SetStyle(null, Vector2.zero, Vector2.zero);

        private void SetStyle(Sprite sprite, Vector2 size, Vector2 position)
        {
            SpriteContent.sprite = sprite;
            SpriteContent.rectTransform.sizeDelta = size;
            SpriteContent.rectTransform.anchoredPosition = position;
        }

        internal static async UniTask<IEnumerable<RoleUnit>> CreateRoleAsync(Transform parent, int reserveNumber = 4)
        {
            var webViewContentReferenceDeviceSo = (WebViewContentReferenceDeviceSo)await Resources.LoadAsync<WebViewContentReferenceDeviceSo>("Settings/RefPrefabricate");

            var roleUnitList = new List<RoleUnit>(reserveNumber);

            var roleUnits = parent.GetComponentsInChildren<RoleUnit>();

            if (roleUnits != null && roleUnits.Length != 0)
            {
                // 表示有可用的组件
                roleUnitList.AddRange(roleUnits.Select(roleUnit => roleUnit.Init()));
                reserveNumber -= roleUnitList.Count;
            }

            for (var i = 0; i < reserveNumber; i++)
            {
                var roleUnitRect = await webViewContentReferenceDeviceSo
                    .LoadComponentAssetAsync<RectTransform>("Role Unit Template", parent);

                roleUnitRect.name = $"RoleUnit#{i}#";

                roleUnitRect.pivot = new Vector2(.5f, .5f);
                roleUnitRect.localScale = Vector3.one;

                roleUnitList.Add(roleUnitRect.GetComponent<RoleUnit>().Init());
            }

            return roleUnitList;
        }
    }
}