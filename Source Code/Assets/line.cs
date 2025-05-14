using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.UI;

public class line : MonoBehaviour
{
    public RectTransform graphContainer;
    public Sprite pointSprite;

    void Start()
    {
        // Add points to the graph
        AddPoint(0, 0);
        AddPoint(1, 1);
        AddPoint(2, 2);
        AddPoint(5, 5);
        AddPoint(10, 10);
    }

    void AddPoint(float x, float y)
    {
        // Create a new game object for the point
        GameObject pointObj = new GameObject("Point", typeof(Image));
        pointObj.transform.SetParent(graphContainer, false);

        // Set the sprite for the point
        Image pointImage = pointObj.GetComponent<Image>();
        pointImage.sprite = pointSprite;

        // Set the position of the point based on the x and y values
        RectTransform pointTransform = pointObj.GetComponent<RectTransform>();
        pointTransform.anchoredPosition = new Vector2(x, y);
        pointTransform.sizeDelta = new Vector2(10, 10);
    }
}

