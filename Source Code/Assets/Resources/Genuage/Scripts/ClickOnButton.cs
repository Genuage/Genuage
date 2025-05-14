using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using itk.simple;
using Data;
using IO;
using DesktopInterface;
using Vector3 = System.Numerics.Vector3;

public class ClickOnButton : MonoBehaviour
{
    public Material material;
    public Button FirstButton;
    public GameObject VolumeObjectPrefab;
    public TransferFunction TF;

    private string inputImageFileName = "";
    void Start()
    {
        FirstButton.onClick.AddListener(ButtonClicked);
    }

    void ButtonClicked()
    {
        var extensions = new[]
        {
            new ExtensionFilter("TIF file", "tif")
        };
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, true, (string[] paths) => { ImageStackLoader.instance.LoadFromFile(paths); });
    }

    private void LoadTIFF(string[] paths)
    {
        ImageFileReader reader = new ImageFileReader();
        reader.SetImageIO("TIFFImageIO");
        reader.SetFileName(paths[0]);
        itk.simple.Image image = reader.Execute();
        int width = (int)image.GetWidth();
        int height = (int)image.GetHeight();
        int layers = (int)image.GetDepth();
        int ind = 0;

        Color[] colors1 = new Color[width * height * layers];
        Vector3 dimensions = new Vector3(width, height, layers);
        Color[] colors = new Color[width * height * layers];
        Texture3D texture = new Texture3D(width, height, layers, TextureFormat.RGBA32, false);
        MinimumMaximumImageFilter minMaxFilter = new MinimumMaximumImageFilter();
        minMaxFilter.Execute(image);
        double max = minMaxFilter.GetMaximum();
        double min = minMaxFilter.GetMinimum();
        int ind1 = 0;
        Texture3D texture1 = new Texture3D(width, height, layers, TextureFormat.RGBA32, false);

        int cptpos = 0;
        int cptneg = 0;

        for (uint i = 0; i < layers; i++)
        {
            for (uint j = 0; j < height; j++)
            {


                for (uint k = 0; k < width; k++)
                {


                    VectorUInt32 poz = new VectorUInt32(new uint[] { k, j, i });//index


                    ushort pix2 = image.GetPixelAsUInt16(poz);

                    double colorValue = ((double)pix2 - min) / (max - min);
                    colors[ind] = new Color((float)colorValue, (float)colorValue, (float)colorValue, 1);
                    ind++;
                    if ((k >= 1) && (j >= 1) && (i >= 1) && (i < (layers - 1)) && (k < (width - 1)) && (j < (height - 1)))
                    {

                        VectorUInt32 poz1 = new VectorUInt32(new uint[] { k - 1, j, i });//index linie-1
                        VectorUInt32 poz2 = new VectorUInt32(new uint[] { k + 1, j, i });//index linie+1
                        VectorUInt32 poz3 = new VectorUInt32(new uint[] { k, j - 1, i });//index coloana-1
                        VectorUInt32 poz4 = new VectorUInt32(new uint[] { k, j + 1, i });//index coloana+1
                        VectorUInt32 poz8 = new VectorUInt32(new uint[] { k, j, i - 1 });//index linie+1
                        VectorUInt32 poz9 = new VectorUInt32(new uint[] { k, j, i + 1 });//index coloana+1
                        ushort pix3 = image.GetPixelAsUInt16(poz1);
                        ushort pix4 = image.GetPixelAsUInt16(poz3);
                        ushort pix5 = image.GetPixelAsUInt16(poz2);
                        ushort pix6 = image.GetPixelAsUInt16(poz4);
                        ushort pix7 = image.GetPixelAsUInt16(poz8);
                        ushort pix8 = image.GetPixelAsUInt16(poz9);
                       double  Value1 = (pix3 - pix5);
                       float Value_1 = (float)((Value1 - min) / (max - min));
                       double Value2 = (pix4 - pix6);
                       float Value_2 = (float)((Value2 - min) / (max - min));
                       double Value3 = (pix7 - pix8);
                       float Value_3 = (float)((Value3 - min) / (max - min));

                        colors1[ind1] = new Color(Value_1, Value_2, Value_3, 1.0f);

                        ind1++;
                        cptpos++;

                    }
                    else
                    {
      
                        colors1[ind1].a=0;
                        ind1++;

                        cptneg++;


                    }




                }
            }
            

            



        }
        texture1.SetPixels(colors1);
        texture1.Apply();
        texture.SetPixels(colors);
        texture.Apply();
        Debug.Log("positive loop : " + cptpos);
        Debug.Log("negative loop : " + cptneg);

        //AssetDatabase.CreateAsset(texture1, "Assets/3DTexture_gradient_inc.asset");
        material.SetTexture("_VolumeTex", texture);
        //AssetDatabase.CreateAsset(texture, "Assets/3DTexture_inc.asset");
        material.SetTexture("_GradientTex", texture1);

        GameObject GO = Instantiate(VolumeObjectPrefab) as GameObject;
        //TransferFunctionUI.ShowWindow(TF);
        material.SetTexture("_TransferFunctionTex", TF.GetTexture());
        //CloudUpdater.instance.ActivateVolumeRendering(GO);

    }


}

