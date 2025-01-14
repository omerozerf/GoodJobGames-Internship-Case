using System.Collections.Generic;
using Others;
using UnityEngine;

namespace Managers
{
    public class BlockVisualManager : MonoBehaviour
    {
        [SerializeField] private Board _board;
        
        private readonly HashSet<Cell> m_Visited = new HashSet<Cell>();
        private int m_ColorsInGame;

        
        private void Awake()
        {
            m_ColorsInGame = GameManager.GetColorsInGame();
            
            Board.OnFillEmptyCellsEnded += HandleOnFillEmptyCellsEnded;
            BoardShuffleManager.OnShuffleBoardEnded += HandleShuffleBoardEnded;
        }

        private void OnDestroy()
        {
            Board.OnFillEmptyCellsEnded -= HandleOnFillEmptyCellsEnded;
            BoardShuffleManager.OnShuffleBoardEnded -= HandleShuffleBoardEnded;
        }


        private void HandleShuffleBoardEnded(int rows, int column, Cell[,] cellArray)
        {
            UpdateBlockSortingOrder(rows, column, cellArray);
            UpdateAllBlockSpritesBasedOnGroupSize(rows, column, cellArray);
        }

        private void HandleOnFillEmptyCellsEnded(int rows, int column, Cell[,] cellArray)
        {
            UpdateBlockSortingOrder(rows, column, cellArray);
            UpdateAllBlockSpritesBasedOnGroupSize(rows, column, cellArray);
        }


        private void UpdateBlockSortingOrder(int rows, int columns, Cell[,] cellArray)
        {
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    var block = cellArray[row, col].GetBlock();
                    if (!block) continue;

                    block.GetVisual().SetOrderInLayer(row);
                }
            }
        }

        private void UpdateAllBlockSpritesBasedOnGroupSize(int rows, int columns, Cell[,] cellArray)
        {
            m_Visited.Clear();

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    var currentCell = cellArray[row, col];
            
                    if (m_Visited.Contains(currentCell) || !currentCell.GetBlock())
                        continue;

                    var groupCells = _board.FloodFill(row, col, (cell) =>
                        cell.GetBlock()?.GetColor() == currentCell.GetBlock().GetColor());

                    foreach (var groupCell in groupCells)
                    {
                        m_Visited.Add(groupCell);
                    }

                    foreach (var groupCell in groupCells)
                    {
                        groupCell.GetBlock()?.GetVisual()
                            ?.UpdateSpriteBasedOnGroupSize(groupCells.Count, m_ColorsInGame);
                    }
                }
            }
        }
    }
}