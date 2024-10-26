using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.GameFunction.CharacterComponent;
using Xiyu.Settings;

namespace Xiyu.VirtualLiveRoom.Component.DanmuItem
{
    public class Danmu : MonoBehaviour
    {
        [SerializeField] private DanmuHead headComponent;
        public DanmuHead Head => headComponent;

        [SerializeField] private DanmuUserName userNameComponent;
        public DanmuUserName UserName => userNameComponent;

        [SerializeField] private DanmuContent contentComponent;
        public DanmuContent Content => contentComponent;


        [SerializeField] private Image basePanel;

        public Color PanelColor
        {
            get => basePanel.color;
            set => basePanel.color = value;
        }


        public static async UniTask<Danmu> CreateAsync(Transform root, Sprite headSprite, string userName, string content)
        {
            var asset = (WebViewContentReferenceDeviceSo)await Resources.LoadAsync<WebViewContentReferenceDeviceSo>("Settings/RefPrefabricate");

            var danmu = await asset.LoadComponentAssetAsync<Danmu>("弹幕项", root);

            // *设置背景颜色*
            danmu.PanelColor = new Color(.9F, .9F, .9F);
            danmu.Head.PanelColor = new Color(.9F, .9F, .9F);
            // *设置头像(未做位置处理)*
            danmu.Head.SetHeadSprite(headSprite, Vector2.zero, new Vector2(50, 50));
            // *设置默认名称样式*
            danmu.UserName.Name = userName;
            danmu.UserName.FontColor = Color.white;
            danmu.UserName.PanelColor = Color.clear;
            // *设置内容默认样式*
            danmu.Content.Content = content;
            danmu.Content.PanelColor = Color.clear;
            //

            return danmu;
        }
    }
}