using System;
using System.Collections.Generic;
using Blocks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Helpers;
using Managers;
using UnityEngine;

namespace Others
{
    public class Board : MonoBehaviour
    {
        [Header("Block Animation Times")]
        [SerializeField] private float _blastBlockTime;
        [SerializeField] private float _moveBlockDownTime;
        [SerializeField] private float _newBlockMoveTime;

        [Header("References")]
        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private BlockCreateManager _blockCreateManager;
        [SerializeField] private Transform _cellsTransform;

        public static event Action<int, int> OnInitializeBoard;
        public static event Action<int, int, Cell[,]> OnFillEmptyCellsEnded;
        public static event Action<int, int, Cell[,]> OnBoardCreated;
        public static event Action<int, int, Cell[,]> OnClearRegionEnded;

        private int m_Rows;
        private int m_Columns;
        private int m_ColorsInGame;
        private Cell[,] m_Cells;
        private bool m_CanInteract;

        private void Awake()
        {
            PlayerInputManager.OnMouseClick += HandleOnMouseClick;
        }

        private void Start()
        {
            SetCanInteract(true);
            InitializeGameSettings();
            InitializeBoard();
            FillEmptyCells();

            OnBoardCreated?.Invoke(m_Rows, m_Columns, m_Cells);
        }

        private void OnDestroy()
        {
            PlayerInputManager.OnMouseClick -= HandleOnMouseClick;
        }


        private async void HandleOnMouseClick(Vector2 mousePosition)
        {
            if (!GetCanInteract()) return;
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (!hit.collider) return;

            if (hit.collider.TryGetComponent(out BlockCollision blockCollision))
            {
                var block = blockCollision.GetBlock();
                if (!block) return;

                SetCanInteract(false);
                await ClearRegionAsync(block.GetCell().GetRow(), block.GetCell().GetColumn());

                OnClearRegionEnded?.Invoke(m_Rows, m_Columns, m_Cells);
                SetCanInteract(true);
            }

        }


        private void SetCanInteract(bool canInteract)
        {
            m_CanInteract = canInteract;
        }

        private bool GetCanInteract()
        {
            return m_CanInteract;
        }

        private void InitializeGameSettings()
        {
            m_Rows = GameManager.GetRows();
            m_Columns = GameManager.GetColumns();
            m_ColorsInGame = GameManager.GetColorsInGame();
        }

        private void InitializeBoard()
        {
            m_Cells = new Cell[m_Rows, m_Columns];

            for (var row = 0; row < m_Rows; row++)
            {
                for (var col = 0; col < m_Columns; col++)
                {
                    var cell = Instantiate(_cellPrefab, _cellsTransform);
                    cell.SetPosition(row, col);
                    m_Cells[row, col] = cell;
                }
            }

            OnInitializeBoard?.Invoke(m_Rows, m_Columns);
        }

        private async UniTask ClearRegionAsync(int startRow, int startCol)
        {
            bool MatchCriteria(Cell cell) =>
                cell.GetBlock()?.GetColor() == m_Cells[startRow, startCol].GetBlock()?.GetColor();

            var matchedCells = FloodFill(startRow, startCol, MatchCriteria);

            if (matchedCells.Count >= 2)
            {
                await ClearMatchedCellsAsync(matchedCells);
            }
        }

        private async UniTask ClearMatchedCellsAsync(List<Cell> matchedCells)
        {
            foreach (var cell in matchedCells)
            {
                var block = cell.GetBlock();
                if (block)
                {
                    block.GetAnimation().DOScale(Vector3.zero, _blastBlockTime, Ease.InBack, () =>
                    {
                        _blockCreateManager.ReturnBlockToPool(block);
                    }).Forget();
                }

                cell.ClearBlock();
            }

            await UniTask.WaitForSeconds(_blastBlockTime);
            FillEmptyCells();
        }


        private void FillEmptyCells()
        {
            for (var col = 0; col < m_Columns; col++)
            {
                for (var row = 0; row < m_Rows; row++)
                {
                    if (m_Cells[row, col].GetBlock()) continue;

                    MoveBlocksDown(row, col);
                    CreateNewBlockIfEmpty(row, col);
                }
            }
            OnFillEmptyCellsEnded?.Invoke(m_Rows, m_Columns, m_Cells);
        }

        private void MoveBlocksDown(int row, int col)
        {
            for (var r = row + 1; r < m_Rows; r++)
            {
                if (!m_Cells[r, col].GetBlock()) continue;

                var block = m_Cells[r, col].GetBlock();

                m_Cells[row, col].SetBlock(block);
                m_Cells[r, col].ClearBlock();
                block.SetCell(m_Cells[row, col]);

                block.GetAnimation()
                    .DOMove(m_Cells[row, col].transform.position, _moveBlockDownTime, Ease.OutBounce)
                    .Forget();

                break;
            }
        }

        private void CreateNewBlockIfEmpty(int row, int col)
        {
            if (m_Cells[row, col].GetBlock()) return;

            var newBlock = _blockCreateManager.CreateRandomBlock(col, m_Cells);
            m_Cells[row, col].SetBlock(newBlock);
            newBlock.SetCell(m_Cells[row, col]);

            newBlock.GetAnimation()
                .DOMove(m_Cells[row, col].transform.position, 0.5f, Ease.OutBounce)
                .Forget();
        }

        public List<Cell> FloodFill(int startRow, int startCol, Func<Cell, bool> matchCriteria)
        {
            return FloodFillHelper.Execute(m_Cells, m_Rows, m_Columns, startRow, startCol, matchCriteria);
        }

        public int GetColorsInGame()
        {
            return m_ColorsInGame;
        }
    }
}