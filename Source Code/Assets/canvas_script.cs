using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvas_script : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    private RectTransform graph;

     void Start()
    {
        graph = transform.Find("graph").GetComponent<RectTransform>();
        points(new Vector2(200, 200));
        List<int> list = new List<int>() { 5, 3, 45, 5, 4 };
        showGraph(list);
        
    }

   private GameObject points(Vector2 position)
    {
        GameObject point = new GameObject("circle", typeof(Image));
        point.transform.SetParent(graph, false);
        point.GetComponent<Image>().sprite = sprite;
        RectTransform recttr = point.GetComponent<RectTransform>();
        recttr.anchoredPosition = position;
        recttr.sizeDelta = new Vector2(11, 11);
        recttr.anchorMin = new Vector2(0, 0);
        recttr.anchorMax = new Vector2(0, 0);
        return point;
    }
    private void showGraph(List <int> list)
    {
        float height = graph.sizeDelta.y;
        float yMaximum = 100;
        float xSize = 50;
        GameObject lastCircle = null;
        for (int i=0;i<list.Count;i++)
        {
            float xPosition = i * xSize;
            float yPosition = (list[i] / yMaximum)*height;
            GameObject circle= points(new Vector2(xPosition, yPosition));
            if (lastCircle!=null)
            {
                DotConnection(lastCircle.GetComponent<RectTransform>().anchoredPosition, circle.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircle = circle;

        }

    }
    private void DotConnection(Vector2 x, Vector2 y)
    {
        GameObject connection = new GameObject("dotConnection", typeof(Image));
        connection.transform.SetParent(graph, false);
        connection.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        RectTransform recttr = connection.GetComponent<RectTransform>();
        Vector2 dir = (y - x).normalized;
        float distance = Vector2.Distance(x, y);
        recttr.sizeDelta = new Vector2(distance, 3);
        recttr.anchorMin = new Vector2(0, 0);
        recttr.anchorMax = new Vector2(0, 0);
        Vector2 vector2 = dir * distance;
        vector2.x = (float)0.5 * vector2.x;
        vector2.y = (float)0.5 * vector2.y;
        recttr.anchoredPosition = x + vector2;
   
        

    }


}
    

