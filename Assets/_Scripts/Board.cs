using System;
using System.Collections.Generic;
using Blocks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private Block[] _blockPrefabArray;

    private Cell[,] m_Cells;


    private void Start()
    {
        InitializeBoard();
        FillEmptyCells();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("Hit: " + hit.collider.name);
                var blockCollision = hit.collider.GetComponent<BlockCollision>();
                var block = blockCollision?.GetBlock();
                if (block != null)
                {
                    var group = DetectGroup(block.GetCell());
                    if (group.Count >= 2)
                    {
                        ClearMatchedCells(group);
                    }
                }
            }
        }
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
        if (startCell == null || startCell.GetBlock() == null)
        {
            return new List<Cell>();
        }

        var group = new List<Cell>();
        bool[,] visited = new bool[_rows, _columns];

        FloodFill(startCell, startCell.GetBlock(), visited, group);

        return group;
    }

    private void FloodFill(Cell cell, Block targetBlock, bool[,] visited, List<Cell> group)
    {
        if (cell == null)
            return;

        int row = cell.GetRow();
        int col = cell.GetColumn();

        // Hücre sınırlarını kontrol et
        if (row < 0 || row >= _rows || col < 0 || col >= _columns)
            return;

        // Daha önce ziyaret edilmiş mi kontrol et
        if (visited[row, col])
            return;

        // Hücre boş mu veya hedef blok ile eşleşmiyor mu kontrol et
        if (cell.GetBlock() == null || !cell.GetBlock().GetMatcher().Match(targetBlock))
            return;

        // Hücreyi ziyaret edilmiş olarak işaretle
        visited[row, col] = true;

        // Gruba ekle
        group.Add(cell);

        // Komşuları kontrol et
        FloodFill(GetCell(row - 1, col), targetBlock, visited, group); // Yukarı
        FloodFill(GetCell(row + 1, col), targetBlock, visited, group); // Aşağı
        FloodFill(GetCell(row, col - 1), targetBlock, visited, group); // Sol
        FloodFill(GetCell(row, col + 1), targetBlock, visited, group); // Sağ
    }

    public void ClearMatchedCells(List<Cell> matchedCells)
    {
        Debug.Log("sadadwa");
        foreach (var cell in matchedCells)
        {
            cell.ClearBlock();
        }

        // FillEmptyCells();
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
                        newBlock.SetCell(m_Cells[row, col]);
                    }
                }
            }
        }
    }

    private Block CreateRandomBlock()
    {
        return Instantiate(_blockPrefabArray[Random.Range(0, _blockPrefabArray.Length)]);
    }
}