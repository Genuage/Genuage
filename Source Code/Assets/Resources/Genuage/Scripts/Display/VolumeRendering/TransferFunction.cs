using UnityEngine;
using System.Collections.Generic;
using System;


public class TransferFunction : MonoBehaviour
{

    [SerializeField]
    public List<TFColourControlPoint> colourControlPoints = new List<TFColourControlPoint>();
    [SerializeField]
    public List<TFAlphaControlPoint> alphaControlPoints = new List<TFAlphaControlPoint>();

    private Texture2D texture = null;
    private Texture2D OpacityTexture = null; //Texture used for the path tracing module

    private Color[] tfCols;
    private Color[] opacityTFCols;

    private const int TEXTURE_WIDTH = 512;
    private const int TEXTURE_HEIGHT = 1;

    public void AddControlPoint(TFColourControlPoint ctrlPoint)
    {
        colourControlPoints.Add(ctrlPoint);
    }

    public void AddControlPoint(TFAlphaControlPoint ctrlPoint)
    {
        alphaControlPoints.Add(ctrlPoint);
    }


    public Texture2D GetTexture()
    {
        if (texture == null)
            GenerateTexture();

        return texture;
    }

    public Texture2D GetOpacityTFTexture()
    {
        if (OpacityTexture == null)
            GenerateTexture();

        return OpacityTexture;
    }

    public void GenerateTexture()
    {
        if (texture == null)
            CreateTexture();

        List<TFColourControlPoint> cols = new List<TFColourControlPoint>(colourControlPoints);
        List<TFAlphaControlPoint> alphas = new List<TFAlphaControlPoint>(alphaControlPoints);

        // Sort lists of control points
        cols.Sort((a, b) => (a.dataValue.CompareTo(b.dataValue)));
        alphas.Sort((a, b) => (a.dataValue.CompareTo(b.dataValue)));

        // Add colour points at beginning and end
        if (cols.Count == 0 || cols[cols.Count - 1].dataValue < 1.0f)
            cols.Add(new TFColourControlPoint(1.0f, Color.white));
        if (cols[0].dataValue > 0.0f)
            cols.Insert(0, new TFColourControlPoint(0.0f, Color.white));

        // Add alpha points at beginning and end
        if (alphas.Count == 0 || alphas[alphas.Count - 1].dataValue < 1.0f)
            alphas.Add(new TFAlphaControlPoint(1.0f, 1.0f));
        if (alphas[0].dataValue > 0.0f)
            alphas.Insert(0, new TFAlphaControlPoint(0.0f, 0.0f));

        int numColours = cols.Count;
        int numAlphas = alphas.Count;
        int iCurrColour = 0;
        int iCurrAlpha = 0;

        for (int iX = 0; iX < TEXTURE_WIDTH; iX++)
        {
            float t = iX / (float)(TEXTURE_WIDTH - 1);
            while (iCurrColour < numColours - 2 && cols[iCurrColour + 1].dataValue < t)
                iCurrColour++;
            while (iCurrAlpha < numAlphas - 2 && alphas[iCurrAlpha + 1].dataValue < t)
                iCurrAlpha++;

            TFColourControlPoint leftCol = cols[iCurrColour];
            TFColourControlPoint rightCol = cols[iCurrColour + 1];
            TFAlphaControlPoint leftAlpha = alphas[iCurrAlpha];
            TFAlphaControlPoint rightAlpha = alphas[iCurrAlpha + 1];

            float tCol = (Mathf.Clamp(t, leftCol.dataValue, rightCol.dataValue) - leftCol.dataValue) / (rightCol.dataValue - leftCol.dataValue);
            float tAlpha = (Mathf.Clamp(t, leftAlpha.dataValue, rightAlpha.dataValue) - leftAlpha.dataValue) / (rightAlpha.dataValue - leftAlpha.dataValue);

            tCol = Mathf.SmoothStep(0.0f, 1.0f, tCol);
            tAlpha = Mathf.SmoothStep(0.0f, 1.0f, tAlpha);

            Color pixCol = rightCol.colourValue * tCol + leftCol.colourValue * (1.0f - tCol);
            pixCol.a = rightAlpha.alphaValue * tAlpha + leftAlpha.alphaValue * (1.0f - tAlpha);

            for (int iY = 0; iY < TEXTURE_HEIGHT; iY++)
            {
                tfCols[iX + iY * TEXTURE_WIDTH] = pixCol;
                opacityTFCols[iX + iY * TEXTURE_WIDTH] = new Color(pixCol.a, 0f, 0f, 255f); //TODO : CHANGE A TO R and R to 255
            }
        }

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(tfCols);
        texture.Apply();

        OpacityTexture.wrapMode = TextureWrapMode.Clamp;
        OpacityTexture.SetPixels(opacityTFCols);
        OpacityTexture.Apply();
    }

    private void CreateTexture()
    {
        TextureFormat texformat = TextureFormat.RGBAFloat;
        texture = new Texture2D(TEXTURE_WIDTH, TEXTURE_HEIGHT, texformat, false);
        tfCols = new Color[TEXTURE_WIDTH * TEXTURE_HEIGHT];

        OpacityTexture = new Texture2D(TEXTURE_WIDTH, TEXTURE_HEIGHT, texformat, false);
        opacityTFCols = new Color[TEXTURE_WIDTH * TEXTURE_HEIGHT];
    }

}


[System.Serializable]
public struct TFColourControlPoint
{
    public float dataValue;
    public Color colourValue;

    public TFColourControlPoint(float dataValue, Color colourValue)
    {
        this.dataValue = dataValue;
        this.colourValue = colourValue;
    }
}

[System.Serializable]
public struct TFAlphaControlPoint
{
    public float dataValue;
    public float alphaValue;

    public TFAlphaControlPoint(float dataValue, float alphaValue)
    {
        this.dataValue = dataValue;
        this.alphaValue = alphaValue;
    }
}