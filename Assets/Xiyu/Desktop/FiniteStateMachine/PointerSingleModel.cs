namespace Xiyu.Desktop.FiniteStateMachine
{
    public class PointerSingleModel : IPointerState
    {
        public PointerSingleModel(DesktopIconSelector selector)
        {
            _selector = selector;
        }


        private readonly DesktopIconSelector _selector;


        public PointerModel Model => PointerModel.Single;

        public void OnClickEnter(DesktopIcon desktopIcon)
        {
            _selector.ClearSelects();
            _selector.AddSelect(desktopIcon.DesktopMatrix, desktopIcon);
        }

        public void OnClick(DesktopIcon desktopIcon)
        {
        }

        public void OnClickExit(DesktopIcon desktopIcon)
        {
        }
    }
}