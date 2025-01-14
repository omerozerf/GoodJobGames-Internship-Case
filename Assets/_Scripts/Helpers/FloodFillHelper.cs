using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Others;

namespace Helpers
{
    public static class FloodFillHelper
    {
        // Static fields to manage visited cells and traversal stack.
        private static bool[] visitedCells;
        private static Stack<int> traversalStack;
        private static readonly ConcurrentBag<List<Cell>> cellListPool = new();

        // Main execution method for flood fill algorithm.
        public static List<Cell> Execute(Cell[,] grid, int rowCount, int columnCount, int startRow, int startColumn, Func<Cell, bool> shouldVisit)
        {
            InitializeVisitedCells(rowCount, columnCount);
            InitializeTraversalStack(rowCount, columnCount);
            var matchedCells = FetchOrCreateCellList(rowCount, columnCount);
            traversalStack.Push(ComputeIndex(startRow, startColumn, columnCount));
            PerformFloodFill(grid, rowCount, columnCount, shouldVisit, matchedCells);
            cellListPool.Add(matchedCells);
            return matchedCells;
        }

        // Initialize or clear the array tracking visited cells.
        private static void InitializeVisitedCells(int rowCount, int columnCount)
        {
            var totalCells = rowCount * columnCount;
            if (visitedCells == null || visitedCells.Length < totalCells)
            {
                visitedCells = new bool[totalCells];
            }
            else
            {
                Array.Clear(visitedCells, 0, visitedCells.Length);
            }
        }

        // Prepare or reset the stack used for tracking cell traversal.
        private static void InitializeTraversalStack(int rowCount, int columnCount)
        {
            traversalStack ??= new Stack<int>(rowCount * columnCount);
            traversalStack.Clear();
        }

        // Attempt to reuse a list from the pool or create a new list if none are available.
        private static List<Cell> FetchOrCreateCellList(int rowCount, int columnCount)
        {
            return cellListPool.TryTake(out var list) ? list : new List<Cell>(rowCount * columnCount);
        }

        // Helper to compute a one-dimensional index from two-dimensional coordinates.
        private static int ComputeIndex(int row, int column, int columnCount)
        {
            return row * columnCount + column;
        }

        // Core logic of the flood fill algorithm.
        private static void PerformFloodFill(Cell[,] grid, int rowCount, int columnCount, Func<Cell, bool> shouldVisit, List<Cell> matchedCells)
        {
            matchedCells.Clear();
            while (traversalStack.Count > 0)
            {
                var currentIndex = traversalStack.Pop();
                var currentRow = currentIndex / columnCount;
                var currentColumn = currentIndex % columnCount;

                if (IsOutsideBounds(currentRow, rowCount, currentColumn, columnCount))
                    continue;

                if (visitedCells[currentIndex] || !shouldVisit(grid[currentRow, currentColumn]))
                    continue;

                visitedCells[currentIndex] = true;
                matchedCells.Add(grid[currentRow, currentColumn]);
                PushAdjacentCells(currentRow, currentColumn, rowCount, columnCount);
            }
        }

        // Check if the current cell is outside the grid bounds.
        private static bool IsOutsideBounds(int row, int rowCount, int column, int columnCount)
        {
            return row < 0 || row >= rowCount || column < 0 || column >= columnCount;
        }

        // Add adjacent cells to the stack if they meet the bounds condition.
        private static void PushAdjacentCells(int row, int column, int rowCount, int columnCount)
        {
            if (row > 0) traversalStack.Push(ComputeIndex(row - 1, column, columnCount));
            if (row < rowCount - 1) traversalStack.Push(ComputeIndex(row + 1, column, columnCount));
            if (column > 0) traversalStack.Push(ComputeIndex(row, column - 1, columnCount));
            if (column < columnCount - 1) traversalStack.Push(ComputeIndex(row, column + 1, columnCount));
        }
    }
}