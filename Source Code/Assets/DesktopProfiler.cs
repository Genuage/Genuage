using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Data;
using Display;
using IO;
using SFB;

namespace DesktopInterface
{


    public class DesktopProfiler : IButtonScript
    {
        bool runUpdate = false;
        Vector3 initialMouseScreenPosition;
        Vector3 CurrentMouseScreenPosition;
        Vector3 CurrentMouseWorldPosition;
        Vector3 initialMouseWorldPosition;
        Vector3 FinalMousePosition;
        GameObject MeshObject;
        public GameObject UIWindow;
        CloudData data;

        GameObject cylinder = null;
        GameObject cylinderSide1;
        GameObject cylinderSide2;

        public InputField CylinderSide1_XField;
        public InputField CylinderSide1_YField;
        public InputField CylinderSide1_ZField;

        public InputField CylinderSide2_XField;
        public InputField CylinderSide2_YField;
        public InputField CylinderSide2_ZField;

        public InputField CylinderHeightField;


        public InputField BinsField;
        private int BinNumber;

        public Button ExportButton;
        public Button CloseButton;

        public Text DistanceText;
        private float Distance; 

        public List<GameObject> sectionList = new List<GameObject>();

        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();

            CylinderSide1_XField.onEndEdit.AddListener(delegate { ChangeCylinderPositions(); });
            CylinderSide1_YField.onEndEdit.AddListener(delegate { ChangeCylinderPositions(); });
            CylinderSide1_ZField.onEndEdit.AddListener(delegate { ChangeCylinderPositions(); });

            CylinderSide2_XField.onEndEdit.AddListener(delegate { ChangeCylinderPositions(); });
            CylinderSide2_YField.onEndEdit.AddListener(delegate { ChangeCylinderPositions(); });
            CylinderSide2_ZField.onEndEdit.AddListener(delegate { ChangeCylinderPositions(); });

            CylinderHeightField.onEndEdit.AddListener(delegate { ChangeCylinderHeight(); });

            BinsField.onEndEdit.AddListener(delegate { ChangeBinNumber(); });

            ExportButton.onClick.AddListener(delegate { ExportHistogram(); });

            CloseButton.onClick.AddListener(delegate { CloseHistogram(); });
        }


        public void ExportButtonClicked()
        {
            if (runUpdate)
            {
                Execute();
            }
            CloudUpdater.instance.CreateCloudFromSelection();
        }

        public override void Execute()
        {
            if (!CloudSelector.instance.noSelection)
            {
                data = CloudUpdater.instance.LoadCurrentStatus();
                data.transform.parent.gameObject.GetComponent<CloudObjectRefference>().box.gameObject.GetComponent<DragMouse>().enabled = runUpdate;
                data.globalMetaData.FreeSelectionON = true;

                runUpdate = !runUpdate;
                if (runUpdate)
                {
                    MeshObject = new GameObject("Selection Mesh Object");
                    MeshObject.AddComponent<MeshRenderer>();
                    MeshObject.AddComponent<MeshFilter>();
                    data.transform.parent.gameObject.GetComponent<CloudObjectRefference>().box.transform.position = Vector3.zero;
                    data.transform.parent.gameObject.GetComponent<CloudObjectRefference>().box.transform.eulerAngles = Vector3.zero;
                    UIManager.instance.DeactivateSelectionButtons();
                    GetComponent<Image>().color = Color.green;
                }
                else
                {
                    Destroy(MeshObject);
                    UIManager.instance.ActivateSelectionButtons();
                    GetComponent<Image>().color = Color.white;
                }

            }
        }

        private void LateUpdate()
        {
            if (runUpdate)
            {


                if (EventSystem.current.IsPointerOverGameObject())
                {
                    PointerEventData pointer = new PointerEventData(EventSystem.current);
                    pointer.position = Input.mousePosition;
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointer, raycastResults);

                    if (raycastResults.Count > 0 && raycastResults[0].gameObject.layer == LayerMask.NameToLayer("UI"))
                    {
                        return;
                    }
                }
                //Calculate Mouse World Position

                if (Input.GetMouseButtonDown(0))
                {
                    initialMouseScreenPosition = Input.mousePosition;
                    RecordInitialPosition();
                }
                else if (Input.GetMouseButton(0))
                {
                    RecordInitialPosition();
                    //Generate mesh wires
                    CalculateCurrentPosition();
                    UpdateMesh();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    RecordInitialPosition();

                    RecordFinalPosition();
                    //RECTANGLE CALCULUS
                    //Vector3 Vertex1 = new Vector3(FinalMousePosition.x, initialMouseWorldPosition.y, FinalMousePosition.z);

                    //Vector3 Vertex3 = new Vector3(initialMouseWorldPosition.x, FinalMousePosition.y, FinalMousePosition.z); ;



                    Matrix4x4 world_to_local = data.gameObject.transform.worldToLocalMatrix;

                    Vector3 LocalVertex0 = world_to_local.MultiplyPoint3x4(initialMouseWorldPosition);
                    //Vector3 LocalVertex1 = world_to_local.MultiplyPoint3x4(Vertex1);
                    Vector3 LocalVertex2 = world_to_local.MultiplyPoint3x4(FinalMousePosition);
                    //Vector3 LocalVertex3 = world_to_local.MultiplyPoint3x4(Vertex3);

                    float MaxX = Mathf.Max(LocalVertex2.x, LocalVertex0.x);
                    float MinX = Mathf.Min(LocalVertex2.x, LocalVertex0.x);
                    float MaxY = Mathf.Max(LocalVertex2.y, LocalVertex0.y);
                    float MinY = Mathf.Min(LocalVertex2.y, LocalVertex0.y);
                    if (cylinder)
                    {
                        Destroy(cylinder);
                    }
                    cylinder = CreateCylinder(LocalVertex0, LocalVertex2);

                    ShowWindow();
                    
                    //TODO
                    //Make window with the 3 positions and number of bins
                    //when a position is changed, move the two ends of the cylinder and make them face eachother
                    //when the bins are set you can run the analysis and display the histogram

                }
            }
        }
        private void RecordInitialPosition()
        {
            initialMouseScreenPosition.z = Mathf.Abs(CameraManager.instance.desktop_camera.transform.position.z) + CameraManager.instance.desktop_camera.nearClipPlane;
            Vector3 newInWorldPosition = CameraManager.instance.desktop_camera.ScreenToWorldPoint(initialMouseScreenPosition);
            initialMouseWorldPosition = newInWorldPosition;

            //Find first mouse position

        }

        private void CalculateCurrentPosition()
        {
            CurrentMouseScreenPosition = Input.mousePosition;
            CurrentMouseScreenPosition.z = Mathf.Abs(CameraManager.instance.desktop_camera.transform.position.z) + CameraManager.instance.desktop_camera.nearClipPlane;
            Vector3 WorldPosition = CameraManager.instance.desktop_camera.ScreenToWorldPoint(CurrentMouseScreenPosition);
            UIManager.instance.ChangeStatusText(" Mouse Position : " + WorldPosition.ToString("F4"));
            CurrentMouseWorldPosition = WorldPosition;

        }
        private void RecordFinalPosition()
        {
            CurrentMouseScreenPosition = Input.mousePosition;
            CurrentMouseScreenPosition.z = Mathf.Abs(CameraManager.instance.desktop_camera.transform.position.z) + CameraManager.instance.desktop_camera.nearClipPlane;
            Vector3 WorldPosition = CameraManager.instance.desktop_camera.ScreenToWorldPoint(CurrentMouseScreenPosition);
            UIManager.instance.ChangeStatusText(" Mouse Position : " + WorldPosition.ToString("F4"));
            FinalMousePosition = WorldPosition;

        }

        private void UpdateMesh()
        {
            //RECTANGLE CALCULUS
            //Vector3 Vertex1 = new Vector3(CurrentMouseWorldPosition.x, initialMouseWorldPosition.y, CurrentMouseWorldPosition.z);

            //Vector3 Vertex3 = new Vector3(initialMouseWorldPosition.x, CurrentMouseWorldPosition.y, CurrentMouseWorldPosition.z); ;



            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[2] { initialMouseWorldPosition, CurrentMouseWorldPosition};
            mesh.SetIndices(new int[2] { 0, 1}, MeshTopology.Lines, 0);
            MeshObject.GetComponent<MeshFilter>().mesh = mesh;

        }

        private GameObject CreateCylinder(Vector3 Vertex1, Vector3 Vertex2)
        {
            Vertex1.z = 0f;
            Vertex2.z = 0f;

            GameObject go = CreateCircleMesh(Vertex1, Vertex2);
            GameObject go2 = CreateCircleMesh(Vertex2, Vertex1);

            GameObject obj = new GameObject();
            //obj.transform.SetParent(baseCircle.transform, false);
            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            MeshFilter mf = obj.AddComponent<MeshFilter>();
            go.transform.SetParent(obj.transform, true);
            go2.transform.SetParent(obj.transform, true);

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            vertices.Add(go.transform.position);
            vertices.Add(go2.transform.position);
            indices.Add(0);
            indices.Add(1);
            mf.mesh.vertices = vertices.ToArray();
            mf.mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

            mr.material = new Material(Shader.Find("Standard"));
            cylinderSide1 = go;
            cylinderSide2 = go2;
            obj.transform.SetParent(data.transform, true);
            return obj;
        }

        public void ReshapeCylinder()
        {
            cylinderSide1.transform.LookAt(cylinderSide2.transform);
            cylinderSide2.transform.LookAt(cylinderSide1.transform);
            MeshFilter mf = cylinder.GetComponent<MeshFilter>();
            Vector3[] newvertices = new Vector3[2] { cylinderSide1.transform.localPosition, cylinderSide2.transform.localPosition };
            mf.mesh.vertices = newvertices;

        }

        public GameObject CreateCircleMesh(Vector3 position, Vector3 direction)
        {
            GameObject newpoint = new GameObject("Circle");

            //Creates a circle

            MeshRenderer mr = newpoint.AddComponent<MeshRenderer>();
            MeshFilter mf = newpoint.AddComponent<MeshFilter>();

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();



            int counter = 0;


            int n = 50;
            float radius = 1f;
            float x, y, z;
            for (int i = 0; i < n; i++) // phi
            {
                x = radius * Mathf.Cos((2 * Mathf.PI * i) / n);
                y = radius * Mathf.Sin((2 * Mathf.PI * i) / n);
                z = 0;
                vertices.Add(new Vector3(x, y, z));
                indices.Add(counter++);

                x = radius * Mathf.Cos((2 * Mathf.PI * (i + 1)) / n);
                y = radius * Mathf.Sin((2 * Mathf.PI * (i + 1)) / n);
                z = 0;
                vertices.Add(new Vector3(x, y, z));
                indices.Add(counter++);
            }

            mf.mesh.vertices = vertices.ToArray();
            mf.mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

            mr.material = new Material(Shader.Find("Standard"));






            newpoint.transform.localScale = Vector3.one * 0.05f;
            newpoint.transform.position = position;
            newpoint.transform.LookAt(direction);



            return newpoint;
        }


        private void ShowWindow()
        {
            UIWindow.SetActive(true);
            CylinderSide1_XField.text = Math.Round(cylinderSide1.transform.localPosition.x * data.globalMetaData.maxRange, 2).ToString(CultureInfo.InvariantCulture);
            CylinderSide1_YField.text = Math.Round(cylinderSide1.transform.localPosition.y * data.globalMetaData.maxRange, 2).ToString(CultureInfo.InvariantCulture);
            CylinderSide1_ZField.text = Math.Round(cylinderSide1.transform.localPosition.z * data.globalMetaData.maxRange, 2).ToString(CultureInfo.InvariantCulture);
            CylinderSide2_XField.text = Math.Round(cylinderSide2.transform.localPosition.x * data.globalMetaData.maxRange, 2).ToString(CultureInfo.InvariantCulture);
            CylinderSide2_YField.text = Math.Round(cylinderSide2.transform.localPosition.y * data.globalMetaData.maxRange, 2).ToString(CultureInfo.InvariantCulture);
            CylinderSide2_ZField.text = Math.Round(cylinderSide2.transform.localPosition.z * data.globalMetaData.maxRange, 2).ToString(CultureInfo.InvariantCulture);
            DistanceText.text = "Cylinder distance : "+Vector3.Distance(cylinderSide1.transform.localPosition * data.globalMetaData.maxRange, cylinderSide2.transform.localPosition * data.globalMetaData.maxRange).ToString(CultureInfo.InvariantCulture);
            Distance = Vector3.Distance(cylinderSide1.transform.localPosition * data.globalMetaData.maxRange, cylinderSide2.transform.localPosition * data.globalMetaData.maxRange);
            ChangeBinNumber();
        }

        private void ChangeCylinderHeight()
        {
            float newscale = (float)double.Parse(CylinderHeightField.text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);

            cylinderSide1.transform.localScale = Vector3.one * newscale;
            cylinderSide2.transform.localScale = Vector3.one * newscale;

            if (sectionList.Count != 0)
            {
                foreach(GameObject obj in sectionList)
                {
                    obj.transform.localScale = Vector3.one * newscale;
                }
            }
        }

        private void ChangeCylinderPositions()
        {
            Vector3 PosSide1 = new Vector3((float)double.Parse(CylinderSide1_XField.text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture) / data.globalMetaData.maxRange,
                                           (float)double.Parse(CylinderSide1_YField.text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture) / data.globalMetaData.maxRange,
                                           (float)double.Parse(CylinderSide1_ZField.text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture) / data.globalMetaData.maxRange);

            Vector3 PosSide2 = new Vector3((float)double.Parse(CylinderSide2_XField.text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture) / data.globalMetaData.maxRange,
                                           (float)double.Parse(CylinderSide2_YField.text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture) / data.globalMetaData.maxRange,
                                           (float)double.Parse(CylinderSide2_ZField.text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture) / data.globalMetaData.maxRange);

            cylinderSide1.transform.localPosition = PosSide1;
            cylinderSide2.transform.localPosition = PosSide2;

            DistanceText.text = Vector3.Distance(cylinderSide1.transform.localPosition * data.globalMetaData.maxRange, cylinderSide2.transform.localPosition * data.globalMetaData.maxRange).ToString(CultureInfo.InvariantCulture);
            Distance = Vector3.Distance(cylinderSide1.transform.localPosition * data.globalMetaData.maxRange, cylinderSide2.transform.localPosition * data.globalMetaData.maxRange);

            ReshapeCylinder();
            ChangeBinNumber();
        }

        private void ChangeBinNumber()
        {

            BinNumber = int.Parse(BinsField.text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
            if (BinNumber != 0)
            {


                Debug.Log("bin number : " + BinNumber);
                if (sectionList.Count != 0)
                {
                    for (int j = 0; j < sectionList.Count; j++)
                    {
                        Destroy(sectionList[j]);
                    }
                    sectionList.Clear();

                }

                float distance = Vector3.Distance(cylinderSide1.transform.localPosition, cylinderSide2.transform.localPosition);
                float sizeinterval = distance / BinNumber;
                for (int i = 1; i < BinNumber; i++)
                {
                    GameObject newcircle = CreateCircleMesh(cylinderSide1.transform.localPosition, cylinderSide2.transform.localPosition);
                    newcircle.transform.position = cylinderSide1.transform.position;
                    newcircle.transform.localScale = cylinderSide1.transform.localScale;
                    newcircle.transform.rotation = cylinderSide1.transform.rotation;
                    Vector3 direction = cylinderSide2.transform.position - cylinderSide1.transform.position;
                    newcircle.transform.position += (direction.normalized * (i * sizeinterval));
                    newcircle.transform.SetParent(cylinder.transform);
                    sectionList.Add(newcircle);


                }
                VR_Interaction.HistogramPointSelector selector = GetComponent<VR_Interaction.HistogramPointSelector>();
                List<GameObject> circleList = new List<GameObject>();
                List<Vector3> circlePositionsList = new List<Vector3>();

                circlePositionsList.Add(cylinderSide1.transform.position);
                circleList.Add(cylinderSide1);
                foreach (var go in sectionList)
                {
                    circlePositionsList.Add(go.transform.position);
                    circleList.Add(go);
                }
                circlePositionsList.Add(cylinderSide2.transform.position);
                circleList.Add(cylinderSide2);
                selector.radius = cylinderSide1.transform.localScale.x;
                selector.sectionsNumber = BinNumber;
                selector.FindPointsProto(circleList, circlePositionsList);
            }
        }
        private void ExportHistogram()
        {
            VR_Interaction.HistogramPointSelector selector = GetComponent<VR_Interaction.HistogramPointSelector>();

            HistogramSaveable hsave = new HistogramSaveable();
            hsave.HistogramXValues = selector.xValues;
            hsave.HistogramYValues = selector.yValues;
            hsave.Distance = Distance;
            hsave.Side1Position = cylinderSide1.transform.localPosition;
            hsave.Side2Position = cylinderSide1.transform.localPosition;

            var extensions = new[] {
                new ExtensionFilter("JSON", ".JSON")};
            StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "", extensions, (string path) => { SaveJSON(path, hsave); });
        }

        public void SaveJSON(string path, HistogramSaveable histogramData)
        {
            string JSON = JsonUtility.ToJson(histogramData);
            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(directory + Path.DirectorySeparatorChar + filename + ".JSON"))
            {
                writer.WriteLine(JSON);
            }
        }

        private void CloseHistogram()
        {
            GetComponent<VR_Interaction.HistogramPointSelector>().DestroyHistogramCanvas();
            Destroy(cylinder);
        }
    }

  
}