using System;
using UnityEngine;

public class BlockSortingOrderManager : MonoBehaviour
{
    private void Awake()
    {
        Board.OnFillEmptyCellsEnded += HandleOnFillEmptyCellsEnded;
        Board.OnShuffleBoardEnded += HandleShuffleBoardEnded;
    }

    private void OnDestroy()
    {
        Board.OnFillEmptyCellsEnded -= HandleOnFillEmptyCellsEnded;
        Board.OnShuffleBoardEnded -= HandleShuffleBoardEnded;
    }


    private void HandleShuffleBoardEnded(int rows, int column, Cell[,] cellArray)
    {
        UpdateBlockSortingOrder(rows, column, cellArray);
    }

    private void HandleOnFillEmptyCellsEnded(int rows, int column, Cell[,] cellArray)
    {
        UpdateBlockSortingOrder(rows, column, cellArray);
    }


    private void UpdateBlockSortingOrder(int rows, int columns, Cell[,] cellArray)
    {
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < columns; col++)
            {
                var block = cellArray[row, col].GetBlock();
                if (block != null)
                {
                    var sortingOrder = row;
                    block.GetVisual().SetOrderInLayer(sortingOrder);
                }
            }
        }

        // UpdateAllBlockSpritesBasedOnGroupSize();
    }
}