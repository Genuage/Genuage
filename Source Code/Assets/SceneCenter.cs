using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneCenter : MonoBehaviour
{

    public RectTransform DragTransform;
    Bounds getSceneSize()
    {
        float max_x = -Mathf.Infinity;
        float min_x = Mathf.Infinity;

        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer sprite in allSprites)
        {
            float right_x = sprite.bounds.max.x;
            float left_x = sprite.bounds.min.x;
            if (right_x > max_x)
            {
                max_x = right_x;
            }
            if (left_x < min_x)
            {
                min_x = left_x;
            }
        }

        Bounds b = new Bounds();
        b.min = new Vector3(min_x, 0, 0);
        b.max = new Vector3(max_x, 0, 0);
        return b;
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragTransform.anchoredPosition += eventData.delta / DesktopApplication.instance.Canvas.GetComponent<Canvas>().scaleFactor;
    }

    void Start()
    {
        Bounds dimensions;
        dimensions = getSceneSize();

        Debug.Log("Scene Dimensions: " + dimensions);
    
    }
}
