using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellLayer : MonoBehaviour
{
    public Cell[,] cells;

    // Additional custom per-layer attributes
    // ...
    // ...
    // ...


    /// <summary>
    /// 
    /// </summary>
    public int rowCount;
    public int columnCount;
    public int cellCount;

    /// <summary>
    /// Start this instance.
    /// </summary>
    public void Start()
    {
        rowCount = cells.GetLength(0);
        columnCount = cells.GetLength(1);
        cellCount = cells.Length;
    }


    /// <summary>
    /// 
    /// </summary>
    public void Initialize(Cell cellPrefab, int rows, int columns)
    {
        // create cell array
        cells = new Cell[rows, columns];

        // instantiate cell prefabs and store in array
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Cell cell = Instantiate(cellPrefab,transform);

                cell.transform.localPosition = new Vector3(j, 0.0f, i);
                cells[i, j] = cell;
            }
        }
    }
}

