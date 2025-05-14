using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class grid_scene2 : MonoBehaviour

   
{
    public int rows = 4;    // Number of rows in the grid
    public int columns = 6; // Number of columns in the grid


    public Transform gridParent;  // Parent object to hold the grid cells

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        // Calculate the cell size based on the grid size and layout properties
        float cellWidth = gridLayout.cellSize.x;
        float cellHeight = gridLayout.cellSize.y;

        // Calculate the parent size based on the grid size and layout properties
        float parentWidth = cellWidth * columns;
        float parentHeight = cellHeight * rows;

        RectTransform parentRectTransform = gridParent.GetComponent<RectTransform>();
        parentRectTransform.sizeDelta = new Vector2(parentWidth, parentHeight);

        // Create the grid cells
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject cell = new GameObject("Cell (" + row + ", " + column + ")");
                cell.transform.SetParent(gridParent);

                RectTransform cellRectTransform = cell.AddComponent<RectTransform>();
                cellRectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);

                Image cellImage = cell.AddComponent<Image>();
                cellImage.color = Color.white;
            }
        }
    }
}


