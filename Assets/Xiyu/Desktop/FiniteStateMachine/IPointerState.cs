namespace Xiyu.Desktop.FiniteStateMachine
{
    public interface IPointerState
    {
        PointerModel Model { get; }

        void OnClickEnter
            (DesktopIcon desktopIcon);

        void OnClick
            (DesktopIcon desktopIcon);

        void OnClickExit
            (DesktopIcon desktopIcon);
    }
}