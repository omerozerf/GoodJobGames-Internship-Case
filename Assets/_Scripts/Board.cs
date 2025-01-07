using System;
using System.Collections.Generic;
using System.Linq;
using Blocks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class Board : MonoBehaviour
{
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private Block[] _blockPrefabArray;

    private Cell[,] m_Cells;

    private void Start()
    {
        CenterCamera();
        InitializeBoard();
        FillEmptyCellsAsync();

        if (!CheckForPossibleMoves())
        {
            Debug.Log("Deadlock detected on board initialization! Shuffling the board.");
            ShuffleBoard();
        }
    }

    private async void Update()
    {
        try
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
                if (hit.collider != null)
                {
                    var blockCollision = hit.collider.GetComponent<BlockCollision>();
                    var block = blockCollision?.GetBlock();
                    if (block != null)
                    {
                        await ClearRegionAsync(block.GetCell().GetRow(), block.GetCell().GetColumn());

                        // After clearing blocks and updating the board, check for a deadlock
                        if (!CheckForPossibleMoves())
                        {
                            Debug.Log("Deadlock detected! Shuffling the board.");
                            ShuffleBoard();
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }

    public List<Cell> FloodFill(int startRow, int startCol, Func<Cell, bool> matchCriteria)
    {
        var matchedCells = new List<Cell>();
        var visited = new bool[_rows][];
        for (var index = 0; index < _rows; index++)
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

    private void UpdateBlockSortingOrder()
    {
        for (var row = 0; row < _rows; row++)
        {
            for (var col = 0; col < _columns; col++)
            {
                var block = m_Cells[row, col].GetBlock();
                if (block != null)
                {
                    var sortingOrder = row;
                    block.GetVisual().SetOrderInLayer(sortingOrder);
                }
            }
        }

        UpdateAllBlockSpritesBasedOnGroupSize();
    }

    private void UpdateAllBlockSpritesBasedOnGroupSize()
    {
        // Tüm hücreleri kontrol etmek için bir visited matrisi
        var visited = new bool[_rows][];
        for (var index = 0; index < _rows; index++)
        {
            visited[index] = new bool[_columns];
        }

        for (var row = 0; row < _rows; row++)
        {
            for (var col = 0; col < _columns; col++)
            {
                // Eğer hücre zaten kontrol edildiyse atla
                if (visited[row][col]) continue;

                var block = m_Cells[row, col].GetBlock();
                if (block != null)
                {
                    // FloodFill ile bu blok grubunu bul
                    var groupCells = FloodFill(row, col, cell =>
                        cell.GetBlock()?.GetColor() == block.GetColor());

                    // Grup hücrelerini visited olarak işaretle
                    foreach (var cell in groupCells)
                    {
                        visited[cell.GetRow()][cell.GetColumn()] = true;
                    }

                    // Grup boyutuna göre tüm blokların sprite'ını güncelle
                    foreach (var cell in groupCells)
                    {
                        var groupBlock = cell.GetBlock();
                        groupBlock?.GetVisual()?.UpdateSpriteBasedOnGroupSize(groupCells.Count);
                    }
                }
            }
        }
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
    }

    public Cell GetCell(int row, int col)
    {
        if (row >= 0 && row < _rows && col >= 0 && col < _columns)
        {
            return m_Cells[row, col];
        }

        return null;
    }



    public async UniTask ClearMatchedCellsAsync(List<Cell> matchedCells)
    {
        var tasks = new List<UniTask>();

        foreach (var cell in matchedCells)
        {
            var block = cell.GetBlock();
            if (block != null)
            {
                var task = block.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack)
                    .OnComplete(() => Destroy(block.gameObject)).ToUniTask();
                tasks.Add(task);
            }

            cell.ClearBlock();
        }

        await UniTask.WhenAll(tasks);
        await FillEmptyCellsAsync();

        UpdateBlockSortingOrder();

        // Check for deadlocks after the board updates
        if (!CheckForPossibleMoves())
        {
            Debug.Log("Deadlock detected! Shuffling the board.");
            ShuffleBoard();
        }
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
                        var newBlock = CreateRandomBlock(col);
                        m_Cells[row, col].SetBlock(newBlock);
                        newBlock.SetCell(m_Cells[row, col]);

                        var task =newBlock.transform.DOMove(m_Cells[row, col].transform.position, 0.5f).SetEase(Ease.OutBounce).ToUniTask();
                        tasks.Add(task);
                    }
                }
            }
        }
        UpdateBlockSortingOrder();
        await UniTask.WhenAll(tasks);
    }

    private void CenterCamera()
    {
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            var centerX = (_columns - 1) * 0.5f;
            var centerY = (_rows - 1) * 0.5f;

            mainCamera.transform.position = new Vector3(centerX, centerY, mainCamera.transform.position.z);

            var aspectRatio = mainCamera.aspect;
            float boardHeight = _rows;
            float boardWidth = _columns;

            mainCamera.orthographicSize = Mathf.Max(boardHeight / 2, boardWidth / (2 * aspectRatio)) * 1.5f;
        }
    }

    private Block CreateRandomBlock(int column)
    {
        var mainCamera = Camera.main;

        var cameraTopY = mainCamera.transform.position.y + mainCamera.orthographicSize;

        var spawnPosition = new Vector3(
            m_Cells[0, column].transform.position.x,
            cameraTopY + 2.0f,
            0
        );

        var newBlock = Instantiate(_blockPrefabArray[UnityEngine.Random.Range(0, _blockPrefabArray.Length)]);

        newBlock.transform.position = spawnPosition;

        return newBlock;
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

        if (!CheckForPossibleMoves())
        {
            ShuffleBoard();
        }

        UpdateBlockSortingOrder();
    }
}