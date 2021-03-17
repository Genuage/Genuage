using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using IO;

namespace VR_Interaction
{
    public class VRScreenCaptureTool : IControllerTool
    {
        private VRCameraViewer VRCamera;
        protected VRTK_ControllerEvents _controller;
        BitmapEncoder encoder;

        public override void OnToolActivated()
        {
            _controller = GetComponent<VRTK_ControllerEvents>();
            _controller.TouchpadPressed += OnTriggerClicked;
            VRCamera = new VRCameraViewer();
            VRCamera.CreateCamera();
            VRCamera.Sphere.transform.SetParent(this.transform);
            VRCamera.Sphere.transform.localScale = new Vector3(0.017f, 0.017f, 0.017f);
            VRCamera.Sphere.transform.localPosition = (Vector3.forward+Vector3.up) * 0.05f;
            VRCamera.Sphere.transform.localEulerAngles = Vector3.zero;
            VRCamera.Screen.transform.SetParent(this.transform, false);
            VRCamera.Screen.transform.localPosition = new Vector3(0, 0.15f, 0.05f);
            VRCamera.Screen.transform.localScale = new Vector3(0.4f, 0.25f, 0.001f);
            VRCamera.Screen.transform.Rotate(new Vector3(0f, 180f, 0f));

        }

        public void OnTriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            int screenWidth = VRCamera.MobileCamera.pixelWidth;
            int screenHeight = VRCamera.MobileCamera.pixelHeight;

            Texture2D tempTexture2D = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);

            RenderTexture.active = VRCamera.CameraFeed;
            tempTexture2D.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
            RenderTexture.active = null;
            string persistentDataPath = Application.dataPath + "/Records/ScreenCaptures/";
            if (!System.IO.Directory.Exists(persistentDataPath)) 
            {
                System.IO.Directory.CreateDirectory(persistentDataPath);
            }
            DateTime now = DateTime.Now;
            string outputname = now.Year.ToString() + "-" + now.Month.ToString() + "-" + now.Day.ToString() + "-" + 
                                now.Hour.ToString() + "-" + now.Minute.ToString() + "-" + now.Second.ToString();

            string path = persistentDataPath + "Capture " + outputname + ".bmp";

            // Dequeue the frame, encode it as a bitmap, and write it to the file
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                BitmapEncoder.WriteBitmap(fileStream, screenWidth, screenHeight, tempTexture2D.GetRawTextureData());
                fileStream.Close();
            }

        }

        public override void OnToolDeactivated()
        {
            _controller.TouchpadPressed -= OnTriggerClicked;
            VRCamera.DestroyObjects();
            VRCamera = null;
            //Destroy(VRCamera);
        }
        public override void OnDisabled()
        {
            throw new System.NotImplementedException();
        }
    }
}