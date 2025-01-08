using System;
using System.Collections.Generic;
using System.Linq;
using Blocks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Helpers;
using Managers;

public class Board : MonoBehaviour
{
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField, Range(1, 6)] private int _colorsInGame;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private BlockCreateManager _blockCreateManager;

    public static event Action<int, int> OnInitializeBoard;
    public static event Action<int, int, Cell[,]> OnFillEmptyCellsEnded;
    public static event Action<int, int, Cell[,]> OnBoardCreated;
    public static event Action<int, int, Cell[,]> OnClearRegionEnded;

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

        OnBoardCreated?.Invoke(_rows, _columns, m_Cells);
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

                OnClearRegionEnded?.Invoke(_rows, _columns, m_Cells);
                m_CanInteract = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
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

    private async UniTask ClearRegionAsync(int startRow, int startCol)
    {
        Func<Cell, bool> matchCriteria = cell =>
            cell.GetBlock()?.GetColor() == m_Cells[startRow, startCol].GetBlock()?.GetColor();

        var matchedCells = FloodFill(startRow, startCol, matchCriteria);

        if (matchedCells.Count >= 2)
        {
            await ClearMatchedCellsAsync(matchedCells);
        }
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


    public List<Cell> FloodFill(int startRow, int startCol, Func<Cell, bool> matchCriteria)
    {
        return FloodFillHelper.Execute(m_Cells, _rows, _columns, startRow, startCol, matchCriteria);
    }

    public int GetColorsInGame()
    {
        return _colorsInGame;
    }
}