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
            transform.DOKill();

            await _block.transform.DOMove(targetPosition, duration)
                .SetEase(ease)
                .OnComplete(onComplete)
                .AsyncWaitForCompletion();
        }

        public async UniTask DOScale(Vector3 targetScale, float duration, Ease ease, TweenCallback onComplete = null)
        {
            transform.DOKill();

            await _block.transform.DOScale(targetScale, duration)
                .SetEase(ease)
                .OnComplete(onComplete)
                .AsyncWaitForCompletion();
        }
    }
}