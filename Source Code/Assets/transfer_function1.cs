using Grafic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class transfer_function1 : MonoBehaviour
{
    
    List<Vector3> points = new List<Vector3>();
    public Material MR;

  
    public void AddPoints(float x, float y)
    {
        points.Add(new Vector2(x, y));


    }
    public void changeP(float b, float y, float newX, float newY)
    {
        for (int a = 0; a < points.Count; a++)
        {
            if (points[a].x == b && points[a].y == y)
            {
                points[a].Set(newX, newY,points[a].z);


            }

        }

    }
    public void RemovePoint(float x, float y)
    {

        for (int a = 0; a < points.Count; a++)
        {
            if (points[a].x == x && points[a].y == y)
            {
                points.RemoveAt(a);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Can't find point");
            }

        }
    }
    public void textureStart()
    {
        Grafic.line_draw l = FindObjectOfType<line_draw>();
        points = l.points;
        Texture2D texture = new Texture2D(512, 1);
        //   Color[] colors = new Color[3];
        Color firstCol;
        //    colors[0] = new Color((float)0,0,0,1);
        //   colors[1] = new Color((float)0.02, (float)0, (float)0, 1);
        //     colors[2] = new Color((float)0.2, (float)0, (float)0, 1);

        //   colors[3] = new Color((float)0.8, 0, 0, 1);
        // colors[4] = new Color((float)1, (float)1, (float)0.8, 1);
        int p_length = points.Count;
        float pas = 0;
        for (int index = 0; index < texture.width; index++)
        {
            int i = 0;
            float newX = (float)index / 511;
            while (i < p_length - 1)
            {
                if (newX >= points[i].x && newX < points[i + 1].x)
                {
                    Color color1 = new Color(points[i].y, 0, 0, 1);
                    Color color2 = new Color(points[i + 1].y, 0, 0, 1);
                    pas = (newX - points[i].x) / (points[i + 1].x - points[i].x);
                    firstCol = Color.Lerp(color1, color2, pas);
                    texture.SetPixel(index, 0, firstCol);
                    i++;
                }
                else
                    i++;

            }
        }

        texture.Apply();
        System.DateTime dt = System.DateTime.Now;
        dt = dt.Add(System.TimeSpan.FromSeconds(180));

        var offset = System.DateTimeOffset.Now.Offset;
        string dateString = dt.ToString("yyyy-MM-dd_HH_mm_ss");
      string dest = "Assets/2DTexture"+dateString+".asset";
        Debug.Log(texture);
        Debug.Log(MR);

        //AssetDatabase.CreateAsset(texture, dest);

        MR.SetTexture("_ShaderTex", texture);


    }

    void StartSort()
    {
        Grafic.line_draw l = FindObjectOfType<line_draw>();
        points = l.points;


        points.Sort(CompareByX);


        foreach (Vector2 point in points)
        {
            Debug.Log(point);
        }
    }

    private int CompareByX(Vector3 a, Vector3 b)
    {
        if (a.x < b.x)
        {
            return -1;
        }
        else if (a.x > b.x)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}




