using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{

    public class BoxXScaleSlider :SliderScript
    {
        private float XRange;

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
            XRange = data.globalMetaData.xMax - data.globalMetaData.xMin;
            slider.value = data.globalMetaData.ScaleBarNumberX;
            if (_field)
            {
                _field.text = (XRange / slider.value).ToString();
            }
        }

        public override void Execute(float value)
        {
            CloudUpdater.instance.ChangeBoxGraduationsX(value);
            if (_field)
            {
                _field.text = (XRange / slider.value).ToString();
            }

        }
    }
}