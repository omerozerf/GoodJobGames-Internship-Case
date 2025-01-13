using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Blocks
{
    public class BlockAnimation : MonoBehaviour
    {
        [SerializeField] private Block _block;

        private readonly Vector3 m_PunchPositionOffset = new Vector3(0, 0.15f, 0);
        
        private const float PUNCH_DURATION = 0.25f;
        private const int PUNCH_VIBRATO = 0;
        
        private Transform m_BlockTransform;

        
        private void Awake()
        {
            m_BlockTransform = _block.transform;
        }
        
        
        private void ApplyPunchEffect()
        {
            _block.transform.DOPunchPosition(m_PunchPositionOffset, PUNCH_DURATION, PUNCH_VIBRATO);
        }

        private void KillExistingTweens()
        {
            _block.transform.DOKill();
        }

        private async UniTask WaitForTweenCompletion(Tween tween)
        {
            await UniTask.WaitUntil(() => !tween.IsActive());
        }
        
        
        public async UniTask DoMove(Vector3 targetPosition, float speed, Ease ease)
        {
            KillExistingTweens();

            Tween moveTween = m_BlockTransform.DOMove(targetPosition, speed)
                .SetEase(ease)
                .SetSpeedBased()
                .OnComplete(ApplyPunchEffect);
            
            await WaitForTweenCompletion(moveTween);
        }

        public async UniTask DoScale(Vector3 targetScale, float duration, Ease ease, TweenCallback onComplete = null)
        {
            Tween scaleTween = _block.transform.DOScale(targetScale, duration)
                .SetEase(ease);
            
            await WaitForTweenCompletion(scaleTween);
        }
    }
}