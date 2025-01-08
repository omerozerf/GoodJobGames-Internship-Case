using System;
using DG.Tweening;
using Others;
using UnityEngine;

namespace Blocks
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private BlockColor _color;
        [SerializeField] private BlockCollision _collision;
        [SerializeField] private BlockVisual _visual;

        private Cell m_Cell;


        private void Awake()
        {
            name = _color.ToString();
        }

        private void OnDisable()
        {
            DOTween.KillAll();
        }


        public void SetPosition(int row, int column)
        {
            transform.position = new Vector3(column, row);
        }

        public void SetCell(Cell cell)
        {
            m_Cell = cell;
        }

        public Cell GetCell()
        {
            return m_Cell;
        }

        public void ResetPosition()
        {
            transform.position = Vector3.zero;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

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
