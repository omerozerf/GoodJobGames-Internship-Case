using UnityEngine;

namespace Blocks
{
    public class BlockCollision : MonoBehaviour
    {
        [SerializeField] private Block _block;


        public Block GetBlock()
        {
            return _block;
        }
    }
}
