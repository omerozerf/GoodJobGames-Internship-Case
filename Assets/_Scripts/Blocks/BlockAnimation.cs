using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Blocks
{
    public class BlockAnimation : MonoBehaviour
    {
        [SerializeField] private Block _block;

        
        public async UniTask DOMove(Vector3 targetPosition, float duration, Ease ease, TweenCallback onComplete = null)
        {
            _block.transform.DOKill();

            var tween = _block.transform.DOMove(targetPosition, duration)
                .SetEase(ease);

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