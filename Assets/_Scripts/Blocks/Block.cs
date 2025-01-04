using UnityEngine;

namespace Blocks
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private BlockColor _color;
        [SerializeField] private BlockCollision _collision;
        [SerializeField] private BlockVisual _visual;


        public BlockColor GetColor()
        {
            return _color;
        }

        public BlockCollision GetCollision()
        {
            return _collision;
        }

        public BlockVisual GetVisual()
        {
            return _visual;
        }
    }
}
