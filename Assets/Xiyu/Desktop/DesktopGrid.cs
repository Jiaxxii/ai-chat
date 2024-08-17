using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Xiyu.Constant;
using Xiyu.LoggerSystem;
using Xiyu.Settings;

namespace Xiyu.Desktop
{
    [Flags]
    public enum SelectMode
    {
        Single = 1,

        Shift = 2,
        Ctrl = 4,
        Multiple = Shift | Ctrl
    }

    public class DesktopGrid : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private Color selectIConColor;
        [SerializeField] private Color highlightIConColor;


        [SerializeField] private UnityEvent<DesktopIcon> iconSelectEventHandler;
        public UnityEvent<DesktopIcon> IconSelect => iconSelectEventHandler;

        [SerializeField] private SelectMode selectMode;

        public SelectMode SelectMode
        {
            get => selectMode;
            set => selectMode = value;
        }

        private GridInfo _lastSelectGridInfo;

        private void Awake()
        {
            InitGrids(10, 10, 80);
        }


        private static readonly Dictionary<Vector2Int, GridInfo> MatrixIndexToGrid = new();

        public float TaskMenuHeightSpacing { get; private set; }
        public float HorizontalSpacing { get; private set; }
        public float VerticalSpacing { get; private set; }

        public int HorizontalIConCount => Mathf.FloorToInt(GameConstant.DesktopSize.x / (DesktopIcon.ContentSize.x + HorizontalSpacing));
        public int VerticalIConCount => Mathf.FloorToInt((GameConstant.DesktopSize.y - TaskMenuHeightSpacing) / (DesktopIcon.ContentSize.y + VerticalSpacing));


        // 水平 垂直
        public void InitGrids(float horizontalSpacing, float verticalSpacing, float taskMenuHeightSpacing)
        {
            TaskMenuHeightSpacing = taskMenuHeightSpacing;
            HorizontalSpacing = horizontalSpacing;
            VerticalSpacing = verticalSpacing;


            for (var x = 0; x < HorizontalIConCount; x++)
            {
                for (var y = 0; y < VerticalIConCount; y++)
                {
                    var gridPosition = new Vector2((DesktopIcon.ContentSize.x + HorizontalSpacing) * x, (DesktopIcon.ContentSize.y + VerticalSpacing) * -y);
                    MatrixIndexToGrid.Add(new Vector2Int(x, y), new GridInfo(gridPosition, new Vector2Int(x, y)));
                }
            }

            LoadDefaultDesktopICons();
        }

        public bool TryAbsPositionToMatrixIndex(Vector2 position, out GridInfo gridInfo)
        {
            var x = Mathf.FloorToInt(position.x / (DesktopIcon.ContentSize.x + VerticalSpacing));
            var y = Mathf.FloorToInt(Mathf.Abs(position.y) / (DesktopIcon.ContentSize.y + VerticalSpacing));

            return MatrixIndexToGrid.TryGetValue(new Vector2Int(x, y), out gridInfo);
        }

        private void LoadDefaultDesktopICons()
        {
            var res = Resources.Load<DesktopIconCollectorScriptableObject>("Settings/New DesktopIconCollector");

            var logger = new FileLogger
            {
                Name = "桌面记录器"
            };

            foreach (var iconName in GameConstant.DefaultDesktopIcon)
            {
                if (res.Table.TryGetValue(iconName, out var iconInfo))
                {
                    AddAppICon(iconInfo);
                }
                else
                {
                    logger.LogError($"\"{iconName}\"不是一个桌面图标的有效名称");
                }
            }
        }

        public DesktopIcon AddAppICon(Icon iconInfo) => AddAppICon(iconInfo.IconSprite, iconInfo.IconName);

        public DesktopIcon AddAppICon(Sprite icon, string appName)
        {
            var gridInfo = GetFreeLocation();

            if (gridInfo == null)
            {
                new FileLogger().LogWarning("桌面空间不足!");
                return null;
            }

            var desktopICon = DesktopIcon.Create(content, icon, appName);

            desktopICon.OnPointerEnterEvent += OnPointerEnter;
            desktopICon.OnPointerExitEvent += OnPointerExit;
            desktopICon.OnPointerClickEvent += OnPointerClick;


            ((RectTransform)desktopICon.transform).anchoredPosition = gridInfo.Position;

            var info = MatrixIndexToGrid[gridInfo.MatrixIndex];
            info.GridState = GridState.Exist;
            info.DesktopIcon = desktopICon;

            return desktopICon;
        }


        public static GridInfo GetFreeLocation()
        {
            return MatrixIndexToGrid.Where(fridIndo => fridIndo.Value.GridState != GridState.Exist)
                .Select(fridInfo => fridInfo.Value)
                .FirstOrDefault();
        }

        private void OnPointerEnter(DesktopIcon desktopIcon, PointerEventData eventData)
        {
            desktopIcon.SetHighlight(highlightIConColor);
        }

        private void OnPointerExit(DesktopIcon desktopIcon, PointerEventData eventData)
        {
            if (desktopIcon.IsSelect)
            {
                desktopIcon.SetHighlight(selectIConColor);
                return;
            }

            desktopIcon.SetHighlight(Color.clear);
        }

        private void OnPointerClick(DesktopIcon desktopIcon, PointerEventData eventData)
        {
            // TODO 可能在未来改成有限状态机

            var fileLogger = new FileLogger { Name = "桌面" };

            if (SelectMode == SelectMode.Single)
            {
                // 如果是单选就一定只有一个图标被选择
                foreach (var icon in MatrixIndexToGrid.Where(gi => gi.Value.GridState == GridState.Exist))
                {
                    icon.Value.DesktopIcon.IsSelect = false;
                    icon.Value.DesktopIcon.SelectColor = Color.clear;
                }

                desktopIcon.SetHighlight(highlightIConColor);
                desktopIcon.IsSelect = true;
                TryAbsPositionToMatrixIndex(((RectTransform)desktopIcon.transform).anchoredPosition, out _lastSelectGridInfo);
            }

            if (SelectMode == SelectMode.Shift)
            {
                var anchoredPosition = ((RectTransform)desktopIcon.transform).anchoredPosition;

                if (!TryAbsPositionToMatrixIndex(anchoredPosition, out var firstSelectObject))
                {
                    // TODO
                    fileLogger.LogWarning($"无法将坐标({anchoredPosition.x}, {anchoredPosition.y})转换为矩阵索引!");
                    firstSelectObject = MatrixIndexToGrid.First().Value;
                }

                if (_lastSelectGridInfo == null)
                {
                    (_lastSelectGridInfo = firstSelectObject).DesktopIcon.SetHighlight(selectIConColor);
                    _lastSelectGridInfo.DesktopIcon.IsSelect = true;
                    return;
                }

                if (firstSelectObject != null)
                {
                    var xBegin = Mathf.Min(firstSelectObject.MatrixIndex.x, _lastSelectGridInfo.MatrixIndex.x);
                    var yBegin = Mathf.Min(firstSelectObject!.MatrixIndex.y, _lastSelectGridInfo.MatrixIndex.y);

                    var xEnd = Mathf.Max(firstSelectObject.MatrixIndex.x, _lastSelectGridInfo.MatrixIndex.x);
                    var yEnd = Mathf.Max(firstSelectObject!.MatrixIndex.y, _lastSelectGridInfo.MatrixIndex.y);

                    foreach (var gi in MatrixIndexToGrid.Where(v => v.Value.GridState == GridState.Exist))
                    {
                        var current = gi.Key;
                        // 在矩阵中
                        if (current.x >= xBegin && current.y >= yBegin && current.x <= xEnd && current.y <= yEnd)
                        {
                            gi.Value.DesktopIcon.SetHighlight(selectIConColor);
                            gi.Value.DesktopIcon.IsSelect = true;
                        }
                        else // 不在矩阵中
                        {
                            gi.Value.DesktopIcon.SelectColor = Color.clear;
                            gi.Value.DesktopIcon.IsSelect = false;
                        }
                    }
                }

                desktopIcon.SetHighlight(highlightIConColor);
                desktopIcon.IsSelect = true;
                TryAbsPositionToMatrixIndex(((RectTransform)desktopIcon.transform).anchoredPosition, out _lastSelectGridInfo);
                return;
            }

            if (SelectMode == SelectMode.Ctrl)
            {
                // 选择的图标状态反转
                var anchoredPosition = ((RectTransform)desktopIcon.transform).anchoredPosition;
                if (!TryAbsPositionToMatrixIndex(anchoredPosition, out _lastSelectGridInfo))
                {
                    // TODO
                    fileLogger.LogWarning($"无法将坐标({anchoredPosition.x}, {anchoredPosition.y})转换为矩阵索引!");
                    _lastSelectGridInfo = MatrixIndexToGrid.First().Value;
                }

                if (_lastSelectGridInfo.DesktopIcon.IsSelect)
                {
                    _lastSelectGridInfo.DesktopIcon.SetHighlight(Color.clear);
                    _lastSelectGridInfo.DesktopIcon.IsSelect = false;
                }
                else
                {
                    _lastSelectGridInfo.DesktopIcon.SetHighlight(selectIConColor);
                    _lastSelectGridInfo.DesktopIcon.IsSelect = true;
                }
            }

            Debug.LogWarning(_lastSelectGridInfo.MatrixIndex);
        }
    }
}