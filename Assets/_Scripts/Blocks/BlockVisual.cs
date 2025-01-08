using System;
using UnityEngine;

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

        public void UpdateSpriteBasedOnGroupSize(int groupSize, int colorInGame)
        {
            switch (colorInGame)
            {
                case 6:
                    if (groupSize < 5)
                        SetSprite(0);
                    else if (groupSize <= 7)
                        SetSprite(1);
                    else if (groupSize <= 9)
                        SetSprite(2);
                    else
                        SetSprite(3);
                    break;

                case 5:
                case 4:
                    if (groupSize < 5)
                        SetSprite(0);
                    else if (groupSize <= 6)
                        SetSprite(1);
                    else if (groupSize <= 8)
                        SetSprite(2);
                    else
                        SetSprite(3);
                    break;

                case 3:
                    if (groupSize < 5)
                        SetSprite(0);
                    else if (groupSize == 5)
                        SetSprite(1);
                    else if (groupSize <= 7)
                        SetSprite(2);
                    else
                        SetSprite(3);
                    break;

                case 2:
                    if (groupSize < 5)
                        SetSprite(0);
                    else if (groupSize == 5)
                        SetSprite(1);
                    else if (groupSize == 6)
                        SetSprite(2);
                    else
                        SetSprite(3);
                    break;

                case 1:
                    if (groupSize < 5)
                        SetSprite(0);
                    else if (groupSize == 5)
                        SetSprite(1);
                    else
                        SetSprite(3);
                    break;

                default:
                    SetSprite(0);
                    break;
            }
        }
    }
}