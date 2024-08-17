using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Xiyu.Desktop
{
    public class DesktopBehaviorUtility : MonoBehaviour
    {
        public UnityEvent<List<DesktopIcon>> SelectDesktopElements { get; set; } 
    }
}