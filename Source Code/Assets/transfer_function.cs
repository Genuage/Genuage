using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class transfer_function : MonoBehaviour
{


    void Start()
    {
        Texture2D texture = new Texture2D(512, 1);
        Color[] colors = new Color[4];
        Color firstCol = colors[0];
        colors[0] = Color.red;
        colors[1] = Color.yellow;
        colors[2] = Color.blue;
        colors[3] = Color.green;
        float pas = 0;


        Vector2[] points = new Vector2[]
   {
     new Vector2( (float)0.0, (float)0.1 ),
   new Vector2( (float)0.1, (float)0.4 ),
   new Vector2( (float)0.4, (float)0.6 ),
   new Vector2( 1, 1 )

   };

     
       
            for (int x = 0; x < texture.width; x++)
            {


                for (int i = 0; i < points.Length-1; i++)
                {
                float newX = (float)x / 511;
                    if ((newX >= points[i].x)&& (newX < points[i+1].x))
                    {
                      
                        firstCol = Color.Lerp(colors[i], colors[i + 1], pas);
                        texture.SetPixel(x, 1, firstCol);
                         pas = (newX - points[i].x / (points[i+1].x - points[i].x))/100;
                        break;
                        
                    }
                    
                }




            }
        


        texture.Apply();
        //AssetDatabase.CreateAsset(texture, "Assets/LepText.asset");

    }

}
