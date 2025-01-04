using Blocks;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Blocks
{
    public class BlockVisual : MonoBehaviour
    {
        [SerializeField] private Block _block;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite[] _spriteArray;


        public Block GetBlock()
        {
            return _block;
        }

        public Image GetImage()
        {
            return _image;
        }
    }
}
