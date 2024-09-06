using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xiyu.Constant;
using Xiyu.LoggerSystem;
using Logger = Xiyu.LoggerSystem.Logger;

namespace Xiyu.Desktop
{
    public class DesktopIconOrganizer
    {
        public DesktopIconOrganizer(float horizontalSpace, float verticalSpace, float taskbarHeight)
        {
            HorizontalSpace = horizontalSpace;
            VerticalSpace = verticalSpace;
            TaskbarHeight = taskbarHeight;
        }

        public float DesktopWidth => GameConstant.DesktopSize.x;
        public float DesktopHeight => GameConstant.DesktopSize.y;

        public float IconWidth => DesktopIcon.ContentSize.x;
        public float IconHeight => DesktopIcon.ContentSize.y;

        private readonly Logger _logger;


        // horizontal
        public float HorizontalSpace { get; private set; }

        // vertical
        public float VerticalSpace { get; private set; }

        public float TaskbarHeight { get; private set; }

        public int MaxDesktopHorIcons => Mathf.FloorToInt(DesktopWidth / (IconWidth + HorizontalSpace));
        public int MaxDesktopVerIcons => Mathf.FloorToInt((DesktopHeight - TaskbarHeight) / (IconHeight + VerticalSpace));


        private readonly Dictionary<Vector2Int, DesktopIcon> _desktopIconMatrix = new();

        private Dictionary<string, DesktopIcon> _nonStandardDesktopIconMatrix;

        private readonly HashSet<string> _containNameHashSet = new();

        public bool Select(Vector2Int matrix, out DesktopIcon desktopIcon)
        {
            if (_desktopIconMatrix.TryGetValue(matrix, out var first))
            {
                desktopIcon = first;
                return true;
            }


            desktopIcon = null;
            return false;
        }

        public bool Find(string desktopIconName, out DesktopIcon desktopIcon)
        {
            if (_nonStandardDesktopIconMatrix.TryGetValue(desktopIconName, out var first))
            {
                desktopIcon = first;
                return true;
            }

            desktopIcon = null;
            return false;
        }

        public void AddDesktopIcon(DesktopIcon desktopIcon)
        {
            // 防止文件出现重名
            if (_containNameHashSet.Add(desktopIcon.AppName) == false)
            {
                Rename(desktopIcon);
            }

            if (CheckingMatrix(desktopIcon.DesktopMatrix) && _desktopIconMatrix.TryAdd(desktopIcon.DesktopMatrix, desktopIcon))
            {
                // 这里一定会转换成功
                TryMatrixToAnchoredPosition(desktopIcon.DesktopMatrix, out var anchoredPosition);
                desktopIcon.SetAnchoredPosition(anchoredPosition);
                return;
            }
            // 这个矩阵存在桌面图标时

            var newMatrix = GetEmptyGrid(desktopIcon.DesktopMatrix);

            if (newMatrix.HasValue)
            {
                desktopIcon.DesktopMatrix = newMatrix.Value;
                _desktopIconMatrix.Add(newMatrix.Value, desktopIcon);

                TryMatrixToAnchoredPosition(desktopIcon.DesktopMatrix, out var anchoredPosition);
                desktopIcon.SetAnchoredPosition(anchoredPosition);
            }
            else
            {
                // 表示桌面已经满了
                new FileLogger
                {
                    Name = nameof(DesktopIconOrganizer)
                }.LogError("桌面没有空余空间！");

                AddNonstandardDesktopIconMatrix(desktopIcon);
                // TODO DEBUG
            }
        }

        public DesktopIcon CreateAndAddDesktopIcon(Transform parent, Sprite iconSprite, string iconName)
        {
            var desktopIcon = DesktopIcon.Create(parent, iconSprite, iconName);

            // 防止文件出现重名
            if (_containNameHashSet.Add(desktopIcon.AppName) == false)
            {
                Rename(desktopIcon);
            }

            var newMatrix = GetEmptyGrid(desktopIcon.DesktopMatrix);

            if (newMatrix.HasValue)
            {
                desktopIcon.DesktopMatrix = newMatrix.Value;
                _desktopIconMatrix.Add(newMatrix.Value, desktopIcon);

                TryMatrixToAnchoredPosition(desktopIcon.DesktopMatrix, out var anchoredPosition);
                desktopIcon.SetAnchoredPosition(anchoredPosition);
            }
            else
            {
                // 表示桌面已经满了
                new FileLogger
                {
                    Name = nameof(DesktopIconOrganizer)
                }.LogError("桌面没有空余空间！");

                AddNonstandardDesktopIconMatrix(desktopIcon);
                // 取消显示
                desktopIcon.SetActive(false);
            }

            return desktopIcon;
        }

        public bool CheckingMatrix(Vector2Int matrix)
        {
            return matrix.x >= 0 && matrix.x <= MaxDesktopHorIcons &&
                   matrix.y >= 0 && matrix.y <= MaxDesktopVerIcons;
        }

        public bool RecoveryDesktopIcon(DesktopIcon desktopIcon)
        {
            if (!_containNameHashSet.Contains(desktopIcon.AppName))
            {
                _logger.LogError($"\"{desktopIcon.AppName}\"桌面图标的名称可能被修改");
                return false;
            }

            // 判断桌面图标的矩阵是否包含
            if (_desktopIconMatrix.ContainsKey(desktopIcon.DesktopMatrix))
            {
                _desktopIconMatrix.Remove(desktopIcon.DesktopMatrix);
                _containNameHashSet.Remove(desktopIcon.AppName);

                return true;
            }

            // 如果不包含就判断桌面图标是不是一个非标准的桌面图标（桌面图标满时）
            if (_nonStandardDesktopIconMatrix.ContainsKey(desktopIcon.AppName))
            {
                _nonStandardDesktopIconMatrix.Remove(desktopIcon.AppName);
                _containNameHashSet.Remove(desktopIcon.AppName);

                return true;
            }

            _logger.LogError(
                $"桌面图标\"{desktopIcon.AppName}\"的矩阵[{desktopIcon.DesktopMatrix.x},{desktopIcon.DesktopMatrix.y}]不存在，它可能被非预期的修改！");
            return false;
        }

        public bool RecoveryDesktopIcon(Vector2Int matrix)
        {
            return !_desktopIconMatrix.TryGetValue(matrix, out var desktopIcon) || RecoveryDesktopIcon(desktopIcon);
        }


        private void AddNonstandardDesktopIconMatrix(DesktopIcon desktopIcon)
        {
            // 防止文件出现重名
            if (_containNameHashSet.Add(desktopIcon.AppName) == false)
            {
                Rename(desktopIcon);
            }

            (_nonStandardDesktopIconMatrix ??= new Dictionary<string, DesktopIcon>())
                .Add(desktopIcon.AppName, desktopIcon);
        }

        private Vector2Int? GetEmptyGrid()
        {
            for (var x = 0; x < MaxDesktopHorIcons; x++)
            for (var y = 0; y < MaxDesktopVerIcons; y++)
            {
                var matrix = new Vector2Int(x, y);
                if (!_desktopIconMatrix.ContainsKey(matrix))
                {
                    return matrix;
                }
            }


            return null;
        }

        private Vector2Int? GetEmptyGrid(Vector2Int start)
        {
            if (start.x < 0 || start.y > MaxDesktopHorIcons)
            {
                start.x = MaxDesktopVerIcons;
            }

            if (start.y < 0 || start.y > MaxDesktopVerIcons)
            {
                start.y = MaxDesktopVerIcons;
            }


            for (var x = start.x; x < MaxDesktopHorIcons; x++)
            for (var y = start.y; y < MaxDesktopVerIcons; y++)
            {
                var matrix = new Vector2Int(x, y);
                if (!_desktopIconMatrix.ContainsKey(matrix))
                {
                    return matrix;
                }
            }

            for (var x = start.x - 1; x >= 0; x--)
            for (var y = start.y - 1; y >= 0; y++)
            {
                var matrix = new Vector2Int(x, y);
                if (!_desktopIconMatrix.ContainsKey(matrix))
                {
                    return matrix;
                }
            }


            return null;
        }


        private bool TryMatrixToAnchoredPosition(Vector2Int matrix, out Vector2 anchoredPosition)
        {
            if (!_desktopIconMatrix.ContainsKey(matrix))
            {
                anchoredPosition = Vector2.negativeInfinity;
                return false;
            }

            anchoredPosition = new Vector2(matrix.x * (IconWidth + HorizontalSpace), -matrix.y * (IconWidth + VerticalSpace));
            return true;
        }


        // [CanBeNull]
        // private DesktopIcon RepetitiveItem() => _desktopIconMatrix.Values.FirstOrDefault(v => _containNameHashSet.Add(v.AppName));


        private void Rename(DesktopIcon desktopIcon)
        {
            var index = 1;
            string reName;

            do
            {
                reName = $"{desktopIcon.AppName} ({index++})";
            } while (_containNameHashSet.Add(reName) == false);


            desktopIcon.AppName = reName;
        }

        public IEnumerable<KeyValuePair<Vector2Int, DesktopIcon>> GetActiveDesktopIcons() => _desktopIconMatrix;
        // public IEnumerable<DesktopIcon> GetNotActiveDesktopIcons() => _nonStandardDesktopIconMatrix.Select(item => item.Value);
        //
        // public IEnumerable<DesktopIcon> GetAllDesktopIcons()
        // {
        //     foreach (var item in _desktopIconMatrix)
        //     {
        //         yield return item.Value;
        //     }
        //
        //     foreach (var item in _nonStandardDesktopIconMatrix)
        //     {
        //         yield return item.Value;
        //     }
        // }
    }
}