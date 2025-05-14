using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{

    public class BoxZScaleSlider : ISliderScript
    {
        private float ZRange;

        private void Awake()
        {
            slider = this.GetComponent<Slider>();
            
            CloudSelector.instance.OnSelectionChange += UpdateRange;
            CloudUpdater.instance.OnCloudReloaded += UpdateRange;
            //UpdateRange(0);
            InitializeSliderEvent();

        }

        private void UpdateRange(int id)
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            ZRange = data.globalMetaData.zMax - data.globalMetaData.zMin;
            slider.minValue = ZRange / 10;
            slider.maxValue = ZRange;
            slider.value = data.globalMetaData.ScaleBarDistanceZ;
            //slider.value = data.globalMetaData.ScaleBarNumberX;
            if (_field)
            {
                _field.text = (slider.value).ToString();
            }

        }

        public override void Execute(float value)
        {
            CloudUpdater.instance.ChangeBoxGraduationsZ(value);
            if (_field)
            {
                _field.text = (slider.value).ToString();
            }

        }
    }
}