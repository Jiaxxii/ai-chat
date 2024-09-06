namespace Xiyu.Desktop.FiniteStateMachine
{
    public class PointerCtrlModel : IPointerState
    {
        public PointerModel Model => PointerModel.Ctrl;

        private readonly DesktopIconSelector _selector;

        public PointerCtrlModel(DesktopIconSelector selector)
        {
            _selector = selector;
        }

        public void OnClickEnter(DesktopIcon desktopIcon)
        {
            // var last = _selector.Last;
            // if (last == null)
            // {
            //     _selector.SelectItems.Add(desktopIcon);
            // }
            // else
            // {
            //     if (_selector.Select(desktopIcon.DesktopMatrix, out var selectDesktopIcon))
            //     {
            //         if (selectDesktopIcon.DesktopMatrix == desktopIcon.DesktopMatrix)
            //         {
            //             _selector.SelectItems.RemoveAt(_selector.SelectItems.FindIndex( v => v.DesktopMatrix == desktopIcon.DesktopMatrix));
            //         }
            //         else
            //         {
            //             _selector.SelectItems.Add(desktopIcon);
            //         }
            //     }
            // }
        }

        public void OnClick(DesktopIcon desktopIcon)
        {
        }

        public void OnClickExit(DesktopIcon desktopIcon)
        {
        }
    }
}