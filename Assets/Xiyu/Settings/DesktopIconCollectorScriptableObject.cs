using System.Collections.Generic;
using UnityEngine;
using Xiyu.Desktop;

namespace Xiyu.Settings
{
    [CreateAssetMenu(fileName = "New DesktopIconCollector", menuName = "ScriptableObject/DesktopIconCollector", order = 0)]
    public class DesktopIconCollectorScriptableObject : ScriptableObject
    {
        [SerializeField] private List<Icon> desktopIconsCollector;


        private Dictionary<string, Icon> _table;

        public Dictionary<string, Icon> Table
        {
            get
            {
                if (_table != null) return _table;

                _table = new Dictionary<string, Icon>();
                foreach (var icon in desktopIconsCollector)
                {
                    _table.Add(icon.IconName, icon);
                }

                return _table;
            }
        }
    }
}