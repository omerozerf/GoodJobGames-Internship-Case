using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Blocks
{
    public class BlockVisual : MonoBehaviour
    {
        [SerializeField] private Block _block;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite[] _spriteArray;

        public event Action<int> OnSpriteChanged;


        private void SetSprite(int index)
        {
            if (_spriteArray != null && index >= 0 && index < _spriteArray.Length)
            {
                _spriteRenderer.sprite = _spriteArray[index];
                OnSpriteChanged?.Invoke(index);
            }
            else
            {
                Debug.LogError("Invalid sprite index: " + index);
            }
        }


        public Block GetBlock()
        {
            return _block;
        }

        public SpriteRenderer GetSpriteRenderer()
        {
            return _spriteRenderer;
        }

        public void SetOrderInLayer(int order)
        {
            _spriteRenderer.sortingOrder = order;
        }

        public void UpdateSpriteBasedOnGroupSize(int groupSize)
        {
            switch (groupSize)
            {
                case > 4 and < 8:
                {
                    SetSprite(1); // First icon (index 1)
                    break;
                }
                case > 7 and <= 10:
                {
                    SetSprite(2); // Second icon (index 2)
                    break;
                }
                case > 10:
                {
                    SetSprite(3); // Third icon (index 3)
                    break;
                }
                default:
                {
                    SetSprite(0); // Default icon (index 0)
                    break;
                }
            }
        }
    }
}