using UnityEngine;
using UnityEngine.EventSystems;

namespace Xiyu.VirtualLiveRoom.AudioSystem
{
    public class UIContentEnterInAudioSound : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private string label;
        [SerializeField] private string audioName;


        public void OnPointerEnter(PointerEventData eventData)
        {
            var operatorPlayer = AudioManager.Instance.GetAudioOperatorPlayer(label);
            ((Sound)operatorPlayer).SendPlay(audioName).Forget();
        }
    }
}