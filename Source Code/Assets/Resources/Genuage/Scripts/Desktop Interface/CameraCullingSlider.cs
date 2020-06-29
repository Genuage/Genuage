using Display;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace DesktopInterface
{


    public class CameraCullingSlider : SliderScript
    {
        private void Awake()
        {
            slider = GetComponent<Slider>();
            InitializeSliderEvent();
            slider.value = CameraManager.instance.vr_camera.nearClipPlane;
        }


        public override void Execute(float value)
        {
            //CameraManager.instance.desktop_camera.nearClipPlane = value;
            CameraManager.instance.vr_camera.nearClipPlane = value;

            if (_field)
            {
                _field.text = value.ToString();
            }
        }

    }
}