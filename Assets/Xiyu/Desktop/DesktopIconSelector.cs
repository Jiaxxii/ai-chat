using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xiyu.Desktop.FiniteStateMachine;

namespace Xiyu.Desktop
{
    public class DesktopIconSelector
    {
        private readonly DesktopIconOrganizer _iconOrganizer;

        private readonly StateMachine _machine = new();

        // public List<DesktopIcon> SelectItems { get; } = new();

        // public DesktopIcon Last => SelectItems.Count == 0 ? null : SelectItems[^1];

        private readonly Dictionary<Vector2Int, DesktopIcon> _selectMap = new();


        public DesktopIconSelector(DesktopIconOrganizer iconOrganizer, IEnumerable<IPointerState> pointerStateHandlers)
        {
            foreach (var stateHandlers in pointerStateHandlers)
            {
                _machine.Add(stateHandlers);
            }

            _iconOrganizer = iconOrganizer;
        }


        public bool Select(Vector2Int matrix, out DesktopIcon desktopIcon)
        {
            return _iconOrganizer.Select(matrix, out desktopIcon);
        }

        public bool AddSelect(Vector2Int matrix, DesktopIcon desktopIcon)
        {
            return _selectMap.TryAdd(matrix, desktopIcon);
        }

        public bool SelectsQueueContains(Vector2Int matrix)
        {
            return _selectMap.ContainsKey(matrix);
        }

        public void ClearSelects()
        {
            _selectMap.Clear();
        }

        //
        // public bool Select(IEnumerable<Vector2Int> matrix)
        // {
        //     SelectItems.Clear();
        //     foreach (var mat in matrix)
        //     {
        //         if (_iconOrganizer.Select(mat, out var desktopIcon))
        //         {
        //             SelectItems.Add(desktopIcon);
        //         }
        //     }
        // 
        //     return SelectItems.Count < 0;
        // }
        //
        //
        public IEnumerable<DesktopIcon> Selects(Vector2Int matrixBegin, Vector2Int matrixEnd)
        {
            var list = new List<DesktopIcon>();

            // 确保结束坐标不小于开始坐标  
            var minX = Mathf.Min(matrixBegin.x, matrixEnd.x);
            var minY = Mathf.Min(matrixBegin.y, matrixEnd.y);

            var maxX = Mathf.Max(matrixBegin.x, matrixEnd.x);
            var maxY = Mathf.Max(matrixBegin.y, matrixEnd.y);

            for (var y = minY; y <= maxY; y++)
            {
                var lineEnd = Mathf.Min(maxX + 1, _iconOrganizer.MaxDesktopVerIcons); // +1 是为了避免包含超出范围的索引  
                for (var x = minX; x < lineEnd; x++)
                {
                    if (_iconOrganizer.Select(new Vector2Int(x, y), out var desktopIcon))
                    {
                        list.Add(desktopIcon);
                    }
                }
            }

            return list;
        }
    }
}


// (0,0) (1,0) (2,0) (3,0) (4,0) (5,0)
// (0,1) (1,1) (2,1) (3,1) (4,1) (5,1)
// (0,2) (1,2) (2,2) (3,2) (4,2) (5,2)
// (0,3) (1,3) (2,3) (3,3) (4,3) (5,3)
// (0,4) (1,4) (2,4) (3,4) (4,4) (5,4)
// (0,5) (1,5) (2,5) (3,5) (4,5) (5,5)