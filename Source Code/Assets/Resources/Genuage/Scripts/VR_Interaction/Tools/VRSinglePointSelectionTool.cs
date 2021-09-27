using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using VRTK;

namespace VR_Interaction
{

    public class VRSinglePointSelectionTool : IControllerTool
    {

        protected VRTK_ControllerEvents _controller;
        protected GameObject sphere;

        public Vector3 sphereposition;
        public float diameter;
        public bool jobON = false;

        private PointSelectorDistanceBased pointSelector;
        public Material material;


        public GameObject SingleSelectionUICanvas;

        public TextMesh sectionselectiontextmesh;
        public int selectiondisplayID;
        public int selectedcolumnID = 0;

        public VRSinglePointSelectionTool(VRTK_ControllerEvents controller)
        {
            //_controller = controller;
        }

        public override void OnToolActivated()
        {
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<MeshRenderer>().material = material;
            sphere.GetComponent<SphereCollider>().enabled = false;
            sphere.transform.SetParent(this.transform);
            sphere.transform.localScale = 0.01f * Vector3.one;
            sphere.transform.localPosition = Vector3.forward * 0.05f;
            sphere.transform.localEulerAngles = Vector3.zero;
            sphere.GetComponent<SphereCollider>().isTrigger = true;
            pointSelector = gameObject.AddComponent<PointSelectorDistanceBased>();
            _controller = GetComponent<VRTK_ControllerEvents>();
            _controller.TouchpadPressed += OnTriggerClicked;
            diameter = 0.01f;

            SingleSelectionUICanvas = GetComponent<ControllerObjectsRefference>().CanvasSinglePointSelection;
            SingleSelectionUICanvas.SetActive(true);

            sectionselectiontextmesh = SingleSelectionUICanvas.GetComponent<VRContainerUIRef>().centertext;
            selectiondisplayID = 1;
            selectedcolumnID = selectiondisplayID - 1;
            sectionselectiontextmesh.text = "1";

            jobON = true;
            Shader.EnableKeyword("FREE_SELECTION");
        }

        public void OnTriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            if (e.touchpadAxis.y >= 0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
            {
                //up
                DoSelection();
            }

            else if (e.touchpadAxis.y <= -0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
            {
                //down
                DoColumnSelection();
            }

            else if (e.touchpadAxis.x <= -0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
            {

                //left
                if(selectiondisplayID-1 > 0)
                {
                    selectiondisplayID--;
                    selectedcolumnID = selectiondisplayID - 1;
                    sectionselectiontextmesh.text = selectiondisplayID.ToString();
                }

            }
            else if (e.touchpadAxis.x >= 0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
            {
                //right
                selectiondisplayID++;
                selectedcolumnID = selectiondisplayID - 1;
                sectionselectiontextmesh.text = selectiondisplayID.ToString();


            }
        }

        private void DoSelection()
        {
            if (!CloudSelector.instance.noSelection)
            {
                pointSelector.data = CloudUpdater.instance.LoadCurrentStatus();
                pointSelector.worldPosition = sphere.transform.position;
                pointSelector.selectionRadius = diameter / 2;
                pointSelector.DoSelection();
            }

        }

        private void DoColumnSelection()
        {
            if (!CloudSelector.instance.noSelection)
            {
                pointSelector.data = CloudUpdater.instance.LoadCurrentStatus();
                pointSelector.worldPosition = sphere.transform.position;
                pointSelector.selectionRadius = diameter / 2;
                pointSelector.DoColumnSelection(selectedcolumnID);
            }
        }

        private void Update()
        {
            if (jobON == true)
            {
                if (!CloudSelector.instance.noSelection)
                {
                    sphereposition = sphere.transform.position;
                    CloudData data = CloudUpdater.instance.LoadCurrentStatus();
                    Transform box = data.transform.parent.GetComponent<CloudObjectRefference>().box.transform;
                    Shader.SetGlobalMatrix("_BoxWorldToLocalMatrix", box.worldToLocalMatrix);
                    Shader.SetGlobalVector("_BoxLocalScale", box.localScale);

                    Shader.SetGlobalVector("_SpherePosition", sphere.transform.position);
                    Shader.SetGlobalFloat("_SphereRadius", sphere.transform.localScale.x);

                }

            }
        }

        public override void OnToolDeactivated()
        {
            jobON = false;
            Shader.DisableKeyword("FREE_SELECTION");
            //FreeSelectionUICanvas.SetActive(false);

            if (sphere != null)
            {
                Destroy(sphere);
            }

            if (_controller)
            {
                _controller.TouchpadPressed -= OnTriggerClicked;
                _controller = null;

            }

            SingleSelectionUICanvas.SetActive(false);
            Destroy(pointSelector);

        }

        public override void OnDisabled()
        {
            
        }
    }
}