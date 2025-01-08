using System;
using System.Collections.Generic;
using System.Linq;
using Blocks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Managers;

public class Board : MonoBehaviour
{
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private BlockCreateManager _blockCreateManager;

    public static event Action<int, int> OnInitializeBoard;
    public static event Action<int, int, Cell[,]> OnFillEmptyCellsEnded;
    public static event Action<int, int, Cell[,]> OnShuffleBoardEnded;

    private Cell[,] m_Cells;
    private bool m_CanInteract;


    private void Awake()
    {
        PlayerInputManager.OnMouseClick += HandleOnMouseClick;
    }

    private void Start()
    {
        m_CanInteract = true;
        InitializeBoard();
        _ = FillEmptyCellsAsync();

        TryShuffleBoard();
    }

    private void OnDestroy()
    {
        PlayerInputManager.OnMouseClick -= HandleOnMouseClick;
    }

    private async void HandleOnMouseClick(Vector2 mousePosition)
    {
        try
        {
            if (!m_CanInteract) return;
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (!hit.collider) return;

            if (hit.collider.TryGetComponent(out BlockCollision blockCollision))
            {
                var block = blockCollision.GetBlock();
                if (!block) return;

                m_CanInteract = false;
                await ClearRegionAsync(block.GetCell().GetRow(), block.GetCell().GetColumn());

                TryShuffleBoard();
                m_CanInteract = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public List<Cell> FloodFill(int startRow, int startCol, Func<Cell, bool> matchCriteria)
    {
        return FloodFillHelper.Execute(m_Cells, _rows, _columns, startRow, startCol, matchCriteria);
    }

    public async UniTask ClearRegionAsync(int startRow, int startCol)
    {
        Func<Cell, bool> matchCriteria = cell =>
            cell.GetBlock()?.GetColor() == m_Cells[startRow, startCol].GetBlock()?.GetColor();

        var matchedCells = FloodFill(startRow, startCol, matchCriteria);

        if (matchedCells.Count >= 2)
        {
            await ClearMatchedCellsAsync(matchedCells);
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

        OnInitializeBoard?.Invoke(_rows, _columns);
    }

    private async UniTask ClearMatchedCellsAsync(List<Cell> matchedCells)
    {
        const float scaleTime = 0.5f;
        foreach (var cell in matchedCells)
        {
            var block = cell.GetBlock();
            if (block != null)
            {
                block.transform.DOScale(Vector3.zero, scaleTime)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => _blockCreateManager.ReturnBlockToPool(block));
            }

            cell.ClearBlock();
        }

        await UniTask.WaitForSeconds(scaleTime);

        _ = FillEmptyCellsAsync();
        TryShuffleBoard();
    }

    private async UniTask FillEmptyCellsAsync()
    {
        var tasks = new List<UniTask>();
        for (var col = 0; col < _columns; col++)
        {
            for (var row = 0; row < _rows; row++)
            {
                if (m_Cells[row, col].GetBlock() == null)
                {
                    for (var r = row + 1; r < _rows; r++)
                    {
                        if (m_Cells[r, col].GetBlock() != null)
                        {
                            var block = m_Cells[r, col].GetBlock();

                            m_Cells[row, col].SetBlock(block);
                            m_Cells[r, col].ClearBlock();
                            block.SetCell(m_Cells[row, col]);

                            var task = block.transform.DOMove(m_Cells[row, col].transform.position, 0.3f).SetEase(Ease.OutBounce).
                                ToUniTask();
                            tasks.Add(task);
                            break;
                        }
                    }

                    if (m_Cells[row, col].GetBlock() == null)
                    {
                        var newBlock = _blockCreateManager.CreateRandomBlock(col, m_Cells);
                        m_Cells[row, col].SetBlock(newBlock);
                        newBlock.SetCell(m_Cells[row, col]);

                        var task =newBlock.transform.DOMove(m_Cells[row, col].transform.position, 0.5f).SetEase(Ease.OutBounce).ToUniTask();
                        tasks.Add(task);
                    }
                }
            }
        }
        OnFillEmptyCellsEnded?.Invoke(_rows, _columns, m_Cells);
        await UniTask.WhenAll(tasks);
    }

    private bool CheckForPossibleMoves()
    {
        var visited = new bool[_rows][];
        for (int index = 0; index < _rows; index++)
        {
            visited[index] = new bool[_columns];
        }

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                if (visited[row][col] || m_Cells[row, col].GetBlock() == null)
                    continue;

                var groupCells = FloodFill(row, col, cell =>
                    cell.GetBlock()?.GetColor() == m_Cells[row, col].GetBlock()?.GetColor());

                if (groupCells.Count >= 2)
                {
                    return true;
                }

                foreach (var cell in groupCells)
                {
                    visited[cell.GetRow()][cell.GetColumn()] = true;
                }
            }
        }

        return false;
    }

    private bool TryShuffleBoard()
    {
        if (CheckForPossibleMoves()) return false;

        ShuffleBoard();
        return true;
    }

    private void ShuffleBoard()
    {
        // Flatten the board into a list
        var blocks = new List<Block>();
        for (var row = 0; row < _rows; row++)
        {
            for (var col = 0; col < _columns; col++)
            {
                var block = m_Cells[row, col].GetBlock();
                if (block != null)
                {
                    blocks.Add(block);
                    m_Cells[row, col].ClearBlock();
                }
            }
        }

        var random = new System.Random();
        blocks = blocks.OrderBy(_ => random.Next()).ToList();

        var index = 0;
        for (var row = 0; row < _rows; row++)
        {
            for (var col = 0; col < _columns; col++)
            {
                if (index < blocks.Count)
                {
                    m_Cells[row, col].ClearBlock();
                    m_Cells[row, col].SetBlock(blocks[index]);
                    blocks[index].SetCell(m_Cells[row, col]);

                    blocks[index].transform.DOMove(m_Cells[row, col].transform.position, 0.5f).SetEase(Ease.OutBounce);

                    index++;
                }
            }
        }

        TryShuffleBoard();
        OnShuffleBoardEnded?.Invoke(_rows, _columns, m_Cells);
    }
}