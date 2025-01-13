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
        public static event Action<int, int, Cell[,]> OnShuffleBoardEnded;

        private List<Cell> _reusableGroupCells = new List<Cell>();
        private bool[][] _visitedCache;


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


        private bool CheckForPossibleMoves(int rows, int columns, Cell[,] cellArray)
        {
            EnsureVisitedCache(rows, columns);
            var visited = _visitedCache;

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

        private void EnsureVisitedCache(int rows, int columns)
        {
            if (_visitedCache == null || _visitedCache.Length != rows || _visitedCache[0].Length != columns)
            {
                _visitedCache = new bool[rows][];
                for (var i = 0; i < rows; i++)
                {
                    _visitedCache[i] = new bool[columns];
                }
            }
            else
            {
                for (var i = 0; i < rows; i++)
                {
                    Array.Clear(_visitedCache[i], 0, columns);
                }
            }
        }

        private bool TryShuffleBoard(int rows, int columns, Cell[,] cellArray)
        {
            if (CheckForPossibleMoves(rows, columns, cellArray)) return false;

            ShuffleBoard(rows, columns, cellArray);
            return true;
        }

        private void ShuffleBoard(int rows, int columns, Cell[,] cellArray)
        {
            var blocks = new List<Block>();
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    var block = cellArray[row, col].GetBlock();
                    if (block)
                    {
                        blocks.Add(block);
                        cellArray[row, col].ClearBlock();
                    }
                }
            }

            var random = new System.Random();
            for (int i = blocks.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (blocks[i], blocks[j]) = (blocks[j], blocks[i]);
            }

            var index = 0;
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    if (index < blocks.Count)
                    {
                        cellArray[row, col].ClearBlock();
                        cellArray[row, col].SetBlock(blocks[index]);
                        blocks[index].SetCell(cellArray[row, col]);

                        blocks[index].GetAnimation()
                            .DoMove(cellArray[row, col].transform.position, 0.5f, Ease.OutBounce)
                            .Forget();

                        index++;
                    }
                }
            }

            TryShuffleBoard(rows, columns, cellArray);
            OnShuffleBoardEnded?.Invoke(rows, columns, cellArray);
        }
    }
}