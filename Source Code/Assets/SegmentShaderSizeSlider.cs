using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
namespace DesktopInterface
{

    public class SegmentShaderSizeSlider : ISliderScript
    {
        private void Awake()
        {
            slider = GetComponent<Slider>();
            InitializeSliderEvent();

        }

        public override void Execute(float value)
        {
            CloudUpdater.instance.ChangeTrajectorySegmentSize(value);
            //Shader.SetGlobalFloat("_Thickness", (float)value/0.003f);
            if (_field)
            {
                _field.text = value.ToString();
            }
        }





    }

}