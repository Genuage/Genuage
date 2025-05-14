using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DesktopInterface;
using itk.simple;
using Valve.VR.InteractionSystem;
using Data;
//using static UnityEditor.ShaderGraph.ShaderGenerator;

namespace IO
{

    public class ImageStackLoader : MonoBehaviour
    {

        private const int DEFAULT_ID = 0;
        private const string DEFAULT_NAME = "Volume";

        private itk.simple.Image Image;
        private string filepath;
        private float[] pixelData;
        private Vector3Int dimensions;
        private Vector3 scales;

        public Material material;
        public GameObject VolumeObjectPrefab;

        public static ImageStackLoader instance;
        public TransferFunction TF;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadFromFile(string[] file_paths)
        {
            string filepath = file_paths[0];
            UIManager.instance.ChangeStatusText("Loading Cloud...");

            Image = SimpleITK.ReadImage(filepath);

            // Cast to 32-bit float
            Image = SimpleITK.Cast(Image, PixelIDValueEnum.sitkFloat32);
            VectorUInt32 size = Image.GetSize();
            int numPixels = 1;
            uint dimensionNBR = Image.GetDimension();
            Debug.Log("Loaded image set has " + dimensionNBR + "dimensions");
            for (int dimension = 0; dimension < dimensionNBR; dimension++)
            {
                numPixels *= (int)size[dimension];
            }
            Debug.Log("Total Number of Pixels : " + numPixels);

            // Read pixel data
            pixelData = new float[numPixels];
            IntPtr imgBuffer = Image.GetBufferAsFloat();
            Marshal.Copy(imgBuffer, pixelData, 0, numPixels);

            //TODO : Determine if clamping pixels is necessary
            //for (int i = 0; i < pixelData.Length; i++)
            //	pixelData[i] = Mathf.Clamp(pixelData[i], -1024, 3071);

            VectorDouble spacing = Image.GetSpacing();


            //FixDimensions(pixelData, (int)size[0], (int)size[1], (int)size[2]);

            dimensions = new Vector3Int((int)size[0], (int)size[1], (int)size[2]);
            scales = new Vector3((float)(spacing[0] * size[0]), (float)(spacing[1] * size[1]), (float)(spacing[2] * size[2]));
            Debug.Log("Size of dimension x : " + dimensions.x);
            Debug.Log("Size of dimension y : " + dimensions.y);
            Debug.Log("Size of dimension z : " + dimensions.z);
            Texture3D DataTexture = CreateDataTexture(pixelData, dimensions.x, dimensions.y, dimensions.z);
            Texture3D GradientTexture = CreateGradientTexture(pixelData, dimensions.x, dimensions.y, dimensions.z);

            material.SetTexture("_VolumeTex", DataTexture);
            //AssetDatabase.CreateAsset(texture, "Assets/3DTexture_inc.asset");
            material.SetTexture("_GradientTex", GradientTexture);
            GameObject GO = Instantiate(VolumeObjectPrefab) as GameObject;
            
            VolumeData vdata = new VolumeData();
            float minValue = GetMinDataValue(pixelData, dimensions.x, dimensions.y, dimensions.z);
            float maxValue = GetMaxDataValue(pixelData, dimensions.x, dimensions.y, dimensions.z);
            vdata.MinValue = minValue;
            vdata.MaxValue = maxValue;
            vdata.PixelNumber = pixelData.Length;
            vdata.pixelData = pixelData;
            vdata.dataTexture = DataTexture;
            TransferFunctionUI.ShowWindow(TF, vdata);

            material.SetTexture("_TransferFunctionTex", TF.GetTexture());

            CloudUpdater.instance.ActivateVolumeRendering(GO, dimensions);

        }
        public Texture3D CreateDataTexture(float[] pixelData, int dimensionX, int dimensionY, int dimensionZ)
        {
            //TextureFormat texformat = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf) ? TextureFormat.RGBAHalf : TextureFormat.RGBAFloat;
            TextureFormat texformat = TextureFormat.RGBAFloat;

            Texture3D ImageTexture = new Texture3D(dimensionX, dimensionY, dimensionZ, texformat, false);
            ImageTexture.wrapMode = TextureWrapMode.Clamp;

            float minValue = GetMinDataValue(pixelData, dimensionX, dimensionY, dimensionZ);
            float maxValue = GetMaxDataValue(pixelData, dimensionX, dimensionY, dimensionZ);
            float maxRange = maxValue - minValue;
            //bool isHalfFloat = texformat == TextureFormat.RHalf;

            //try
            //{
            //	// Create a byte array for filling the texture. Store has half (16 bit) or single (32 bit) float values.
            //	int sampleSize = isHalfFloat ? 2 : 4;
            //	byte[] bytes = new byte[pixelData.Length * sampleSize]; // This can cause OutOfMemoryException
            //	Debug.Log(bytes.Length);
            //	for (int iData = 0; iData < pixelData.Length; iData++)
            //	{
            //		float pixelValue = (float)(pixelData[iData] - minValue) / maxRange;
            //		byte[] pixelBytes = isHalfFloat ? BitConverter.GetBytes(Mathf.FloatToHalf(pixelValue)) : BitConverter.GetBytes(pixelValue);

            //		Array.Copy(pixelBytes, 0, bytes, iData * sampleSize, sampleSize);
            //	}
            //	Debug.Log(pixelData.Length);
            //	Debug.Log(ImageTexture.GetPixels().Length);
            //	ImageTexture.SetPixelData(bytes, 0);
            //}
            //catch(OutOfMemoryException ex)
            //{
            //Debug.LogWarning("Out of memory when creating texture. Using fallback method.");
            Debug.Log("min : " + minValue + " max : " + maxValue + " mR : " + maxRange);

            Debug.Log(pixelData.Length);
            Debug.Log(dimensionX);
            Debug.Log(dimensionY);
            Debug.Log(dimensionZ);
            Debug.Log(dimensionX * dimensionY * dimensionZ);
            Color[] colors = new Color[dimensionX * dimensionY * dimensionZ];

            int counter = 0;

            for (int z = 0; z < dimensionZ; z++)
            {


                for (int y = 0; y < dimensionY; y++)
                {
                    for (int x = 0; x < dimensionX; x++)
                    {
                        //float val = (float)(pixelData[x + y * dimensionX + z * (dimensionX * dimensionY)] - minValue) / maxRange;
                        colors[counter] = new Color((float)(pixelData[x + (y * dimensionX) + (z * dimensionX * dimensionY)] - minValue) / maxRange,
                                                    (float)(pixelData[x + (y * dimensionX) + (z * dimensionX * dimensionY)] - minValue) / maxRange,
                                                    (float)(pixelData[x + (y * dimensionX) + (z * dimensionX * dimensionY)] - minValue) / maxRange, 1.0f);
                        counter++;
                        //Debug.Log("pixelvalue = "+ pixelData[x + y * dimensionX + z * (dimensionX * dimensionY)]+ "finalvalue = " +val);
                    }
                }
            }
            Debug.Log(counter);
            ImageTexture.SetPixels(colors);
            ImageTexture.Apply();
            //Debug.Log(ImageTexture);
            return ImageTexture;
        }

        public Texture3D CreateGradientTexture(float[] pixelData, int dimensionX, int dimensionY, int dimensionZ)
        {
            TextureFormat texformat = TextureFormat.RGBAFloat;

            Texture3D GradientTexture = new Texture3D(dimensionX, dimensionY, dimensionZ, texformat, false);
            GradientTexture.wrapMode = TextureWrapMode.Clamp;

            float minValue = GetMinDataValue(pixelData, dimensionX, dimensionY, dimensionZ);
            float maxValue = GetMaxDataValue(pixelData, dimensionX, dimensionY, dimensionZ);
            float maxRange = maxValue - minValue;
            Color[] colors = new Color[dimensionX * dimensionY * dimensionZ];
            int counter = 0;
            Color[] negativecolors;


            for (int z = 0; z < dimensionZ; z++)
            {


                for (int y = 0; y < dimensionY; y++)
                {
                    for (int x = 0; x < dimensionX; x++)
                    {
                        int iData = x + y * dimensionX + z * (dimensionX * dimensionY);

                        float x1 = pixelData[Math.Min(x + 1, dimensionX - 1) + y * dimensionX + z * (dimensionX * dimensionY)] - minValue;
                        float x2 = pixelData[Math.Max(x - 1, 0) + y * dimensionX + z * (dimensionX * dimensionY)] - minValue;
                        float y1 = pixelData[x + Math.Min(y + 1, dimensionY - 1) * dimensionX + z * (dimensionX * dimensionY)] - minValue;
                        float y2 = pixelData[x + Math.Max(y - 1, 0) * dimensionX + z * (dimensionX * dimensionY)] - minValue;
                        float z1 = pixelData[x + y * dimensionX + Math.Min(z + 1, dimensionZ - 1) * (dimensionX * dimensionY)] - minValue;
                        float z2 = pixelData[x + y * dimensionX + Math.Max(z - 1, 0) * (dimensionX * dimensionY)] - minValue;

                        Vector3 grad = new Vector3((x2 - x1) / maxRange, (y2 - y1) / maxRange, (z2 - z1) / maxRange);


                        colors[counter] = new Color(grad.x, grad.y, grad.z, (float)(pixelData[iData] - minValue) / maxRange);

                        counter++;
                    }
                }

            }

            Debug.Log(counter);


            GradientTexture.SetPixels(colors);
            GradientTexture.Apply();

            return GradientTexture;

        }

        public float GetMinDataValue(float[] data, int dimensionX, int dimensionY, int dimensionZ)
        {
            float minDataValue = float.MaxValue;
            for (int i = 0; i < data.Length; i++)
            {
                float val = data[i];
                minDataValue = Mathf.Min(minDataValue, val);
            }
            return minDataValue;
        }

        public float GetMaxDataValue(float[] data, int dimensionX, int dimensionY, int dimensionZ)
        {
            float maxDataValue = float.MinValue;
            for (int i = 0; i < data.Length; i++)
            {
                float val = data[i];
                maxDataValue = Mathf.Max(maxDataValue, val);
            }
            return maxDataValue;
        }


    }
}