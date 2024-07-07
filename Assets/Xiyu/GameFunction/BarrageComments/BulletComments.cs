using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.ExpandMethod;

namespace Xiyu.GameFunction.BarrageComments
{
    public class BulletComments : MonoBehaviour
    {
        [SerializeField] private Image panel;
        [SerializeField] private TextMeshProUGUI bulletComment;


        public int Index { get; set; }
        public float BulletSpeed { get; set; }

        public Vector3 StartPos { get; private set; }
        public Vector3 TargetPos { get; private set; }

        public bool IsPlay { get; private set; }

        public RectTransform ContentRect => panel.rectTransform;

        public Image Panel => panel;

        public TextMeshProUGUI BulletComment => bulletComment;


        private Action<BulletComments> _onMoveToTarget;
        private Action<BulletComments> _onPointerEnterEventHandler;
        private Action<BulletComments> _onPointerExitEventHandler;
        private Action<BulletComments> _onPointerClickEventHandler;


        public BulletComments Init(Vector3 start, Vector3 target, float bulletSpeed, Action<BulletComments> onMoveToTarget)
        {
            StartPos = start;
            TargetPos = target;
            BulletSpeed = bulletSpeed;
            _onMoveToTarget = onMoveToTarget;
            panel.rectTransform.anchoredPosition = start.ToV2();

            IsPlay = false;

            return this;
        }


        public void Registers(Action<BulletComments> onPointerEnter, Action<BulletComments> onPointerExit, Action<BulletComments> onPointerClick)
        {
            _onPointerEnterEventHandler += onPointerEnter;
            _onPointerExitEventHandler += onPointerExit;
            _onPointerClickEventHandler += onPointerClick;
        }
        public void ReRegisters(Action<BulletComments> onPointerEnter, Action<BulletComments> onPointerExit, Action<BulletComments> onPointerClick)
        {
            _onPointerEnterEventHandler -= onPointerEnter;
            _onPointerExitEventHandler -= onPointerExit;
            _onPointerClickEventHandler -= onPointerClick;
        }
        public bool IsSeeAll()
        {
            var rt = panel.rectTransform;
            return rt.anchoredPosition.x + rt.sizeDelta.x < StartPos.x;
        }


        public void SetActive(bool value)
        {
            if (IsPlay)
            {
                Debug.LogWarning($"在弹幕移动时改变激活状态为:{value} 可能不是预期的行为!");
            }

            panel.gameObject.SetActive(value);
        }

        public void UpData(string message, float maxHeight)
        {
            bulletComment.text = message;
            panel.rectTransform.sizeDelta = new Vector2(bulletComment.preferredWidth, Mathf.Clamp(bulletComment.preferredHeight, 0, maxHeight));
        }

        public void UpData(string message, float maxHeight, float fontSize)
        {
            bulletComment.fontSize = fontSize;
            UpData(message, maxHeight);
        }

        public void UpData(string message, float maxHeight, float fontSize, Color fontColor)
        {
            bulletComment.color = fontColor;
            UpData(message, maxHeight, fontSize);
        }

        public void Play()
        {
            IsPlay = true;
        }

        public void Stop()
        {
            IsPlay = false;
        }

        public void OnPointerEnter()
        {
            _onPointerEnterEventHandler?.Invoke(this);
        }

        public void OnPointerExit()
        {
            _onPointerExitEventHandler?.Invoke(this);
        }

        public void OnPointerClick()
        {
            _onPointerClickEventHandler?.Invoke(this);
        }

        private void Update()
        {
            if (!IsPlay) return;

            var target = new Vector2(TargetPos.x - panel.rectTransform.sizeDelta.x, TargetPos.y);
            if (Vector2.Distance(panel.rectTransform.anchoredPosition, target) <= 0.01F)
            {
                IsPlay = false;
                panel.rectTransform.anchoredPosition = target;
                _onMoveToTarget.Invoke(this);
            }
            else
            {
                IsPlay = true;
                panel.rectTransform.anchoredPosition = Vector2.MoveTowards(panel.rectTransform.anchoredPosition, target, Time.deltaTime * BulletSpeed);
            }
        }
    }
}