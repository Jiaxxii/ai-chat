using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using Xiyu.ExpandMethod;
using Xiyu.GameFunction.CharacterComponent;
using Xiyu.GameFunction.InputComponent;
using Xiyu.GameFunction.SceneView;
using Random = UnityEngine.Random;

namespace Xiyu.GameFunction.BarrageComments
{
    public delegate void BulletCommentsShip(BulletComments bullet);


    public class BulletCommentsScreenView : MonoBehaviour
    {
        [SerializeField] private float bulletSpeed = 1F;

        [Header("弹幕设置")] [Tooltip("弹幕内容最大容量")] [SerializeField]
        private float maxHeight;

        [SerializeField] private float minHeight;
        [Tooltip("弹幕行数")] [SerializeField] private int rowCount;
        [Tooltip("弹幕字体大小")] [SerializeField] private Vector2 fontSize;

        [Tooltip("发送弹幕时延迟随机时间后打印结果")] [SerializeField]
        private Vector2 sendDelaySecondRange = new(2F, 5F);


        private ObjectPool<BulletComments> _pool;

        private readonly List<List<BulletComments>> _bulletComments = new();

        private readonly Dictionary<string, BulletCommentsShip> _bulletCommentsShipsMap = new();

        private readonly Queue<string> _bulletBufferQueue = new();

        private Transform _selectObj;

        public event Action<string, float> OnBulletCommentSubmitEventHandler;


        private bool _isSelect;

        private IEnumerator Start()
        {
            // var str = new []
            // {
            //     "？？？","WOW","耗康","服从调剂","哇库"
            // }
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0.1F, 1F));
                var now = DateTime.Now;
                // {now.Year}年{now.Year}月{now.Day:00}日 
                SendBulletComment($"{now.Hour}时{now.Minute:00}分钟{now.Second:00}秒");
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

            _bulletCommentsShipsMap.Add("选择", bullet =>
            {
                bullet.BulletSpeed /= 2F;
                bullet.Panel.color = new Color(1, 1, 1, .3F);
                bullet.Panel.transform.SetAsLastSibling();
            });
            _bulletCommentsShipsMap.Add("取消选择", bullet =>
            {
                bullet.BulletSpeed *= 2F;
                bullet.Panel.color = Color.clear;
            });
            _bulletCommentsShipsMap.Add("默认", bullet =>
            {
                bullet.BulletSpeed = bulletSpeed;
                bullet.Panel.color = Color.clear;
                if (bullet.IsPlay)
                {
                    bullet.Stop();
                }

                bullet.ContentRect.localScale = Vector3.one;
            });
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
            // 从上向下遍历每一行
            for (var i = 0; i < _bulletComments.Count; i++)
            {
                var rowItems = _bulletComments[i];

                // 如果当前行的最后一条弹幕已经全部显示
                var lastBullet = rowItems.Count != 0 ? rowItems[^1] : null;

                // 这一行没有弹幕 或 最后一个弹幕没有完全显示时直接进入下一轮循环
                if (lastBullet != null && !lastBullet.IsSeeAll())
                {
                    continue;
                }

                var bullet = _pool.Get();

                // 如果缓存队列中有弹幕 就先优先让它来
                if (_bulletBufferQueue.Count != 0)
                {
                    var bufferMessage = message;

                    message = _bulletBufferQueue.Dequeue();

                    _bulletBufferQueue.Enqueue(bufferMessage);
                }

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

                if (_selectObj != null)
                {
                    _selectObj.SetAsLastSibling();
                }

                return;
            }

            _bulletBufferQueue.Enqueue(message);
        }


        private void OnPointerEnter(BulletComments bulletComment)
        {
            if (_isSelect)
            {
                return;
            }

            _bulletCommentsShipsMap["选择"].Invoke(bulletComment);
        }

        private void OnPointerExit(BulletComments bulletComment)
        {
            if (_isSelect)
            {
                return;
            }

            _bulletCommentsShipsMap["取消选择"].Invoke(bulletComment);
        }


        private void OnPointerClick(BulletComments bulletComment)
        {
            if (_isSelect)
            {
                return;
            }

            Transform transform1;
            (transform1 = bulletComment.Panel.transform).SetAsLastSibling();
            _selectObj = transform1;

            _isSelect = true;

            // 移除托管
            _bulletComments[bulletComment.Index].Remove(bulletComment);

            var rawSpeed = bulletSpeed;

            bulletSpeed = 0;
            SetBulletSpeed(0);

            // 让弹幕停止
            bulletComment.Stop();

            var scale = Vector3.one * 1.5F;
            var targetPosX = -GameInsView.ScreenSize.x.Half() - bulletComment.ContentRect.sizeDelta.x.Half() * scale.x;
            var targetPosY = -GameInsView.ScreenSize.y.Half() - bulletComment.ContentRect.sizeDelta.y.Half() * scale.y;


            bulletComment.ContentRect.DOAnchorPos(new Vector2(targetPosX, targetPosY), 1.5F).SetAutoKill(true).SetEase(Ease.OutElastic);
            bulletComment.Panel.DOColor(new Color(1, 1f, 0, .8F), 1.5F).SetAutoKill(true).SetEase(Ease.OutElastic);

            bulletComment.ContentRect.DOScale(scale, 1.5F).OnComplete(() =>
            {
                SetBulletSpeed(bulletSpeed = rawSpeed);

                var inputField = bulletComment.BulletComment.InputText();

                inputField.onValueChanged.AddListener(content =>
                {
                    bulletComment.UpData(content, maxHeight / (rowCount - 1));
                    bulletComment.ContentRect.anchoredPosition = new Vector2(
                        -GameInsView.ScreenSize.x.Half() - bulletComment.ContentRect.sizeDelta.x.Half() * scale.x,
                        -GameInsView.ScreenSize.y.Half() - bulletComment.ContentRect.sizeDelta.y.Half() * scale.y);
                });

                inputField.onSubmit.AddListener(content => HideBullet(bulletComment, () =>
                {
                    OnBulletCommentSubmitEventHandler?.Invoke(content, Random.Range(sendDelaySecondRange.x, sendDelaySecondRange.y));
                    _selectObj = null;
                    Destroy(inputField);
                    SendBulletComment(content);
                }));
            });
        }


        private void HideBullet(BulletComments bulletComment, Action onHideComplete)
        {
            bulletComment.ContentRect.DOAnchorPos(new Vector2(0, bulletComment.ContentRect.anchoredPosition.y), 0.75F)
                .SetEase(Ease.OutQuart)
                .OnComplete(() =>
                {
                    _bulletCommentsShipsMap["默认"].Invoke(bulletComment);
                    onHideComplete.Invoke();
                    _pool.Release(bulletComment);
                    _isSelect = false;
                });
        }
    }
}