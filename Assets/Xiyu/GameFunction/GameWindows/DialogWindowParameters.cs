namespace Xiyu.GameFunction.GameWindows
{
    public class DialogWindowParameters : IDialogWindowParameters
    {
        public DialogWindowParameters(string title)
        {
            Title = title;
        }

        public string Title { get; }
        public (float duration, DG.Tweening.Ease ease)? ShowTweenParams { get; set; }
        public (float duration, DG.Tweening.Ease ease)? HideTweenParams { get; set; }
    }
}