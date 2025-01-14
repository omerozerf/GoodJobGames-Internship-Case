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
                    switch (groupSize)
                    {
                        case < 5:
                            SetSprite(0);
                            break;
                        case <= 7:
                            SetSprite(1);
                            break;
                        case <= 9:
                            SetSprite(2);
                            break;
                        default:
                            SetSprite(3);
                            break;
                    }
                    break;

                case 5:
                case 4:
                    switch (groupSize)
                    {
                        case < 5:
                            SetSprite(0);
                            break;
                        case <= 6:
                            SetSprite(1);
                            break;
                        case <= 8:
                            SetSprite(2);
                            break;
                        default:
                            SetSprite(3);
                            break;
                    }
                    break;

                case 3:
                    switch (groupSize)
                    {
                        case < 5:
                            SetSprite(0);
                            break;
                        case 5:
                            SetSprite(1);
                            break;
                        case <= 7:
                            SetSprite(2);
                            break;
                        default:
                            SetSprite(3);
                            break;
                    }
                    break;

                case 2:
                    switch (groupSize)
                    {
                        case < 5:
                            SetSprite(0);
                            break;
                        case 5:
                            SetSprite(1);
                            break;
                        case 6:
                            SetSprite(2);
                            break;
                        default:
                            SetSprite(3);
                            break;
                    }
                    break;

                case 1:
                    switch (groupSize)
                    {
                        case < 5:
                            SetSprite(0);
                            break;
                        case 5:
                            SetSprite(1);
                            break;
                        default:
                            SetSprite(3);
                            break;
                    }
                    break;

                default:
                    SetSprite(0);
                    break;
            }
        }
    }
}