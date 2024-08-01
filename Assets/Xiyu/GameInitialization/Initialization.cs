using UnityEngine;

namespace Xiyu.GameInitialization
{
    public static class Initialization
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Init()
        {
        }
        
        
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.)]
        // public static void Init()
        // {
        //     
        // }
        
    }
}