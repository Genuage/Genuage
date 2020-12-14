using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
namespace DesktopInterface
{

    public class BoxYScaleSlider : SliderScript
    {
        private float YRange;

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
            YRange = data.globalMetaData.yMax - data.globalMetaData.yMin;
            slider.value = data.globalMetaData.ScaleBarNumberY;
            if (_field)
            {
                _field.text = (YRange / slider.value).ToString();
            }
        }

        public override void Execute(float value)
        {
            CloudUpdater.instance.ChangeBoxGraduationsY(value);
            if (_field)
            {
                _field.text = (YRange / slider.value).ToString();
            }

        }
    }
}
