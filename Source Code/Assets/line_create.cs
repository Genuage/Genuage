using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class line_create : MonoBehaviour
{

    public  LineRenderer lineRenderer;
    private RawImage[] rawImages;
    public GameObject lineRendererObject;
    private string lineRendererName="Sphere";

    public void line()
    {

         rawImages = FindObjectsOfType<RawImage>();
        lineRenderer = FindLineRendererByName(lineRendererName);

        if (rawImages.Length >= 2)
        {

            lineRenderer.positionCount = rawImages.Length;
            Debug.Log(lineRenderer.positionCount);
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            Debug.Log("da");
            for (int i = 0; i < rawImages.Length; i++)
            {
                lineRenderer.SetPosition(i, rawImages[i].transform.position);
                Debug.Log("a intrat in for");
            }
        }

    }


    LineRenderer FindLineRendererByName(string name)
    {

        LineRenderer[] lineRenderers = FindObjectsOfType<LineRenderer>();

        foreach (LineRenderer renderer in lineRenderers)
        {
            if (renderer.gameObject.name == name)
            {
                return renderer;
            }
        }

        return null;
    }
}


