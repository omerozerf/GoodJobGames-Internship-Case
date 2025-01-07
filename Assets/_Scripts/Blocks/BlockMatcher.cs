using UnityEngine;

namespace Blocks
{
    public class BlockMatcher : MonoBehaviour
    {
        [SerializeField] private Block _block;

        public bool Match(Block other)
        {
            if (!_block || !other) return false;
            return _block.GetColor() == other.GetColor();
        }
    }
}