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

        
        public async UniTask DOMove(Vector3 targetPosition, float speed, Ease ease, TweenCallback onComplete = null)
        {
            _block.transform.DOKill();

            var tween = _block.transform.DOMove(targetPosition, speed)
                .SetEase(ease)
                .SetSpeedBased()
                .OnComplete(() =>
                {
                    _block.transform.DOPunchPosition(PUNCH_POSITION_OFFSET, PUNCH_DURATION, PUNCH_VIBRATO);
                });

            if (onComplete != null)
                tween.OnComplete(() => HandleTweenComplete(onComplete));

            await UniTask.WaitUntil(() => !tween.IsActive());
        }

        public async UniTask DOScale(Vector3 targetScale, float duration, Ease ease, TweenCallback onComplete = null)
        {
            _block.transform.DOKill();

            var tween = _block.transform.DOScale(targetScale, duration)
                .SetEase(ease);

            if (onComplete != null)
                tween.OnComplete(() => HandleTweenComplete(onComplete));

            await UniTask.WaitUntil(() => !tween.IsActive());
        }

        private void HandleTweenComplete(TweenCallback onComplete)
        {
            onComplete?.Invoke();
        }
    }
}