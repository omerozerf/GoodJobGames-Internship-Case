using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Blocks
{
    public class BlockAnimation : MonoBehaviour
    {
        [SerializeField] private Block _block;

        private static readonly Vector3 PUNCH_POSITION_OFFSET = new Vector3(0, 0.15f, 0);
        private const float PUNCH_DURATION = 0.25f;
        private const int PUNCH_VIBRATO = 0;

        
        private void ApplyPunchEffect()
        {
            _block.transform.DOPunchPosition(PUNCH_POSITION_OFFSET, PUNCH_DURATION, PUNCH_VIBRATO);
        }

        private void KillExistingTweens()
        {
            _block.transform.DOKill();
        }

        private void AddCompletionCallback(Tween tween, TweenCallback onComplete)
        {
            if (onComplete != null)
            {
                tween.OnComplete(onComplete.Invoke);
            }
        }

        private async UniTask WaitForTweenCompletion(Tween tween)
        {
            await UniTask.WaitUntil(() => !tween.IsActive());
        }
        
        
        public async UniTask DoMove(Vector3 targetPosition, float speed, Ease ease, TweenCallback onComplete = null)
        {
            KillExistingTweens();

            Tween moveTween = _block.transform.DOMove(targetPosition, speed)
                .SetEase(ease)
                .SetSpeedBased()
                .OnComplete(ApplyPunchEffect);

            AddCompletionCallback(moveTween, onComplete);

            await WaitForTweenCompletion(moveTween);
        }

        public async UniTask DoScale(Vector3 targetScale, float duration, Ease ease, TweenCallback onComplete = null)
        {
            KillExistingTweens();

            Tween scaleTween = _block.transform.DOScale(targetScale, duration)
                .SetEase(ease);

            AddCompletionCallback(scaleTween, onComplete);

            await WaitForTweenCompletion(scaleTween);
        }
    }
}