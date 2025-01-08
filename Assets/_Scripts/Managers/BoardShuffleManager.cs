using System;
using System.Collections.Generic;
using System.Linq;
using Blocks;
using DG.Tweening;
using Helpers;
using UnityEngine;

namespace Managers
{
    public class BoardShuffleManager : MonoBehaviour
    {
        public static event Action<int, int, Cell[,]> OnShuffleBoardEnded;


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
            var visited = new bool[rows][];
            for (int index = 0; index < rows; index++)
            {
                visited[index] = new bool[columns];
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (visited[row][col] || cellArray[row, col].GetBlock() == null)
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
                }
            }

            return false;
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
                if (block != null)
                {
                    blocks.Add(block);
                    cellArray[row, col].ClearBlock();
                }
            }
        }

            var random = new System.Random();
            blocks = blocks.OrderBy(_ => random.Next()).ToList();

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

                        blocks[index].transform.DOMove(cellArray[row, col].transform.position, 0.5f).SetEase(Ease.OutBounce);

                        index++;
                    }
                }
            }

            TryShuffleBoard(rows, columns, cellArray);
            OnShuffleBoardEnded?.Invoke(rows, columns, cellArray);
        }
    }
}