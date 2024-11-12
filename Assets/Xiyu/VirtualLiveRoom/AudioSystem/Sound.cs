using Cysharp.Threading.Tasks;

namespace Xiyu.VirtualLiveRoom.AudioSystem
{
    public sealed class Sound : AudioOperator
    {
        public override string OperatorType { get; } = nameof(Sound).ToLower();


        public async UniTaskVoid SendPlay(string soundName)
        {
            await SetClip(soundName);
            base.SetVolume(0.5F).SetLoop(false).Play();
        }
    }
}