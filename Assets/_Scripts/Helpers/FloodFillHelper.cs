using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Others;

namespace Helpers
{
    public static class FloodFillHelper
    {
        private static readonly ConcurrentBag<List<Cell>> CELL_LIST_POOL = new();
        
        private static bool[] ms_VisitedCells;
        private static Stack<int> ms_TraversalStack;

        // Main execution method for flood fill algorithm.
        public static List<Cell> Execute(Cell[,] grid, int rowCount, int columnCount, int startRow, int startColumn, Func<Cell, bool> shouldVisit)
        {
            InitializeVisitedCells(rowCount, columnCount);
            InitializeTraversalStack(rowCount, columnCount);
            var matchedCells = FetchOrCreateCellList(rowCount, columnCount);
            ms_TraversalStack.Push(ComputeIndex(startRow, startColumn, columnCount));
            PerformFloodFill(grid, rowCount, columnCount, shouldVisit, matchedCells);
            CELL_LIST_POOL.Add(matchedCells);
            return matchedCells;
        }

        // Initialize or clear the array tracking visited cells.
        private static void InitializeVisitedCells(int rowCount, int columnCount)
        {
            var totalCells = rowCount * columnCount;
            if (ms_VisitedCells == null || ms_VisitedCells.Length < totalCells)
            {
                ms_VisitedCells = new bool[totalCells];
            }
            else
            {
                Array.Clear(ms_VisitedCells, 0, ms_VisitedCells.Length);
            }
        }

        // Prepare or reset the stack used for tracking cell traversal.
        private static void InitializeTraversalStack(int rowCount, int columnCount)
        {
            ms_TraversalStack ??= new Stack<int>(rowCount * columnCount);
            ms_TraversalStack.Clear();
        }

        // Attempt to reuse a list from the pool or create a new list if none are available.
        private static List<Cell> FetchOrCreateCellList(int rowCount, int columnCount)
        {
            return CELL_LIST_POOL.TryTake(out var list) ? list : new List<Cell>(rowCount * columnCount);
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
            while (ms_TraversalStack.Count > 0)
            {
                var currentIndex = ms_TraversalStack.Pop();
                var currentRow = currentIndex / columnCount;
                var currentColumn = currentIndex % columnCount;

                if (IsOutsideBounds(currentRow, rowCount, currentColumn, columnCount))
                    continue;

                if (ms_VisitedCells[currentIndex] || !shouldVisit(grid[currentRow, currentColumn]))
                    continue;

                ms_VisitedCells[currentIndex] = true;
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
            if (row > 0) ms_TraversalStack.Push(ComputeIndex(row - 1, column, columnCount));
            if (row < rowCount - 1) ms_TraversalStack.Push(ComputeIndex(row + 1, column, columnCount));
            if (column > 0) ms_TraversalStack.Push(ComputeIndex(row, column - 1, columnCount));
            if (column < columnCount - 1) ms_TraversalStack.Push(ComputeIndex(row, column + 1, columnCount));
        }
    }
}