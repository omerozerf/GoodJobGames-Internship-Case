using System;
using Blocks;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public event Action<Block> OnBlockChanged;

    private int m_Row;
    private int m_Column;
    private Block m_Block;

    public void SetPosition(int row, int column)
    {
        SetRow(row);
        SetColumn(column);

        transform.position = new Vector3(column, row);
    }

    public void SetRow(int row)
    {
        m_Row = row;
    }

    public void SetColumn(int column)
    {
        m_Column = column;
    }

    public void SetBlock(Block block)
    {
        m_Block = block;

        if (block)
        {
            m_Block.transform.SetParent(transform);
        }

        OnBlockChanged?.Invoke(block);
    }

    public int GetRow()
    {
        return m_Row;
    }

    public int GetColumn()
    {
        return m_Column;
    }

    public Block GetBlock()
    {
        return m_Block;
    }

    public void ClearBlock()
    {
        SetBlock(null);
    }
}