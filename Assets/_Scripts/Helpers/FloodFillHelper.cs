using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Others;

namespace Helpers
{
    public static class FloodFillHelper
    {
        private static bool[] ms_VisitedPool;
        private static Stack<int> ms_StackPool;
        private static readonly ConcurrentBag<List<Cell>> CELL_LIST_POOL = new();

        public static List<Cell> Execute(Cell[,] cells, int rows, int columns, int startRow, int startCol, Func<Cell, bool> matchCriteria)
        {
            if (ms_VisitedPool == null || ms_VisitedPool.Length < rows * columns)
            {
                ms_VisitedPool = new bool[rows * columns];
            }
            else
            {
                Array.Clear(ms_VisitedPool, 0, ms_VisitedPool.Length);
            }

            ms_StackPool ??= new Stack<int>(rows * columns);
            ms_StackPool.Clear();

            var matchedCells = CELL_LIST_POOL.TryTake(out var list) ? list : new List<Cell>(rows * columns);
            matchedCells.Clear();

            ms_StackPool.Push(startRow * columns + startCol);

            while (ms_StackPool.Count > 0)
            {
                var position = ms_StackPool.Pop();
                var row = position / columns;
                var col = position % columns;

                if (row < 0 || row >= rows || col < 0 || col >= columns)
                    continue;

                var index = row * columns + col;

                if (ms_VisitedPool[index] || !matchCriteria(cells[row, col]))
                    continue;

                ms_VisitedPool[index] = true;

                matchedCells.Add(cells[row, col]);

                if (row > 0) ms_StackPool.Push((row - 1) * columns + col);
                if (row < rows - 1) ms_StackPool.Push((row + 1) * columns + col);
                if (col > 0) ms_StackPool.Push(row * columns + (col - 1));
                if (col < columns - 1) ms_StackPool.Push(row * columns + (col + 1));
            }

            CELL_LIST_POOL.Add(matchedCells);

            return matchedCells;
        }
    }
}