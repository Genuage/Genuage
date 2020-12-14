using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{

    public class BoxZScaleSlider : SliderScript
    {
        private float ZRange;

        private void Start()
        {
            slider = this.GetComponent<Slider>();
            InitializeSliderEvent();
            CloudSelector.instance.OnSelectionChange += UpdateRange;
            CloudUpdater.instance.OnCloudReloaded += UpdateRange;
            if (_field)
            {
                _field.onEndEdit.RemoveListener(InputLabelChanged);
            }
            UpdateRange(0);

        }

        private void UpdateRange(int id)
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            ZRange = data.globalMetaData.zMax - data.globalMetaData.zMin;
            slider.value = data.globalMetaData.ScaleBarNumberZ;
            if (_field)
            {
                _field.text = (ZRange / slider.value).ToString();
            }
        }

        public override void Execute(float value)
        {
            CloudUpdater.instance.ChangeBoxGraduationsZ(value);
            if (_field)
            {
                _field.text = (ZRange / slider.value).ToString();
            }

        }
    }
}