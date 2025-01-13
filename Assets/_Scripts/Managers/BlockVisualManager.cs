using System.Collections.Generic;
using Others;
using UnityEngine;

namespace Managers
{
    public class BlockVisualManager : MonoBehaviour
    {
        [SerializeField] private Board _board;
        
        private readonly HashSet<Cell> m_Visited = new HashSet<Cell>();
        private readonly Dictionary<Cell, int> m_GroupCache = new Dictionary<Cell, int>();

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

                    block.GetVisual().SetOrderInLayer(row);
                }
            }
        }

        private void UpdateAllBlockSpritesBasedOnGroupSize(int rows, int columns, Cell[,] cellArray)
        {
            // 1. Visited hücrelerin takibi için bir HashSet kullanıyoruz.
            m_Visited.Clear();

            // 2. Grup boyutlarını cache etmek için bir Dictionary kullanıyoruz.
            m_GroupCache.Clear();

            // 3. Bütün hücreleri iteratif olarak dolaşıyoruz.
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    var currentCell = cellArray[row, col];
            
                    // Hücre zaten ziyaret edildiyse veya boşsa işlem yapma.
                    if (m_Visited.Contains(currentCell) || currentCell.GetBlock() == null)
                        continue;

                    // 4. Flood-Fill algoritmasını çalıştır ve grup hücrelerini al.
                    var groupCells = _board.FloodFill(row, col, (cell) =>
                        cell.GetBlock()?.GetColor() == currentCell.GetBlock().GetColor());

                    // Grup hücrelerini visited listesine ekle ve cache'le.
                    foreach (var groupCell in groupCells)
                    {
                        m_Visited.Add(groupCell);
                        m_GroupCache[groupCell] = groupCells.Count;
                    }

                    // Grup boyutuna göre sprite'ları güncelle.
                    foreach (var groupCell in groupCells)
                    {
                        groupCell.GetBlock()?.GetVisual()
                            ?.UpdateSpriteBasedOnGroupSize(groupCells.Count, GameManager.GetColorsInGame());
                    }
                }
            }
        }
    }
}