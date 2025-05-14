using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Grafic { 
public class textures : MonoBehaviour
{
    private Texture2D texture;
        public List<Vector3> points;

    // Start is called before the first frame update
        void Start()
    {       line_draw l;
            l = FindObjectOfType<line_draw>();
            points = new List<Vector3>();
            points = l.points;

           texture = new Texture2D(512, 1);
            //  Color[] colors = new Color[5];
              Color firstCol;
            //   colors[0] = new Color((float)0, 0, 0, 1);
            //  colors[1] = new Color((float)0.02, (float)0, (float)0, 1);
            //   colors[2] = new Color((float)0.05, (float)0, (float)0, 1);
            //   colors[3] = new Color((float)0.1, (float)0, (float)0, 1);
            //   colors[4] = new Color((float)0.2, (float)0, (float)0, 1);
            //   //colors[5] = new Color((float)0.8, 0, 0, 1);
            //     colors[6] = new Color((float)1, (float)1, (float)0.8, 1);
            //  int c_length = colors.Length;
            int points_l = points.Count;
            float pas = 0;
            for (int x = 0; x < texture.width; x++)
            {
                int i = 0;
                float newX = (float)x / 511;
                while (i < (points_l - 1))
                {
                    if((newX >= (points[i].x / 7)) && (newX < (points[i + 1].x / 7)))
                    {
                        Debug.Log(points[i]+" i="+i);
                        pas = (newX - points[i].x) / (points[i + 1].x - points[i].x);
                        firstCol = Color.Lerp(new Color(points[i].y/25,0,0), new Color(points[i+1].y/25, 0, 0), pas);
                        texture.SetPixel(x, 0, firstCol);
                        i++;
                    }
                    else
                        i++;

                }
            }
            texture.Apply();
            //AssetDatabase.CreateAsset(texture, "Assets/2DTexture6.asset");



        }
    }

    }