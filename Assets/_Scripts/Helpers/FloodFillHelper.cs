using System;
using System.Collections.Generic;

namespace Helpers
{
    public static class FloodFillHelper
    {
        public static List<Cell> Execute(Cell[,] cells, int rows, int columns, int startRow, int startCol, Func<Cell, bool> matchCriteria)
        {
            var matchedCells = new List<Cell>();
            var visited = new bool[rows][];
            for (var index = 0; index < rows; index++)
            {
                visited[index] = new bool[columns];
            }

            void Fill(int row, int col)
            {
                if (row < 0 || row >= rows || col < 0 || col >= columns)
                    return;

                if (visited[row][col] || !matchCriteria(cells[row, col]))
                    return;

                visited[row][col] = true;
                matchedCells.Add(cells[row, col]);

                Fill(row - 1, col); // Up
                Fill(row + 1, col); // Down
                Fill(row, col - 1); // Left
                Fill(row, col + 1); // Right
            }

            Fill(startRow, startCol);
            return matchedCells;
        }
    }
}