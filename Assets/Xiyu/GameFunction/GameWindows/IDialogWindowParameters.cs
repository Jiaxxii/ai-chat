using DG.Tweening;

namespace Xiyu.GameFunction.GameWindows
{
    public interface IDialogWindowParameters
    {
        string Title { get; }


        (float duration, Ease ease)? ShowTweenParams { get; set; }

        (float duration, Ease ease)? HideTweenParams { get; set; }
    }
}