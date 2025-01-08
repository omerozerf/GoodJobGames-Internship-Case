using System.Collections.Generic;
using Blocks;
using UnityEngine;

namespace Managers
{
    public class BlockCreateManager : MonoBehaviour
    {
        [SerializeField] private Block[] _blockPrefabArray;
        [SerializeField] private Transform _blockPoolTransform;
        [SerializeField] private int _poolSizeBuffer;

        private Dictionary<Block, ObjectPool<Block>> m_BlockPools;
        private Camera m_MainCamera;


        private void Awake()
        {
            m_MainCamera = Camera.main;

            Board.OnInitializeBoard += HandleOnInitializeBoard;
        }

        private void OnDestroy()
        {
            Board.OnInitializeBoard -= HandleOnInitializeBoard;
        }


        private void HandleOnInitializeBoard(int rows, int columns)
        {
            InitializePools(rows, columns);
        }


        private void InitializePools(int rows, int columns)
        {
            m_BlockPools = new Dictionary<Block, ObjectPool<Block>>();
            var poolSize = (rows * columns / _blockPrefabArray.Length) + _poolSizeBuffer;

            foreach (var blockPrefab in _blockPrefabArray)
            {
                m_BlockPools[blockPrefab] = new ObjectPool<Block>(
                    blockPrefab,
                    _blockPoolTransform,
                    poolSize
                );
            }
        }

        private Block GetBlockFromPool(Block prefab)
        {
            var block = m_BlockPools[prefab].Get();
            block.transform.localScale = Vector3.one;
            return block;
        }


        public Block CreateRandomBlock(int column, Cell[,] cellArray)
        {
            var randomPrefab = _blockPrefabArray[UnityEngine.Random.Range(0, _blockPrefabArray.Length)];
            var block = GetBlockFromPool(randomPrefab);

            block.transform.position = new Vector3(
                cellArray[0, column].transform.position.x,
                m_MainCamera.transform.position.y + m_MainCamera.orthographicSize + 2.0f,
                0
            );

            return block;
        }

        public void ReturnBlockToPool(Block block)
        {
            foreach (var kvp in m_BlockPools)
            {
                if (kvp.Key.GetType() != block.GetType()) continue;

                kvp.Value.Return(block, _blockPoolTransform);
                return;
            }
        }
    }
}