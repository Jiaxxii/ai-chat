using System;
using System.Collections;
using UnityEngine;
using Xiyu.VirtualLiveRoom;
using Xiyu.VirtualLiveRoom.View;

namespace Xiyu
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private RectTransform dm;
        [SerializeField] private Sprite sprite;

        private void Awake()
        {
            User.UserHeadSprite = sprite;
        }

        private IEnumerator Start()
        {
            var dc = FindObjectOfType<DanmuController>();

            yield return null;
        }
    }
}