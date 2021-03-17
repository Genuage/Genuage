using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VR_Interaction
{
    //This Component generates a camera on the associated gameobject, 
    //and link its feed to a rendertexture;
    public class VRCameraViewer 
    {
        public Camera MobileCamera;
        public GameObject Screen;
        public RenderTexture CameraFeed;
        //public GameObject SpherePrefab;
        public GameObject Sphere;

        public void CreateCamera()
        {
            CreateCameraGameObject();
            CreateScreen();
            GameObject obj = new GameObject("Mobile Camera");
            obj.transform.SetParent(Sphere.transform, false);
            MobileCamera = obj.AddComponent<Camera>();
            MobileCamera.targetDisplay = 2;
            //MobileCamera.fieldOfView = 80;
            MobileCamera.nearClipPlane = 0.01f;
            MobileCamera.clearFlags = CameraClearFlags.SolidColor;
            MobileCamera.backgroundColor = Color.black;
            MobileCamera.targetTexture = CameraFeed;
            MobileCamera.forceIntoRenderTexture = true;
            MobileCamera.transform.localPosition = Vector3.zero;
            MobileCamera.transform.localRotation = Quaternion.identity;
            MobileCamera.transform.localScale = Vector3.one;
        }

        private void CreateScreen()
        {
            CameraFeed = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);

            Screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Screen.name = "Screen";
            //Screen.transform.SetParent(this.transform, false);
            //Screen.transform.localPosition = new Vector3(0, 0.15f, 0.05f);
            //Screen.transform.localScale = new Vector3(0.4f, 0.25f, 0.001f);
            //Screen.transform.Rotate(new Vector3(0f, 180f, 0f));
            Screen.GetComponent<MeshRenderer>().material.mainTexture = CameraFeed;

        }
        public void CreateCameraGameObject()
        {
            Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //Sphere.transform.SetParent(this.transform);
            //Sphere.transform.localScale = new Vector3(0.017f, 0.017f, 0.017f);
            //Sphere.transform.localPosition = Vector3.forward * 0.05f;
            //Sphere.transform.localEulerAngles = Vector3.zero;

            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.SetParent(Sphere.transform);
            cylinder.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
            cylinder.transform.localPosition = new Vector3(0f, 0f, 0.494f);
            cylinder.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            cylinder.GetComponent<MeshRenderer>().material.color = Color.red;

        }

        public void DestroyObjects()
        {
            Object.Destroy(Screen);
            Object.Destroy(Sphere);
            MobileCamera = null;
            CameraFeed.DiscardContents();
            Object.Destroy(CameraFeed);

        }
    }
}