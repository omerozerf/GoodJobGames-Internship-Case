using System.Collections.Generic;
using System.Linq;
using Blocks;
using UnityEngine;

namespace Managers
{
    public class BlockCreateManager : MonoBehaviour
    {
        [SerializeField] private Board _board;
        [SerializeField] private Block[] _blockPrefabArray;
        [SerializeField] private Transform _blockPoolTransform;
        [SerializeField] private int _poolSizeBuffer;

        private Dictionary<BlockColor, ObjectPool<Block>> m_BlockPools;
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
            m_BlockPools = new Dictionary<BlockColor, ObjectPool<Block>>();
            var poolSize = (rows * columns / GetColorInGame()) + _poolSizeBuffer;

            for (var index = 0; index < GetColorInGame(); index++)
            {
                var blockPrefab = _blockPrefabArray[index];
                m_BlockPools[blockPrefab.GetColor()] = new ObjectPool<Block>(
                    blockPrefab,
                    _blockPoolTransform,
                    poolSize
                );
            }
        }

        private Block GetBlockFromPool(Block prefab)
        {
            var block = m_BlockPools[prefab.GetColor()].Get();
            block.transform.localScale = Vector3.one;
            return block;
        }

        private int GetColorInGame()
        {
            return _board.GetColorsInGame();
        }

        
        public Block CreateRandomBlock(int column, Cell[,] cellArray)
        {
            var keys = m_BlockPools.Keys.ToList();
            var randomKey = keys[Random.Range(0, keys.Count)];

            var block = GetBlockFromPool(_blockPrefabArray.First(blockPrefab => blockPrefab.GetColor() == randomKey));

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