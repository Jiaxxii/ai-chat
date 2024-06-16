using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xiyu.Settings
{
    [CreateAssetMenu(fileName = "New PreformData", menuName = "ScriptableObject/PreformData")]
    public class PreformScriptableObject : ScriptableObject
    {
        [Serializable]
        public class Pair
        {
            [SerializeField] private string name;
            [SerializeField] private GameObject preform;

            public string Name => string.IsNullOrEmpty(name) ? preform.name : name;

            public GameObject Preform => preform;

            public T GetComponent<T>() => preform.GetComponent<T>();
        }

        [SerializeField] private List<Pair> preforms;

        private Dictionary<string, Pair> _table;
        public Dictionary<string, Pair> Table
        {
            get
            {
                if (_table != null) return _table;
                _table = new Dictionary<string, Pair>();
                foreach (var pair in preforms.Where(pair => !_table.TryAdd(pair.Name, pair)))
                {
                    Debug.LogWarning($"重复的key\"{pair.Name}\"");
                }

                return _table;
            }
        }
    }
}