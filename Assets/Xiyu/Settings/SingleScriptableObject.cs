using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace Xiyu.Settings
{
    public class SingleScriptableObject<T> : ScriptableObject where T : ScriptableObject

    {
    public virtual string Folder => "Settings";

    public virtual string Name => $"New {nameof(T)}";


    

    }
}