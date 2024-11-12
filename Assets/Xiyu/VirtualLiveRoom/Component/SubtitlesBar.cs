using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.Expand;
using Xiyu.GameFunction.GeometricTransformations;
using Xiyu.LoggerSystem;
using Xiyu.VirtualLiveRoom.EventFunctionSystem;
using Xiyu.VirtualLiveRoom.View.LiveRoomView;
using Xiyu.VirtualLiveRoom.View.Text;

namespace Xiyu.VirtualLiveRoom.Component
{
    public class SubtitlesBar : UIContainer
    {
        [SerializeField] private Image basePanel;
        [SerializeField] private TextMeshProUGUI subtitles;

        private readonly HashSet<char> _endCharSet = new()
        {
            '.', '。', '?', '？', '!', '！', '~'
        };

        private Property<float> _alphaProperty;

        private void Awake()
        {
            _alphaProperty = new Property<float>(() => basePanel.color.a, alpha =>
            {
                basePanel.color = basePanel.color.SetAlpha(alpha);
                subtitles.alpha = alpha;
            });
            basePanel.gameObject.SetActive(false);
            subtitles.text = string.Empty;
        }

        [WebContentInit(ThenAfterInitialization = typeof(LiveRoom))]
        protected override async UniTask Initialization(CancellationToken cancellationToken = default)
        {
            transform.SetAsLastSibling();

            basePanel.gameObject.SetActive(true);
            await Print($"欢迎'{User.UserName}'来到直播间哦！", outDuration: 1);

            basePanel.gameObject.SetActive(false);
        }


        private async UniTask Print(string text, float inDuration = 0.75F, float outDuration = 1.75F, float nextCharTime = 0.075F)
        {
            subtitles.text = string.Empty;

            await Fade(0F, 1F, inDuration, Ease.InCubic);

            await subtitles.Print(text, nextCharTime, false);


            await UniTask.WaitForSeconds(text.Length / 10F);

            await Fade(1, 0, outDuration, Ease.OutCirc);
        }

        public async UniTask Print(string text, int newLineCount = 30)
        {
            if (text.StartsWith('\"') && text.EndsWith('\"') || text.StartsWith('“') && text.EndsWith('”'))
            {
                text = text.Substring(1, text.Length - 2);
            }

            // await LoggerManager.Instance.LogInfoAsync($"开始打印字幕：{text}");

            if (text.Length <= newLineCount)
            {
                await Print(text, inDuration: 0.1F);
                return;
            }

            var sb = new StringBuilder();
            var subLen = 0;
            for (var i = 0; i < text.Length; i++)
            {
                if (_endCharSet.Contains(text[i]))
                {
                    if (i + 1 >= text.Length || (text[i + 1] != '"' && text[i + 1] != '”'))
                    {
                        var start = 0;
                        do
                        {
                            if (text[i] != text[i + start]) break;
                            start++;
                        } while (i + start < text.Length);

                        if (start > 1)
                        {
                            // 有连续的。。。
                            await Print(sb.Append(text.Substring(i, start)).ToString(), 0.05F);
                            i += start;
                        }
                        else
                            await Print(sb.Append(text[i]).ToString(), 0.1F);

                        subLen = 0;
                        sb.Clear();
                        continue;
                    }
                }

                if (subLen == newLineCount)
                {
                    subLen = 0;
                    sb.Append(text[i]).Append('\n');
                }
                else if (i >= text.Length - 1)
                {
                    await Print(sb.Append(text[i]).ToString(), 0.1F);
                    break;
                }
                else
                {
                    subLen++;
                    sb.Append(text[i]);
                }
            }
        }

        public async UniTask Fade(float startAlpha, float endAlpha, float duration, Ease ease)
        {
            _alphaProperty.Member = startAlpha;
            // transform.SetAsLastSibling();
            basePanel.gameObject.SetActive(true);
            await DOTween.To(() => _alphaProperty.Member, v => _alphaProperty.Member = v, endAlpha, duration)
                .SetEase(ease)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
    }
}