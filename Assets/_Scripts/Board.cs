using System;
using System.Collections.Generic;
using Blocks;
using UnityEngine;
using DG.Tweening;

public class Board : MonoBehaviour
{
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private Block[] _blockPrefabArray;

    private Cell[,] m_Cells;
    private bool flag;

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
                var blockCollision = hit.collider.GetComponent<BlockCollision>();
                var block = blockCollision?.GetBlock();
                if (block != null)
                {
                    ClearRegion(block.GetCell().GetRow(), block.GetCell().GetColumn());
                }
            }
        }
    }

    public List<Cell> FloodFill(int startRow, int startCol, Func<Cell, bool> matchCriteria)
    {
        List<Cell> matchedCells = new List<Cell>();
        bool[][] visited = new bool[_rows][];
        for (int index = 0; index < _rows; index++)
        {
            visited[index] = new bool[_columns];
        }

        void Fill(int row, int col)
        {
            if (row < 0 || row >= _rows || col < 0 || col >= _columns)
                return;

            if (visited[row][col] || !matchCriteria(m_Cells[row, col]))
                return;

            visited[row][col] = true;
            matchedCells.Add(m_Cells[row, col]);

            Fill(row - 1, col); // Up
            Fill(row + 1, col); // Down
            Fill(row, col - 1); // Left
            Fill(row, col + 1); // Right
        }

        Fill(startRow, startCol);
        return matchedCells;
    }

    public void ClearRegion(int startRow, int startCol)
    {
        Func<Cell, bool> matchCriteria = cell => cell.GetBlock()?.GetColor() == m_Cells[startRow, startCol].GetBlock()?.GetColor();

        List<Cell> matchedCells = FloodFill(startRow, startCol, matchCriteria);

        if (matchedCells.Count >= 2)
        {
            ClearMatchedCells(matchedCells);
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



    public void ClearMatchedCells(List<Cell> matchedCells)
    {
        foreach (var cell in matchedCells)
        {
            var block = cell.GetBlock();
            if (block != null)
            {
                block.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack)
                    .OnComplete(() => Destroy(block.gameObject));
            }
            cell.ClearBlock();
        }

        DOVirtual.DelayedCall(0.6f, FillEmptyCells);
    }

    private void FillEmptyCells()
    {
        for (int col = 0; col < _columns; col++)
        {
            for (int row = 0; row < _rows; row++)
            {
                if (m_Cells[row, col].GetBlock() == null)
                {
                    for (int r = row + 1; r < _rows; r++)
                    {
                        if (m_Cells[r, col].GetBlock() != null)
                        {
                            var block = m_Cells[r, col].GetBlock();

                            m_Cells[row, col].SetBlock(block);
                            m_Cells[r, col].ClearBlock();
                            block.SetCell(m_Cells[row, col]);

                            block.transform.DOMove(m_Cells[row, col].transform.position, 0.3f).SetEase(Ease.OutBounce);
                            break;
                        }
                    }

                    if (m_Cells[row, col].GetBlock() == null)
                    {
                        Block newBlock = CreateRandomBlock();
                        m_Cells[row, col].SetBlock(newBlock);
                        newBlock.SetCell(m_Cells[row, col]);

                        newBlock.transform.position = m_Cells[row, col].transform.position + Vector3.up * 2;
                        newBlock.transform.DOMove(m_Cells[row, col].transform.position, 0.5f).SetEase(Ease.OutBounce);
                    }
                }
            }
        }
    }

    private Block CreateRandomBlock()
    {
        return Instantiate(_blockPrefabArray[UnityEngine.Random.Range(0, _blockPrefabArray.Length)]);
    }
}