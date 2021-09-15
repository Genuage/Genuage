using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
            CloudSelector.instance.OnSelectionChange += UpdateRange;
            CloudUpdater.instance.OnCloudReloaded += UpdateRange;
            UpdateRange(0);
            InitializeSliderEvent();
        }
        /**
        public override void InputLabelChanged(string value)
        {
            float new_value = Single.Parse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
            int intvalue = Mathf.RoundToInt(new_value);
            slider.value = new_value;

        }
        **/
        private void UpdateRange(int id)
        {
            if (!CloudSelector.instance.noSelection)
            {
                CloudData data = CloudUpdater.instance.LoadCurrentStatus();
                XRange = data.globalMetaData.xMax - data.globalMetaData.xMin;
                slider.minValue = XRange / 500;
                slider.maxValue = XRange;
                slider.value = data.globalMetaData.ScaleBarDistanceX;
                Execute(slider.value);

            }
        }

        public override void Execute(float value)
        {
            CloudUpdater.instance.ChangeBoxGraduationsX(value);
            if (_field)
            {
                _field.text = (slider.value).ToString();
            }

        }
    }
}