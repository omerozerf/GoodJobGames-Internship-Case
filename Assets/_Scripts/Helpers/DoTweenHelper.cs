using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Helpers
{
    public static class DoTweenHelper
    {
        public static UniTask ToUniTask(this Tween tween)
        {
            var completionSource = new UniTaskCompletionSource();

            tween.OnComplete(() => completionSource.TrySetResult());
            tween.OnKill(() => completionSource.TrySetCanceled());
            tween.OnRewind(() => completionSource.TrySetCanceled());

            return completionSource.Task;
        }
    }
}