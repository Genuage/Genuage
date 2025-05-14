using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Grafic
{
    public class line_draw : UnityEngine.UI.Graphic
    {
        public Vector2Int gridSize;
        public float thickness;
        public pointmanager2 pointmanager;
        public List<Vector3> points;
  
        float width;
        float height;
        float unitWidth;
        float unitHeight;

        public int widthMin = 150;
        public int widthMax = 1125;
        public int heightMin = 50;
        public int heightMax = 120;

        public List<Vector2> newpoints;

        public void MeshCreater()
        {
            newpoints = new List<Vector2>();
            newpoints.Add(new Vector2(widthMax, heightMin));
            for (int n = 0; n < points.Count; n++)
            {
                newpoints.Add(points[n]);
            }

            for (int n = 0; n < newpoints.Count; n++)
            {
                float pointx = (newpoints[n].x - widthMax) / widthMin; //hardcoded
                float pointy = (newpoints[n].y - heightMin) / heightMax;
                if (pointx < 0) { pointx = 0; }
                if (pointy < 0) { pointy = 0; }
                if (pointx > 1) { pointx = 1; }
                if (pointy > 1) { pointy = 1; }
                newpoints[n] = new Vector2(pointx, pointy);
            }
            using (var vh = new VertexHelper())
            {

                OnPopulateMesh(vh);



            }
        }


        protected override void OnPopulateMesh(VertexHelper vh)
        {
            

            vh.Clear();

            width = rectTransform.rect.width;
            height = rectTransform.rect.height;

            unitWidth = width/ gridSize.x;
            unitHeight = height / gridSize.y;

            if (newpoints.Count < 2) return;

            Debug.Log("Pop:Esh");

            float angle = 0;
            for (int i = 0; i < newpoints.Count - 1; i++)
            {

                Vector2 point =new Vector2(newpoints[i].x, newpoints[i].y);
               

                Vector2 point2 = new Vector2(newpoints[i+1].x, newpoints[i+1].y);




                if (i < newpoints.Count - 1)
                { 

                    angle = GetAngle(point, point2) + 90f;
                }

                DrawVerticesForPoint(point, point2, angle, vh);
            }
  

            for (int i = 0; i < newpoints.Count - 1; i++)
            {
                int index = i * 4;
                vh.AddTriangle(index + 0, index + 1, index + 2);
                vh.AddTriangle(index + 1, index + 2, index + 3);
            }


        }


        public float GetAngle(Vector2 me, Vector2 target)
        {


            return (float)(Mathf.Atan2(9f * (target.y - me.y), 16f * (target.x - me.x)) * (180 / Mathf.PI));
        }
        void DrawVerticesForPoint(Vector2 point, Vector2 point2, float angle, VertexHelper vh)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
            vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
            vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
            vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
            vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
            vh.AddVert(vertex);
        }


    }
}