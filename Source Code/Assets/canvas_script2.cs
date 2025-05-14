using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvas_script2 : UnityEngine.UI.Graphic
{
    public float thickness = 10;
    public Vector2Int gridSize = new Vector2Int(1, 1);
    float width;
    float height;
    float cellWidth;
    float cellHeight;
    public void MeshCreater()
    {
        using (var vh = new VertexHelper())
        {
            OnPopulateMesh(vh);
        }
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
   
        cellWidth = width * 0.178f / (float)gridSize.x;
        cellHeight = height * 0.07f / (float)gridSize.y;
        int count = 0;
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                DrawCell(x, y, count, vh);
                count++;
            }
        }
    }
    private void DrawCell(int x, int y, int index,VertexHelper vh)
    {
        float scale = 2f;
        float xPos = cellWidth * x;
        float yPos = cellHeight * y;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        vertex.position = new Vector3(xPos, yPos);
        vh.AddVert(vertex);
        vertex.position = new Vector3(xPos,yPos+ cellHeight);
        vh.AddVert(vertex);
        vertex.position = new Vector3(xPos+cellWidth, yPos+cellHeight);
        vh.AddVert(vertex);
        vertex.position = new Vector3(xPos+cellWidth, yPos);

        vh.AddVert(vertex);

        float widthSqr = thickness * thickness;
        float distanceSqr = widthSqr / 2f; 
        float distance = Mathf.Sqrt(distanceSqr);
        vertex.position = new Vector3(xPos+distance, yPos+distance);
        vh.AddVert(vertex);
        vertex.position = new Vector3(xPos+ distance, yPos+(cellHeight - distance));
        vh.AddVert(vertex);
        vertex.position = new Vector3(xPos+(cellWidth - distance),yPos+( cellHeight - distance));
        vh.AddVert(vertex);
        vertex.position = new Vector3(xPos+(cellWidth - distance),yPos+ distance);
        vh.AddVert(vertex);
        int offset = index * 8;
        vh.AddTriangle(offset+0, offset + 1, offset + 5);
        vh.AddTriangle(offset + 5, offset + 4, offset + 0);

        vh.AddTriangle(offset + 1, offset + 2, offset + 6);
        vh.AddTriangle(offset + 6, offset + 5, offset + 1);

        vh.AddTriangle(offset + 2, offset + 3, offset + 7);
        vh.AddTriangle(offset + 7, offset + 6, offset + 2);

        vh.AddTriangle(offset + 3, offset + 0, offset + 4);
        vh.AddTriangle(offset + 4, offset + 7, offset + 3);
        vh.AddTriangle(offset + 4, offset + 7, offset + 3);


    }

   
}