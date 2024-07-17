using DG.Tweening;

namespace Xiyu.GameFunction.GameWindows
{
    public class DialogParametersDefault : IDialogParameters
    {
        public DialogParametersDefault(string title)
        {
            Title = title;
        }

        public string Title { get; }
        public (float duration, Ease ease)? ShowTweenParams { get; set; }
        public (float duration, Ease ease)? HideTweenParams { get; set; }
    }
    public interface IDialogParameters
    {
        string Title { get; }


        (float duration, Ease ease)? ShowTweenParams { get; set; }

        (float duration, Ease ease)? HideTweenParams { get; set; }
    }
}