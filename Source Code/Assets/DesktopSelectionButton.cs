using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Data;
using Display;


namespace DesktopInterface
{
    public class DesktopSelectionButton : IButtonScript
    {
        bool runUpdate = false;
        Vector3 initialMouseScreenPosition;
        Vector3 CurrentMouseScreenPosition;
        Vector3 CurrentMouseWorldPosition;
        Vector3 initialMouseWorldPosition;
        Vector3 FinalMousePosition;
        GameObject MeshObject;
        CloudData data;
        public bool SelectOneTrajectory = false;
        public Button ExportButton;

        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
            ExportButton.onClick.AddListener(ExportButtonClicked);
        }

        public void ToggleTrajectorySelection()
        {
            SelectOneTrajectory = !SelectOneTrajectory;
        }

        public void ExportButtonClicked()
        {
            if (runUpdate)
            {
                Execute();
                ExportButton.gameObject.SetActive(false);
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
                    Vector3 Vertex1 = new Vector3(FinalMousePosition.x, initialMouseWorldPosition.y, FinalMousePosition.z);

                    Vector3 Vertex3 = new Vector3(initialMouseWorldPosition.x, FinalMousePosition.y, FinalMousePosition.z); ;



                    Matrix4x4 world_to_local = data.gameObject.transform.worldToLocalMatrix;

                    Vector3 LocalVertex0 = world_to_local.MultiplyPoint3x4(initialMouseWorldPosition);
                    Vector3 LocalVertex1 = world_to_local.MultiplyPoint3x4(Vertex1);
                    Vector3 LocalVertex2 = world_to_local.MultiplyPoint3x4(FinalMousePosition);
                    Vector3 LocalVertex3 = world_to_local.MultiplyPoint3x4(Vertex3);

                    float MaxX = Mathf.Max(LocalVertex2.x, LocalVertex0.x);
                    float MinX = Mathf.Min(LocalVertex2.x, LocalVertex0.x);
                    float MaxY = Mathf.Max(LocalVertex2.y, LocalVertex0.y);
                    float MinY = Mathf.Min(LocalVertex2.y, LocalVertex0.y);

                    if (SelectOneTrajectory == true)
                    {
                        bool finished = false;
                        foreach (var kvp in data.pointDataTable)
                        {
                            Vector3 localpos = kvp.Value.normed_position;
                            if (localpos.x >= MinX && localpos.x <= MaxX && localpos.y >= MinY && localpos.y <= MaxY
                                && data.pointMetaDataTable[kvp.Key].isHidden == false && kvp.Value.frame <= data.globalMetaData.upperframeLimit
                                && kvp.Value.frame >= data.globalMetaData.lowerframeLimit && finished == false)
                            {
                                List<int> pointList = data.pointTrajectoriesTable[kvp.Value.trajectory].pointsIDList;
                                Debug.Log("PointList Count " + pointList.Count);
                                data.globalMetaData.FreeSelectionIDList.Clear();
                                foreach (int id in pointList)
                                {
                                    data.globalMetaData.FreeSelectionIDList.Add(id);
                                }
                                finished = true;
                                Debug.Log("Free Selection Count " + data.globalMetaData.FreeSelectionIDList.Count);
                            }
                        }
                    }
                    else
                    {
                        foreach (var kvp in data.pointDataTable)
                        {
                            Vector3 localpos = kvp.Value.normed_position;
                            if (localpos.x >= MinX && localpos.x <= MaxX && localpos.y >= MinY && localpos.y <= MaxY
                                && data.pointMetaDataTable[kvp.Key].isHidden == false)
                            {
                                data.globalMetaData.FreeSelectionIDList.Add(kvp.Key);

                            }

                        }
                        Debug.Log("Free Selection Count " + data.globalMetaData.FreeSelectionIDList.Count);

                    }

                    MeshObject.GetComponent<MeshFilter>().mesh = null;
                    CloudUpdater.instance.UpdatePointSelection();
                    //Find last mouse position and calculate the area selected
                }
                //ERASE SELECTION
                else if (Input.GetMouseButtonDown(1))
                {
                    initialMouseScreenPosition = Input.mousePosition;
                    RecordInitialPosition();
                }
                else if (Input.GetMouseButton(1))
                {
                    RecordInitialPosition();
                    //Generate mesh wires
                    CalculateCurrentPosition();
                    UpdateMesh();
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    RecordInitialPosition();

                    RecordFinalPosition();
                    //RECTANGLE CALCULUS
                    Vector3 Vertex1 = new Vector3(FinalMousePosition.x, initialMouseWorldPosition.y, FinalMousePosition.z);

                    Vector3 Vertex3 = new Vector3(initialMouseWorldPosition.x, FinalMousePosition.y, FinalMousePosition.z); ;



                    Matrix4x4 world_to_local = data.gameObject.transform.worldToLocalMatrix;

                    Vector3 LocalVertex0 = world_to_local.MultiplyPoint3x4(initialMouseWorldPosition);
                    Vector3 LocalVertex1 = world_to_local.MultiplyPoint3x4(Vertex1);
                    Vector3 LocalVertex2 = world_to_local.MultiplyPoint3x4(FinalMousePosition);
                    Vector3 LocalVertex3 = world_to_local.MultiplyPoint3x4(Vertex3);

                    float MaxX = Mathf.Max(LocalVertex2.x, LocalVertex0.x);
                    float MinX = Mathf.Min(LocalVertex2.x, LocalVertex0.x);
                    float MaxY = Mathf.Max(LocalVertex2.y, LocalVertex0.y);
                    float MinY = Mathf.Min(LocalVertex2.y, LocalVertex0.y);

                    foreach (var kvp in data.pointDataTable)
                    {
                        Vector3 localpos = kvp.Value.normed_position;
                        if (localpos.x >= MinX && localpos.x <= MaxX && localpos.y >= MinY && localpos.y <= MaxY)
                        {
                            data.globalMetaData.FreeSelectionIDList.Remove(kvp.Key);

                        }
                    }
                    Debug.Log("Free Selection Count" + data.globalMetaData.FreeSelectionIDList.Count);

                    CloudUpdater.instance.UpdatePointSelection();
                    MeshObject.GetComponent<MeshFilter>().mesh = null;

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
            Vector3 Vertex1 = new Vector3(CurrentMouseWorldPosition.x, initialMouseWorldPosition.y, CurrentMouseWorldPosition.z);

            Vector3 Vertex3 = new Vector3(initialMouseWorldPosition.x, CurrentMouseWorldPosition.y, CurrentMouseWorldPosition.z); ;



            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[4] { initialMouseWorldPosition, Vertex1, CurrentMouseWorldPosition, Vertex3 };
            mesh.SetIndices(new int[8] { 0, 1, 1, 2, 2, 3, 3, 0 }, MeshTopology.Lines, 0);
            MeshObject.GetComponent<MeshFilter>().mesh = mesh;

        }
    }
}