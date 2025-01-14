using System;
using System.Collections.Generic;
using System.Linq;
using Blocks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Helpers;
using Others;
using UnityEngine;

namespace Managers
{
    public class BoardShuffleManager : MonoBehaviour
    {
        [SerializeField] private float _shuffleSpeed;

        public static event Action<int, int, Cell[,]> OnShuffleBoardEnded;

        private bool[][] m_VisitedCache;


        private void Awake()
        {
            Board.OnBoardCreated += HandleOnBoardCreated;
            Board.OnClearRegionEnded += HandleOnClearRegionEnded;
        }

        private void OnDestroy()
        {
            Board.OnBoardCreated -= HandleOnBoardCreated;
            Board.OnClearRegionEnded -= HandleOnClearRegionEnded;
        }


        private void HandleOnBoardCreated(int rows, int columns, Cell[,] cellArray)
        {
            TryShuffleBoard(rows, columns, cellArray);
        }

        private void HandleOnClearRegionEnded(int rows, int columns, Cell[,] cellArray)
        {
            TryShuffleBoard(rows, columns, cellArray);
        }

        
        // This method is used to check if there are any possible moves on the board.
        private bool CheckForPossibleMoves(int rows, int columns, Cell[,] cellArray)
        {
            EnsureVisitedCache(rows, columns);
            var visited = m_VisitedCache;

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    if (visited[row][col] || !cellArray[row, col].GetBlock())
                        continue;

                    var groupCells = FloodFillHelper.Execute(cellArray, rows, columns, row, col, cell =>
                        cell.GetBlock()?.GetColor() == cellArray[row, col].GetBlock()?.GetColor());

                    if (groupCells.Count >= 2)
                    {
                        return true;
                    }

                    foreach (var cell in groupCells)
                    {
                        visited[cell.GetRow()][cell.GetColumn()] = true;
                    }

                    groupCells.Clear();
                }
            }

            return false;
        }

        // This method is used to ensure that the visited cache is created and has the correct size.
        private void EnsureVisitedCache(int rows, int columns)
        {
            if (m_VisitedCache == null || m_VisitedCache.Length < rows)
            {
                m_VisitedCache = new bool[rows][];
                for (var i = 0; i < rows; i++)
                {
                    m_VisitedCache[i] = new bool[columns];
                }
            }
            else
            {
                for (var i = 0; i < rows; i++)
                {
                    if (m_VisitedCache[i] == null || m_VisitedCache[i].Length < columns)
                        m_VisitedCache[i] = new bool[columns];
                    else
                        Array.Clear(m_VisitedCache[i], 0, columns);
                }
            }
        }

        private bool TryShuffleBoard(int rows, int columns, Cell[,] cellArray)
        {
            if (CheckForPossibleMoves(rows, columns, cellArray)) return false;

            ShuffleBoardWithClusters(rows, columns, cellArray);
            return true;
        }
        
        // This method is used to shuffle the board with clusters.
        private void ShuffleBoardWithClusters(int rows, int columns, Cell[,] cellArray)
        {
            var blockGroups = new Dictionary<BlockColor, List<Block>>();
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    var block = cellArray[row, col].GetBlock();
                    if (!block) continue;
                    
                    var color = block.GetColor();
                    if (!blockGroups.ContainsKey(color))
                    {
                        blockGroups[color] = new List<Block>();
                    }
                    blockGroups[color].Add(block);
                    cellArray[row, col].ClearBlock();
                }
            }

            var emptyCells = new List<Cell>();
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    if (!cellArray[row, col].GetBlock())
                    {
                        emptyCells.Add(cellArray[row, col]);
                    }
                }
            }

            var random = new System.Random();

            foreach (var color in blockGroups.Keys)
            {
                var blocks = blockGroups[color];

                var placementMode = random.Next(3);

                var orderedCells = placementMode switch
                {
                    0 => emptyCells.OrderBy(cell => cell.GetRow() * columns + cell.GetColumn()).ToList(),
                    1 => emptyCells.OrderBy(cell => cell.GetColumn() * rows + cell.GetRow()).ToList(),
                    var _ => emptyCells.OrderBy(_ => random.Next()).ToList()
                };

                foreach (var block in blocks)
                {
                    if (orderedCells.Count == 0)
                    {
                        return;
                    }

                    var targetCell = orderedCells[0];
                    orderedCells.RemoveAt(0);
                    emptyCells.Remove(targetCell);

                    targetCell.SetBlock(block);
                    block.SetCell(targetCell);

                    block.GetAnimation()
                        .DoMove(targetCell.transform.position, _shuffleSpeed, Ease.Linear)
                        .Forget();
                }
            }
            OnShuffleBoardEnded?.Invoke(rows, columns, cellArray);
        }
        
        private void RandomShuffleBoard(int rows, int columns, Cell[,] cellArray)
        {
            var blocks = new List<Block>();
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    var block = cellArray[row, col].GetBlock();
                    if (!block) continue;
                    
                    blocks.Add(block);
                    cellArray[row, col].ClearBlock();
                }
            }

            var random = new System.Random();
            for (var i = blocks.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (blocks[i], blocks[j]) = (blocks[j], blocks[i]);
            }

            var index = 0;
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    if (index >= blocks.Count) continue;
                    
                    cellArray[row, col].ClearBlock();
                    cellArray[row, col].SetBlock(blocks[index]);
                    blocks[index].SetCell(cellArray[row, col]);

                    blocks[index].GetAnimation()
                        .DoMove(cellArray[row, col].transform.position, _shuffleSpeed, Ease.Linear)
                        .Forget();

                    index++;
                }
            }

            TryShuffleBoard(rows, columns, cellArray);
            OnShuffleBoardEnded?.Invoke(rows, columns, cellArray);
        }
    }
}