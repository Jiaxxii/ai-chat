namespace Xiyu.Desktop.FiniteStateMachine
{
    public class PointerShiftModel : IPointerState
    {
        private readonly DesktopIconSelector _selector;

        public PointerShiftModel(DesktopIconSelector selector)
        {
            _selector = selector;
        }

        public PointerModel Model => PointerModel.Shift;

        public void OnClickEnter(DesktopIcon desktopIcon)
        {
//             var selects = _selector.Selects(_selector.Last.DesktopMatrix, desktopIcon.DesktopMatrix);
//
//             _selector.SelectItems.Clear(
// );            _selector.SelectItems.AddRange(selects);
        }

        public void OnClick(DesktopIcon desktopIcon)
        {
        }

        public void OnClickExit(DesktopIcon desktopIcon)
        {
        }
    }
}