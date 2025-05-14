using UnityEngine;
using UnityEditor;

using UnityEngine.UI;
using System.Linq;
using Data;
using Display;
using imColorPicker;

namespace DesktopInterface
{
    public class TransferFunctionUI : MonoBehaviour
    {

        //public static VolumeData data;

        public static TransferFunction TransferFunctionDiffuseOpacity;
        public static TransferFunction TransferFunctionSpecular;
        public static Texture2D DefaultSpecularTexture;

        private int movingColPointIndexDiffuse = -1;
        private int movingColPointIndexSpecular = -1;
        private bool DiffuseColorSelected = false;
        private bool SpecularColorSelected = false;

        private int movingAlphaPointIndex = -1;

        private int selectedColPointIndex = -1;

        private static Texture2D histTex = null;

        public Material tfGUIMat = null;
        public Material tfPaletteGUIMat = null;
        public Material tfPaletteGUIMat2 = null;
        public ComputeShader ComputeHistogram;

        public static IMColorPicker colorPicker;
        private static bool showWindow = false;

        public static bool TFhasChanged = false; //Used for path tracing update 

        private static VolumeData volumedata;
         

        private void Awake()
        {
            showWindow = false;

            //tfGUIMat = Resources.Load<Material>("TransferFunctionGUIMat");
            //tfPaletteGUIMat = Resources.Load<Material>("TransferFunctionPaletteGUIMat");
            //tfPaletteGUIMat2 = Resources.Load<Material>("TransferFunctionPaletteGUIMat");

        }

        public static void ShowWindow(TransferFunction tfOpacityDiffuse, VolumeData vdata)
        {
            TransferFunctionDiffuseOpacity = tfOpacityDiffuse;
            //TransferFunctionSpecular = tfSpecular;
            //DefaultSpecularTexture = DefaultTex;
            volumedata = vdata;
            histTex = null;
            showWindow = true;
            colorPicker = new IMColorPicker();

            //TransferFunctionEditorWindow wnd = (TransferFunctionEditorWindow)EditorWindow.GetWindow(typeof(TransferFunctionEditorWindow));
            //wnd.Show();
            //wnd.SetInitialPosition();
        }

        /**
        private void OnEnable()
        {
            showWindow = true;
        }
        **/
        private void OnDisable()
        {
            //showWindow = false;
        }


        private void OnGUI()
        {
            if (showWindow == true)
            {
                TFhasChanged = false;

                //tf = volRendObject.transferFunction;

                Event currentEvent = new Event(Event.current);

                Color oldColour = GUI.color; // Used for setting GUI.color when drawing UI elements
                RectTransform rectTransform = GetComponent<RectTransform>();
                RectTransform ParentRectTransform = transform.parent.GetComponent<RectTransform>();
                float contentWidth = Mathf.Min(rectTransform.rect.width, (rectTransform.rect.height - 100.0f) * 2.0f);
                float contentHeight = contentWidth * 0.5f;

                //Vector3 Screenpos = CameraManager.instance.desktop_camera.WorldToScreenPoint(transform.position);
                Vector3 Screenpos = rectTransform.position;
                Vector3 GUIpos = new Vector3(Screenpos.x - (rectTransform.rect.width / 2), (Screen.height-Screenpos.y) - (rectTransform.rect.height / 2), Screenpos.z);
                //Debug.Log(GUIpos.ToString());
                // Interaction area (slightly larger than the histogram rect)
                Rect interactRect = new Rect(GUIpos.x , GUIpos.y, rectTransform.rect.width , rectTransform.rect.height);


                
                // Histogram rect (histogram view and alpha control points)
                Rect histRect = new Rect(interactRect.x + 20f, interactRect.y + 40f, interactRect.width - 50f, interactRect.height - 70f);

                

                // Diffuse Colour palette rect (colour control points)
                Rect DiffusePaletteRect = new Rect(histRect.x, histRect.y + histRect.height + 20, histRect.width, 20.0f);

                // Specular Colour palette rect (colour control points)
                //Rect SpecularPaletteRect = new Rect(histRect.x, histRect.y + histRect.height + 60, histRect.width, 20.0f);


                // TODO: Don't do this every frame
                TransferFunctionDiffuseOpacity.GenerateTexture();
                //TransferFunctionSpecular.GenerateTexture();

                
                // Create histogram texture
                if (histTex == null)
                {
                    if (SystemInfo.supportsComputeShaders)
                        histTex = HistogramTextureGenerator.GenerateHistogramTextureOnGPU(volumedata, ComputeHistogram);
                    else
                        histTex = HistogramTextureGenerator.GenerateHistogramTexture(volumedata);
                }

                // Draw histogram
                tfGUIMat.SetTexture("_TFTex", TransferFunctionDiffuseOpacity.GetTexture());
                tfGUIMat.SetTexture("_HistTex", histTex);
                Graphics.DrawTexture(histRect, TransferFunctionDiffuseOpacity.GetTexture(), tfGUIMat);
                
                // Draw Diffuse colour palette
                Texture2D tfTexture = TransferFunctionDiffuseOpacity.GetTexture();
                tfPaletteGUIMat.SetTexture("_TFTex", TransferFunctionDiffuseOpacity.GetTexture());
                Graphics.DrawTexture(new Rect(DiffusePaletteRect.x, DiffusePaletteRect.y, DiffusePaletteRect.width, DiffusePaletteRect.height), tfTexture, tfPaletteGUIMat);
                /**
                // Draw Specular colour palette
                Texture2D tfTexture2 = TransferFunctionSpecular.GetTexture();
                //tfPaletteGUIMat2.SetTexture("_TFTex", TransferFunctionSpecular.GetTexture());
                tfPaletteGUIMat2.SetTexture("_TFTex", DefaultSpecularTexture);
                Graphics.DrawTexture(new Rect(SpecularPaletteRect.x, SpecularPaletteRect.y, SpecularPaletteRect.width, SpecularPaletteRect.height), tfTexture2, tfPaletteGUIMat2);
                **/


                colorPicker.DrawWindow();

                // Release selected colour/alpha points if mouse leaves window
                if (movingAlphaPointIndex != -1 && !interactRect.Contains(currentEvent.mousePosition))
                    movingAlphaPointIndex = -1;
                
                if (movingColPointIndexDiffuse != -1 && !(currentEvent.mousePosition.x >= DiffusePaletteRect.x && currentEvent.mousePosition.x <= DiffusePaletteRect.x + DiffusePaletteRect.width))
                    movingColPointIndexDiffuse = -1;
                /**
                if (movingColPointIndexSpecular != -1 && !(currentEvent.mousePosition.x >= DiffusePaletteRect.x && currentEvent.mousePosition.x <= DiffusePaletteRect.x + DiffusePaletteRect.width))
                    movingColPointIndexSpecular = -1;
                **/

                
                // Mouse down => Move or remove selected colour control point
                if (currentEvent.type == EventType.MouseDown && DiffusePaletteRect.Contains(currentEvent.mousePosition))
                {
                    float mousePos = (currentEvent.mousePosition.x - DiffusePaletteRect.x) / DiffusePaletteRect.width;
                    int pointIndex = PickColourControlPoint(mousePos, TransferFunctionDiffuseOpacity);
                    if (pointIndex != -1)
                    {
                        // Add control point
                        if (currentEvent.button == 0 && !currentEvent.control)
                        {
                            movingColPointIndexDiffuse = selectedColPointIndex = pointIndex;
                            TFColourControlPoint colPoint = TransferFunctionDiffuseOpacity.colourControlPoints[selectedColPointIndex];
                            Color newcolor = colPoint.colourValue;
                            colorPicker.color = newcolor;
                            //colorPicker.DrawColorPicker();

                            DiffuseColorSelected = true;
                            SpecularColorSelected = false;
                        }
                        // Remove control point
                        else if (currentEvent.button == 1 && currentEvent.control)
                        {
                            TransferFunctionDiffuseOpacity.colourControlPoints.RemoveAt(pointIndex);
                            currentEvent.type = EventType.Ignore;
                            movingColPointIndexDiffuse = selectedColPointIndex = -1;
                        }
                        TFhasChanged = true;
                    }
                }
                /**
                else if (currentEvent.type == EventType.MouseDown && SpecularPaletteRect.Contains(currentEvent.mousePosition))
                {
                    float mousePos = (currentEvent.mousePosition.x - SpecularPaletteRect.x) / SpecularPaletteRect.width;
                    int pointIndex = PickColourControlPoint(mousePos, TransferFunctionSpecular);
                    if (pointIndex != -1)
                    {
                        // Add control point
                        if (currentEvent.button == 0 && !currentEvent.control)
                        {
                            movingColPointIndexSpecular = selectedColPointIndex = pointIndex;
                            TFColourControlPoint colPoint = TransferFunctionDiffuseOpacity.colourControlPoints[selectedColPointIndex];
                            Color newcolor = colPoint.colourValue;
                            //colorPicker.color = newcolor;

                            DiffuseColorSelected = false;
                            SpecularColorSelected = true;
                        }
                        // Remove control point
                        else if (currentEvent.button == 1 && currentEvent.control)
                        {
                            TransferFunctionSpecular.colourControlPoints.RemoveAt(pointIndex);
                            currentEvent.type = EventType.Ignore;
                            movingColPointIndexSpecular = selectedColPointIndex = -1;
                        }
                        TFhasChanged = true;
                    }

                }
                **/
                else if (currentEvent.type == EventType.MouseUp)
                {
                    movingColPointIndexDiffuse = -1;
                    movingColPointIndexSpecular = -1;
                }
                

                // Mouse down => Move or remove selected alpha control point
                if (currentEvent.type == EventType.MouseDown)
                {
                    Vector2 mousePos = new Vector2((currentEvent.mousePosition.x - histRect.x) / histRect.width, 1.0f - (currentEvent.mousePosition.y - histRect.y) / histRect.height);
                    int pointIndex = PickAlphaControlPoint(mousePos);
                    if (pointIndex != -1)
                    {
                        // Add control point
                        if (currentEvent.button == 0 && !currentEvent.control)
                        {
                            movingAlphaPointIndex = pointIndex;
                        }
                        // Remove control point
                        else if (currentEvent.button == 1 && currentEvent.control)
                        {
                            TransferFunctionDiffuseOpacity.alphaControlPoints.RemoveAt(pointIndex);
                            currentEvent.type = EventType.Ignore;
                            selectedColPointIndex = -1;
                        }
                        TFhasChanged = true;
                    }
                }

                
                // Move selected colour control point
                if (movingColPointIndexDiffuse != -1)
                {
                    TFColourControlPoint colPoint = TransferFunctionDiffuseOpacity.colourControlPoints[movingColPointIndexDiffuse];
                    colPoint.dataValue = Mathf.Clamp((currentEvent.mousePosition.x - DiffusePaletteRect.x) / DiffusePaletteRect.width, 0.0f, 1.0f);
                    TransferFunctionDiffuseOpacity.colourControlPoints[movingColPointIndexDiffuse] = colPoint;
                    TFhasChanged = true;

                }

                /**
                if (movingColPointIndexSpecular != -1)
                {
                    TFColourControlPoint colPoint = TransferFunctionSpecular.colourControlPoints[movingColPointIndexSpecular];
                    colPoint.dataValue = Mathf.Clamp((currentEvent.mousePosition.x - SpecularPaletteRect.x) / SpecularPaletteRect.width, 0.0f, 1.0f);
                    TransferFunctionSpecular.colourControlPoints[movingColPointIndexSpecular] = colPoint;
                    TFhasChanged = true;

                }
                **/

                // Move selected alpha control point
                if (movingAlphaPointIndex != -1)
                {
                    TFAlphaControlPoint alphaPoint = TransferFunctionDiffuseOpacity.alphaControlPoints[movingAlphaPointIndex];
                    alphaPoint.dataValue = Mathf.Clamp((currentEvent.mousePosition.x - histRect.x) / histRect.width, 0.0f, 1.0f);
                    alphaPoint.alphaValue = Mathf.Clamp(1.0f - (currentEvent.mousePosition.y - histRect.y) / histRect.height, 0.0f, 1.0f);
                    TransferFunctionDiffuseOpacity.alphaControlPoints[movingAlphaPointIndex] = alphaPoint;
                    TFhasChanged = true;
                }


                
                // Draw colour control points Diffuse
                for (int iCol = 0; iCol < TransferFunctionDiffuseOpacity.colourControlPoints.Count; iCol++)
                {

                    TFColourControlPoint colPoint = TransferFunctionDiffuseOpacity.colourControlPoints[iCol];
                    Rect ctrlBox = new Rect(histRect.x + histRect.width * colPoint.dataValue, histRect.y + histRect.height + 20, 10, 20);
                    GUI.color = Color.red;
                    GUI.skin.box.fontSize = 6;
                    GUI.Box(ctrlBox, "*");
                }
                /**
                // Draw colour control points Specular
                for (int iCol = 0; iCol < TransferFunctionSpecular.colourControlPoints.Count; iCol++)
                {
                    TFColourControlPoint colPoint = TransferFunctionSpecular.colourControlPoints[iCol];
                    Rect ctrlBox = new Rect(histRect.x + histRect.width * colPoint.dataValue, histRect.y + histRect.height + 60, 10, 20);
                    GUI.color = Color.red;
                    GUI.skin.box.fontSize = 6;
                    GUI.Box(ctrlBox, "*");
                }
                **/

                // Draw alpha control points
                for (int iAlpha = 0; iAlpha < TransferFunctionDiffuseOpacity.alphaControlPoints.Count; iAlpha++)
                {
                    int pointSize = 10;
                    TFAlphaControlPoint alphaPoint = TransferFunctionDiffuseOpacity.alphaControlPoints[iAlpha];
                    Rect ctrlBox = new Rect(histRect.x + histRect.width * alphaPoint.dataValue - pointSize / 2, histRect.y + (1.0f - alphaPoint.alphaValue) * histRect.height - pointSize / 2, pointSize, pointSize);
                    GUI.color = Color.red;
                    GUI.skin.box.fontSize = 6;
                    GUI.Box(ctrlBox, "*");
                    GUI.color = oldColour;
                }

                if (currentEvent.type == EventType.MouseUp)
                {
                    movingColPointIndexDiffuse = -1;
                    movingAlphaPointIndex = -1;
                }

                // Add points
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1)
                {
                    if (histRect.Contains(new Vector2(currentEvent.mousePosition.x, currentEvent.mousePosition.y)))
                        TransferFunctionDiffuseOpacity.alphaControlPoints.Add(new TFAlphaControlPoint(Mathf.Clamp((currentEvent.mousePosition.x - histRect.x) / histRect.width, 0.0f, 1.0f), Mathf.Clamp(1.0f - (currentEvent.mousePosition.y - histRect.y) / histRect.height, 0.0f, 1.0f)));
                    
                    
                    else if (DiffusePaletteRect.Contains(currentEvent.mousePosition))
                    {
                        Color newcolor = Random.ColorHSV();
                        TransferFunctionDiffuseOpacity.colourControlPoints.Add(new TFColourControlPoint(Mathf.Clamp((currentEvent.mousePosition.x - histRect.x) / histRect.width, 0.0f, 1.0f), newcolor));
                        colorPicker.color = newcolor;
                        DiffuseColorSelected = true;
                        SpecularColorSelected = false;
                        selectedColPointIndex = TransferFunctionDiffuseOpacity.colourControlPoints.Count - 1;
                        //colorPicker.DrawColorPicker();
                    }
                    /**
                    else if (SpecularPaletteRect.Contains(currentEvent.mousePosition))
                    {
                        Color newcolor = Random.ColorHSV();
                        TransferFunctionSpecular.colourControlPoints.Add(new TFColourControlPoint(Mathf.Clamp((currentEvent.mousePosition.x - histRect.x) / histRect.width, 0.0f, 1.0f), newcolor));
                        selectedColPointIndex = -1;
                        //colorPicker.color = newcolor;
                        DiffuseColorSelected = false;
                        SpecularColorSelected = true;
                        selectedColPointIndex = TransferFunctionSpecular.colourControlPoints.Count - 1;

                        //colorPicker.DrawColorPicker();
                    }
                    **/
                    TFhasChanged = true;

                }

                /**
                // Save TF
                if (GUI.Button(new Rect(histRect.x, histRect.y + histRect.height + 50.0f, 70.0f, 30.0f), "Save"))
                {
                    string filepath = EditorUtility.SaveFilePanel("Save transfer function", "", "default.tf", "tf");
                    if (filepath != "")
                        TransferFunctionDatabase.SaveTransferFunction(tf, filepath);
                }

                // Load TF
                if (GUI.Button(new Rect(histRect.x + 75.0f, histRect.y + histRect.height + 50.0f, 70.0f, 30.0f), "Load"))
                {
                    string filepath = EditorUtility.OpenFilePanel("Save transfer function", "", "tf");
                    if (filepath != "")
                    {
                        TransferFunction newTF = TransferFunctionDatabase.LoadTransferFunction(filepath);
                        if (newTF != null)
                            volRendObject.transferFunction = tf = newTF;
                    }
                }
                **/
                /**
                // Clear TF
                if (GUI.Button(new Rect(histRect.x + 150.0f, histRect.y + histRect.height + 80.0f, 70.0f, 30.0f), "Clear"))
                {
                    //tf = volRendObject.transferFunction = new TransferFunction();
                    TransferFunctionDiffuseOpacity.alphaControlPoints.Add(new TFAlphaControlPoint(0.2f, 0.0f));
                    TransferFunctionDiffuseOpacity.alphaControlPoints.Add(new TFAlphaControlPoint(0.8f, 1.0f));
                    TransferFunctionDiffuseOpacity.colourControlPoints.Add(new TFColourControlPoint(0.5f, new Color(0.469f, 0.354f, 0.223f, 1.0f)));
                    selectedColPointIndex = -1;
                }
                **/


                
                // Colour picker
                if (selectedColPointIndex != -1 && DiffuseColorSelected)
                {
                    TFColourControlPoint colPoint = TransferFunctionDiffuseOpacity.colourControlPoints[selectedColPointIndex];
                    if (colPoint.colourValue != colorPicker.color)
                    {
                        colPoint.colourValue = colorPicker.color;
                        //colPoint.colourValue = EditorGUI.ColorField(new Rect(histRect.x + 225, histRect.y + histRect.height + 80, 100.0f, 40.0f), colPoint.colourValue);
                        TransferFunctionDiffuseOpacity.colourControlPoints[selectedColPointIndex] = colPoint;
                        TFhasChanged = true;
                    }


                }
                /**
                else if (selectedColPointIndex != -1 && SpecularColorSelected)
                {
                    TFColourControlPoint colPoint = TransferFunctionSpecular.colourControlPoints[selectedColPointIndex];
                    if (!colPoint.colourValue.Equals(colorPicker.color))
                    {
                        colPoint.colourValue = colorPicker.color;
                        //colPoint.colourValue = EditorGUI.ColorField(new Rect(histRect.x + 225, histRect.y + histRect.height + 80, 100.0f, 40.0f), colPoint.colourValue);
                        TransferFunctionSpecular.colourControlPoints[selectedColPointIndex] = colPoint;
                        TFhasChanged = true;
                    }

                }
                **/


                GUI.skin.label.wordWrap = false;
                GUI.Label(new Rect(histRect.x, histRect.y + histRect.height, 720.0f, 30.0f), "Left click to select and move a control point. Right click to add a control point, and ctrl + right click to delete.");

                GUI.color = oldColour;
            }
        }


        /// <summary>
        /// Pick the colour control point, nearest to the specified position.
        /// </summary>
        /// <param name="maxDistance">Threshold for maximum distance. Points further away than this won't get picked.</param>
        private int PickColourControlPoint(float position, TransferFunction tf, float maxDistance = 0.03f)
        {
            int nearestPointIndex = -1;
            float nearestDist = 1000.0f;
            for (int i = 0; i < tf.colourControlPoints.Count; i++)
            {
                TFColourControlPoint ctrlPoint = tf.colourControlPoints[i];
                float dist = Mathf.Abs(ctrlPoint.dataValue - position);
                if (dist < maxDistance && dist < nearestDist)
                {
                    nearestPointIndex = i;
                    nearestDist = dist;
                }
            }
            return nearestPointIndex;
        }


        /// <summary>
        /// Pick the alpha control point, nearest to the specified position.
        /// </summary>
        /// <param name="maxDistance">Threshold for maximum distance. Points further away than this won't get picked.</param>
        private int PickAlphaControlPoint(Vector2 position, float maxDistance = 0.05f)
        {
            int nearestPointIndex = -1;
            float nearestDist = 1000.0f;
            for (int i = 0; i < TransferFunctionDiffuseOpacity.alphaControlPoints.Count; i++)
            {
                TFAlphaControlPoint ctrlPoint = TransferFunctionDiffuseOpacity.alphaControlPoints[i];
                Vector2 ctrlPos = new Vector2(ctrlPoint.dataValue, ctrlPoint.alphaValue);
                float dist = (ctrlPos - position).magnitude;
                if (dist < maxDistance && dist < nearestDist)
                {
                    nearestPointIndex = i;
                    nearestDist = dist;
                }
            }
            return nearestPointIndex;
        }


        /**
        private void OnSelectionChange()
        {
            VolumeRenderedObject newVolRendObj = Selection.activeGameObject?.GetComponent<VolumeRenderedObject>();
            // If we selected another volume object than the one previously edited in this GUI
            if (volRendObject != null && newVolRendObj != null && newVolRendObj != volRendObject)
                this.Close();
        }
        **/


    }
    //TODO ADAPT THIS TO NEW DATASTRUCTURE
    
    public struct VolumeData
    {
        public float MinValue;
        public float MaxValue;
        public float PixelNumber;
        public float[] pixelData;
        public Texture3D dataTexture;

    }

    public class HistogramTextureGenerator
    {
        /// <summary>
        /// Generates a histogram where:
        ///   X-axis = the data sample (density) value
        ///   Y-axis = the sample count (number of data samples with the specified density)
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static Texture2D GenerateHistogramTexture(VolumeData dataset)
        {
            float minValue = dataset.MinValue;
            float maxValue = dataset.MaxValue;
            float valueRange = maxValue - minValue;

            int numFrequencies = Mathf.Min((int)valueRange, 1024);
            Debug.Log("numfrequencies = " + numFrequencies + " valueRange = " + valueRange);

            int[] frequencies = new int[numFrequencies];

            int maxFreq = 0;
            float valRangeRecip = 1.0f / (maxValue - minValue);
            for (int iData = 0; iData < dataset.PixelNumber; iData++)
            {
                float dataValue = dataset.pixelData[iData];
                float tValue = (dataValue - minValue) * valRangeRecip;
                int freqIndex = (int)(tValue * (numFrequencies - 1));
                frequencies[freqIndex] += 1;
                maxFreq = System.Math.Max(frequencies[freqIndex], maxFreq);
            }

            Color[] cols = new Color[numFrequencies];
            Texture2D texture = new Texture2D(numFrequencies, 1, TextureFormat.RGBAFloat, false);

            for (int iSample = 0; iSample < numFrequencies; iSample++)
                cols[iSample] = new Color(Mathf.Log10((float)frequencies[iSample]) / Mathf.Log10((float)maxFreq), 0.0f, 0.0f, 1.0f);

            texture.SetPixels(cols);
            //texture.filterMode = FilterMode.Point;
            texture.Apply();

            return texture;
        }

        /// <summary>
        /// Generates a histogram (but computaion is done on GPU) where:
        ///   X-axis = the data sample (density) value
        ///   Y-axis = the sample count (number of data samples with the specified density)
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static Texture2D GenerateHistogramTextureOnGPU(VolumeData dataset, ComputeShader computeHistogram)
        {
            double actualBound = dataset.MaxValue - dataset.MinValue + 1;
            int numValues = System.Convert.ToInt32(dataset.MaxValue - dataset.MinValue + 1); // removed +1
            Debug.Log("numValues : " + numValues);
            Debug.Log("maxValue : " + dataset.MaxValue);

            Debug.Log("MinValue : " + dataset.MinValue);

            int sampleCount = System.Math.Min(numValues, 256);

            //ComputeShader computeHistogram = Resources.Load("ComputeHistogram") as ComputeShader;
            int handleInitialize = computeHistogram.FindKernel("HistogramInitialize");
            int handleMain = computeHistogram.FindKernel("HistogramMain");

            ComputeBuffer histogramBuffer = new ComputeBuffer(sampleCount, sizeof(uint) * 1);
            uint[] histogramData = new uint[sampleCount];
            Color32[] histogramCols = new Color32[sampleCount];

            Texture3D dataTexture = dataset.dataTexture;

            if (handleInitialize < 0 || handleMain < 0)
            {
                Debug.LogError("Histogram compute shader initialization failed.");
            }

            computeHistogram.SetFloat("ValueRange", (float)(numValues - 1));
            computeHistogram.SetTexture(handleMain, "VolumeTexture", dataTexture);
            computeHistogram.SetBuffer(handleMain, "HistogramBuffer", histogramBuffer);
            computeHistogram.SetBuffer(handleInitialize, "HistogramBuffer", histogramBuffer);
            Debug.Log("SampleCount : " + sampleCount);
            computeHistogram.Dispatch(handleInitialize, sampleCount / 8, 1, 1);
            computeHistogram.Dispatch(handleMain, (dataTexture.width + 7) / 8, (dataTexture.height + 7) / 8, (dataTexture.depth + 7) / 8);

            histogramBuffer.GetData(histogramData);

            int maxValue = (int)histogramData.Max();

            Texture2D texture = new Texture2D(sampleCount, 1, TextureFormat.RGBA32, false);
            for (int iSample = 0; iSample < sampleCount; iSample++)
            {
                histogramCols[iSample] = new Color(Mathf.Log10((float)histogramData[iSample]) / Mathf.Log10((float)maxValue), 0.0f, 0.0f, 1.0f);
            }

            texture.SetPixels32(histogramCols);
            texture.Apply();

            return texture;
        }
    }

}
