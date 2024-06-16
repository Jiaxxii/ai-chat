using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Xiyu.GameFunction.CharacterComponent;
using Xiyu.GameFunction.InputComponent;
using Xiyu.GameFunction.SceneView;
using Random = UnityEngine.Random;

namespace Xiyu.GameFunction.BarrageComments
{
    public class BulletCommentsScreenView : MonoBehaviour
    {
        [SerializeField] private float bulletSpeed = 1F;

        [Header("弹幕设置")] [Tooltip("弹幕内容最大容量")] [SerializeField]
        private float maxHeight;

        [SerializeField] private float minHeight;
        [Tooltip("弹幕行数")] [SerializeField] private int rowCount;
        [Tooltip("弹幕字体大小")] [SerializeField] private Vector2 fontSize;


        private ObjectPool<BulletComments> _pool;

        private readonly List<List<BulletComments>> _bulletComments = new();

        private float _lastY;

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0.5F, 3F));
                var now = DateTime.Now;
                SendBulletComment($"{now.Year}年{now.Year}月{now.Day:00}日 {now.Hour}时{now.Minute:00}分钟{now.Second:00}秒{now.Millisecond}毫秒");
            }
        }

        private void Awake()
        {
            var obj = CharacterContentRoot.PreformScriptableObject.Table["Bullet Comment"].Preform.gameObject;
            _pool = new ObjectPool<BulletComments>(
                () =>
                {
                    var bullet = Instantiate(obj, transform).GetComponent<BulletComments>();
                    bullet.SetActive(false);
                    bullet.Registers(OnPointerEnter, OnPointerExit, OnPointerClick);
                    return bullet;
                },
                bullet => bullet.SetActive(true),
                bullet => bullet.SetActive(false));

            for (var i = 0; i < rowCount; i++)
            {
                _bulletComments.Add(new List<BulletComments>());
            }
        }

        public void SetBulletSpeed(float speed)
        {
            foreach (var item in _bulletComments.SelectMany(row => row))
            {
                item.BulletSpeed = speed;
            }
        }

        public void SendBulletComment(string message)
        {
            var bullet = _pool.Get();

            // 从上向下遍历每一行
            for (var i = 0; i < _bulletComments.Count; i++)
            {
                var rowItems = _bulletComments[i];

                // 如果当前行的最后一条弹幕已经全部显示
                var lastBullet = rowItems.Count != 0 ? rowItems[^1] : null;

                // 这一行没有弹幕 或 最后一个弹幕没有完全显示时直接进入下一轮循环
                if (lastBullet != null && !lastBullet.IsSeeAll()) continue;


                var start = new Vector3(0, maxHeight / (rowCount - 1) * -i - minHeight, 0);
                var end = new Vector3(-GameInsView.ScreenSize.x, start.y, 0);

                bullet.Init(start, end, bulletSpeed, v =>
                {
                    rowItems.Remove(v);
                    _pool.Release(v);
                });

                bullet.Index = i;

                rowItems.Add(bullet);

                bullet.UpData(message, maxHeight / (rowCount - 1), Random.Range(fontSize.x, fontSize.y));
                bullet.Play();
                return;
            }
        }


        private void OnPointerEnter(BulletComments bulletComment)
        {
            bulletComment.BulletSpeed /= 2F;
            bulletComment.Panel.color = new Color(1, 1, 1, .3F);
        }

        private void OnPointerExit(BulletComments bulletComment)
        {
            bulletComment.BulletSpeed *= 2F;
            bulletComment.Panel.color = Color.clear;
        }

        private void OnPointerClick(BulletComments bulletComment)
        {
            // 让弹幕停止
            bulletComment.BulletSpeed = 0f;

            // 移除托管
            _bulletComments[bulletComment.Index].Remove(bulletComment);

            // 声明委托
            UnityAction<string> onValueChange = content => bulletComment.UpData(content, maxHeight / (rowCount - 1));

            // 注册文本输入事件 与 提交事件
            InputText.Instance.OnValueChangedEventHandler(onValueChange).OnSubmitEventHandler(OnSubmit)
                // 打开文本输入框 并且聚焦
                .Active(true).SetFocus();


            return;

            void OnSubmit(string content)
            {
                bulletComment.Stop();
                _pool.Release(bulletComment);
                SendBulletComment(content);

                // 取消委托
                InputText.Instance.RemoveValueChangedEventHandler(onValueChange).RemoveSubmitEventHandler(OnSubmit)
                    // 关闭对话框
                    .Active(false);
            }
        }
    }
}