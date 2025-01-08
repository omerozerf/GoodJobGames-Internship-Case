using UnityEngine;

namespace Managers
{
    public class BlockVisualManager : MonoBehaviour
    {
        [SerializeField] private Board _board;

        private void Awake()
        {
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

                    var sortingOrder = row;
                    block.GetVisual().SetOrderInLayer(sortingOrder);
                }
            }
        }

        private void UpdateAllBlockSpritesBasedOnGroupSize(int rows, int columns, Cell[,] cellArray)
        {
            var visited = new bool[rows][];
            for (var index = 0; index < rows; index++)
            {
                visited[index] = new bool[columns];
            }

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    if (visited[row][col]) continue;

                    var block = cellArray[row, col].GetBlock();
                    if (!block) continue;

                    var groupCells = _board.FloodFill(row, col, cell =>
                        cell.GetBlock()?.GetColor() == block.GetColor());

                    foreach (var cell in groupCells)
                    {
                        visited[cell.GetRow()][cell.GetColumn()] = true;
                    }

                    foreach (var cell in groupCells)
                    {
                        var groupBlock = cell.GetBlock();
                        groupBlock?.GetVisual()
                            ?.UpdateSpriteBasedOnGroupSize(groupCells.Count, GameManager.GetColorsInGame());
                    }
                }
            }
        }
    }
}