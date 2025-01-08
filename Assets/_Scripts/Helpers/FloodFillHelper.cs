using System;
using System.Collections.Generic;

namespace Helpers
{
    public static class FloodFillHelper
    {
        private static readonly Stack<(int, int)> STACK = new Stack<(int, int)>(); // Shared stack
        private static readonly HashSet<(int, int)> VISITED = new HashSet<(int, int)>(); // Shared HashSet
        private static readonly List<Cell> MATCHED_CELLS = new List<Cell>(); // Shared matchedCells list

        public static List<Cell> Execute(Cell[,] cells, int rows, int columns, int startRow, int startCol, Func<Cell, bool> matchCriteria)
        {
            // Clear shared collections
            STACK.Clear();
            VISITED.Clear();
            MATCHED_CELLS.Clear();

            // Initialize with the starting point
            STACK.Push((startRow, startCol));

            while (STACK.Count > 0)
            {
                var (row, col) = STACK.Pop();

                // Boundary check
                if (row < 0 || row >= rows || col < 0 || col >= columns)
                    continue;

                // Check if already visited or doesn't match criteria
                if (VISITED.Contains((row, col)) || !matchCriteria(cells[row, col]))
                    continue;

                // Mark as visited
                VISITED.Add((row, col));

                // Add to matched cells
                MATCHED_CELLS.Add(cells[row, col]);

                // Push neighbors to stack
                STACK.Push((row - 1, col)); // Up
                STACK.Push((row + 1, col)); // Down
                STACK.Push((row, col - 1)); // Left
                STACK.Push((row, col + 1)); // Right
            }

            return MATCHED_CELLS;
        }
    }
}