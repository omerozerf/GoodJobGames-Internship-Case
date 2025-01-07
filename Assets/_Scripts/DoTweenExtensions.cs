using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public static class DoTweenExtensions
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