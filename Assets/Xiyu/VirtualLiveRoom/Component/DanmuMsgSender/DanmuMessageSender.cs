using UnityEngine;
using UnityEngine.UI;

namespace Xiyu.VirtualLiveRoom.Component.DanmuMsgSender
{
    public class DanmuMessageSender : MonoBehaviour
    {
        [SerializeField] private MessageBoxContent messageBoxContentComponent;
        public MessageBoxContent BackGround => messageBoxContentComponent;


        [SerializeField] private MessageBoxInputField messageBoxInputField;
        public MessageBoxInputField MessageBox => messageBoxInputField;






    }
}