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

        private Dictionary<Block, ObjectPool<Block>> m_BlockPools;
        private Dictionary<Block, int> m_BlockUsageCount;
        private List<Block> m_ActivePrefabs;
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
            m_BlockUsageCount = new Dictionary<Block, int>();

            var colorCount = Mathf.Min(GetColorInGame(), _blockPrefabArray.Length);
            m_ActivePrefabs = _blockPrefabArray.Take(colorCount).ToList();

            var poolSize = (rows * columns / m_ActivePrefabs.Count) + _poolSizeBuffer;

            foreach (var blockPrefab in m_ActivePrefabs)
            {
                m_BlockPools[blockPrefab] = new ObjectPool<Block>(
                    blockPrefab,
                    _blockPoolTransform,
                    poolSize
                );

                m_BlockUsageCount[blockPrefab] = 0;
            }
        }

        private Block GetBlockFromPool(Block prefab)
        {
            var block = m_BlockPools[prefab].Get();
            block.transform.localScale = Vector3.one;
            return block;
        }

        private int GetColorInGame()
        {
            return _board.GetColorsInGame();
        }

        public Block CreateRandomBlock(int column, Cell[,] cellArray)
        {
            var minUsage = m_BlockUsageCount.Values.Min();
            var leastUsedPrefabs = m_BlockUsageCount
                .Where(kvp => kvp.Value == minUsage)
                .Select(kvp => kvp.Key)
                .ToList();

            var randomPrefab = leastUsedPrefabs[UnityEngine.Random.Range(0, leastUsedPrefabs.Count)];

            var block = GetBlockFromPool(randomPrefab);
            m_BlockUsageCount[randomPrefab]++;

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
                m_BlockUsageCount[kvp.Key]--;
                return;
            }
        }
    }
}