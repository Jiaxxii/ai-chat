using UnityEngine;

namespace Xiyu.Desktop
{
    public class GridInfo
    {
        public Vector2 Position { get; }

        public Vector2Int MatrixIndex { get; }


        public DesktopIcon DesktopIcon { get; set; }

        public GridState GridState { get; set; }

        public GridInfo(Vector2 position, Vector2Int matrixIndex)
        {
            Position = position;
            MatrixIndex = matrixIndex;
            GridState = GridState.Null;
        }

        public GridInfo(Vector2 position, Vector2Int matrixIndex, DesktopIcon desktopIcon)
        {
            Position = position;
            MatrixIndex = matrixIndex;
            DesktopIcon = desktopIcon;
            GridState = GridState.Exist;
        }
    }

    public enum GridState
    {
        /// <summary>
        /// 未使用
        /// </summary>
        Null,

        /// <summary>
        /// 存在
        /// </summary>
        Exist
    }
}