using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class create_list_points : MonoBehaviour
{


    public float widthMin = 0;
    public float widthMax = 0;
    public float heightMin = 0;
    public float heightMax = 0;
    public List<Vector3> points;
    public Material MR;
    private Transform parentTransform;
    public Texture2D texture;

    void Start()
    {
        points = new List<Vector3>();

    }


    public void textureStart()
    {

        widthMin = -gameObject.GetComponent<RectTransform>().rect.width / 2f;
        widthMax = gameObject.GetComponent<RectTransform>().rect.width / 2f;
        heightMin = -gameObject.GetComponent<RectTransform>().rect.height / 2f;
        heightMax = gameObject.GetComponent<RectTransform>().rect.height / 2f;

        float xRange = widthMax - widthMin;
        float yRange = heightMax - heightMin;

        texture = new Texture2D(512, 1);

        List<Vector2> newpoints = new List<Vector2>();
        newpoints.Add(new Vector2(widthMin, heightMin));
        for(int n = 0; n < points.Count; n++)
        {
            newpoints.Add(points[n]);
        }
        newpoints.Add(new Vector2(widthMax, heightMin));

        Color firstCol;

        int p_length = newpoints.Count;
        float pas = 0;
        for (int index = 0; index < texture.width; index++)
        {
            int i = 0;
            float newX = (float)index / 512;


            while (i < p_length - 1)
            {
                float pointx = (newpoints[i].x - widthMin) / xRange; //hardcoded
                float pointy = (newpoints[i].y - heightMin) / yRange;
                float pointx1 = (newpoints[i + 1].x - widthMin) / xRange;
                float pointy1 = (newpoints[i + 1].y - heightMin) / yRange;

                if (newX >= pointx && newX < pointx1)
                {

                    Color color1 = new Color(1, 1, 1, pointy);
                    Color color2 = new Color(1, 1, 1, pointy1);
                    pas = (newX - pointx) / (pointx1 - pointx);
                    firstCol = Color.Lerp(color1, color2, pas);
                    texture.SetPixel(index, 0, firstCol);
                    i = i + 1;

                }
                else
                    i = i + 1;

            }
        }


        texture.Apply();
        System.DateTime dt = System.DateTime.Now;
        dt = dt.Add(System.TimeSpan.FromSeconds(180));

        var offset = System.DateTimeOffset.Now.Offset;
        string dateString = dt.ToString("yyyy-MM-dd_HH_mm_ss");
        string dest = "Assets/2DTexture" + dateString + ".asset";
        //    AssetDatabase.CreateAsset(texture, dest);
        MR.SetTexture("_TransferFunctionTex", texture);
        //Debug.Log("texture created");

    }

    bool Bounded(Vector3 mousePosition, Vector3 LowerCorner, Vector3 HigherCorner)
    {


        if ((mousePosition.x > LowerCorner.x) && (mousePosition.y > LowerCorner.y))
        {
            if ((mousePosition.x < HigherCorner.x) && (mousePosition.y < HigherCorner.y))
            {

                return true;
            }


        }
        return false;

    }
    public void StartSort()
    {


        points.Sort(CompareByX);


       
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
    public void changeP(int instanceID, float newX, float newY)
    { 
        Vector3 localPosition = new Vector3(newX, newY, 0);
        localPosition.z = instanceID;
        for (int a = 0; a < points.Count; a++)
        {

            if (points[a].z == instanceID)
                points[a] = localPosition;
        }
    }
    float Normalize(float value, float min, float max, float newMin, float newMax)
    {
        float normalizedValue = (value - min) / (max - min);
        float newValue = (normalizedValue * (newMax - newMin)) + newMin;
        return newValue;
    }
}

