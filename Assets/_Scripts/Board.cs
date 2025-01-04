using System.Collections.Generic;
using Blocks;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private Cell _cellPrefab;

    private Cell[,] m_Cells;


    private void Start()
    {
        InitializeBoard();
    }


    private void InitializeBoard()
    {
        m_Cells = new Cell[_rows, _columns];

        for (var row = 0; row < _rows; row++)
        {
            for (var col = 0; col < _columns; col++)
            {
                var cell = Instantiate(_cellPrefab, transform);
                cell.SetPosition(row, col);
                m_Cells[row, col] = cell;
            }
        }
    }

    public Cell GetCell(int row, int col)
    {
        if (row >= 0 && row < _rows && col >= 0 && col < _columns)
        {
            return m_Cells[row, col];
        }
        return null;
    }

    public List<Cell> DetectGroup(Cell startCell)
    {
        var group = new List<Cell>();
        bool[,] visited = new bool[_rows, _columns];

        FloodFill(startCell, startCell.GetBlock(), visited, group);

        return group;
    }

    private void FloodFill(Cell cell, Block targetBlock, bool[,] visited, List<Cell> group)
    {
        if (cell == null || visited[cell.GetRow(), cell.GetColumn()])
            return;

        if (cell.GetBlock() == null || !cell.GetBlock().GetMatcher().Match(targetBlock))
            return;

        visited[cell.GetRow(), cell.GetColumn()] = true;
        group.Add(cell);

        // Komşuları kontrol et
        FloodFill(GetCell(cell.GetRow() - 1, cell.GetColumn()), targetBlock, visited, group); // Yukarı
        FloodFill(GetCell(cell.GetRow() + 1, cell.GetColumn()), targetBlock, visited, group); // Aşağı
        FloodFill(GetCell(cell.GetRow(), cell.GetColumn() - 1), targetBlock, visited, group); // Sol
        FloodFill(GetCell(cell.GetRow(), cell.GetColumn() + 1), targetBlock, visited, group); // Sağ
    }

    public void ClearMatchedCells(List<Cell> matchedCells)
    {
        foreach (var cell in matchedCells)
        {
            cell.ClearBlock();
        }

        FillEmptyCells();
    }

    private void FillEmptyCells()
    {
        for (int col = 0; col < _columns; col++)
        {
            for (int row = _rows - 1; row >= 0; row--)
            {
                if (m_Cells[row, col].GetBlock() == null)
                {
                    // Üst hücrelerden blok kaydır
                    for (int r = row - 1; r >= 0; r--)
                    {
                        if (m_Cells[r, col].GetBlock() != null)
                        {
                            m_Cells[row, col].SetBlock(m_Cells[r, col].GetBlock());
                            m_Cells[r, col].ClearBlock();
                            break;
                        }
                    }

                    // Eğer yukarıda blok yoksa yeni bir blok ekle
                    if (m_Cells[row, col].GetBlock() == null)
                    {
                        Block newBlock = CreateRandomBlock();
                        m_Cells[row, col].SetBlock(newBlock);
                    }
                }
            }
        }
    }

    private Block CreateRandomBlock()
    {
        // Rastgele bir blok oluştur (örnek olarak)
        return null;
    }
}