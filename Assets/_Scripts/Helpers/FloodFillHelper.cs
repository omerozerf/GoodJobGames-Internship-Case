using System;
using System.Collections.Generic;
using Others;

namespace Helpers
{
    public static class FloodFillHelper
    {
        private static bool[] visitedPool;
        private static Stack<int> stackPool;

        public static List<Cell> Execute(Cell[,] cells, int rows, int columns, int startRow, int startCol, Func<Cell, bool> matchCriteria)
        {
            // Havuzlanan visited dizisi
            if (visitedPool == null || visitedPool.Length < rows * columns)
            {
                visitedPool = new bool[rows * columns];
            }
            else
            {
                Array.Clear(visitedPool, 0, visitedPool.Length);
            }

            // Havuzlanan stack
            stackPool ??= new Stack<int>(rows * columns);
            stackPool.Clear();

            // Matched cells
            var matchedCells = new List<Cell>(rows * columns);

            // Başlangıç pozisyonunu stack'e ekle
            stackPool.Push(startRow * columns + startCol);

            while (stackPool.Count > 0)
            {
                var position = stackPool.Pop();
                int row = position / columns;
                int col = position % columns;

                // Boundary check
                if (row < 0 || row >= rows || col < 0 || col >= columns)
                    continue;

                // Indexi hesapla
                int index = row * columns + col;

                // Zaten ziyaret edildiyse veya eşleşmiyorsa devam et
                if (visitedPool[index] || !matchCriteria(cells[row, col]))
                    continue;

                // Ziyaret edildi olarak işaretle
                visitedPool[index] = true;

                // Eşleşen hücreyi ekle
                matchedCells.Add(cells[row, col]);

                // Komşuları stack'e ekle
                if (row > 0) stackPool.Push((row - 1) * columns + col);     // Yukarı
                if (row < rows - 1) stackPool.Push((row + 1) * columns + col); // Aşağı
                if (col > 0) stackPool.Push(row * columns + (col - 1));     // Sol
                if (col < columns - 1) stackPool.Push(row * columns + (col + 1)); // Sağ
            }

            return matchedCells;
        }
    }
}