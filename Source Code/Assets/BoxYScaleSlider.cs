using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
namespace DesktopInterface
{

    public class BoxYScaleSlider : ISliderScript
    {
        private float YRange;

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
            YRange = data.globalMetaData.yMax - data.globalMetaData.yMin;
            slider.minValue = YRange / 10;
            slider.maxValue = YRange;
            slider.value = data.globalMetaData.ScaleBarDistanceY;
            
            if (_field)
            {
                _field.text = (slider.value).ToString();
            }
        }

        public override void Execute(float value)
        {
            CloudUpdater.instance.ChangeBoxGraduationsY(value);
            if (_field)
            {
                _field.text = (slider.value).ToString();
            }

        }
    }
}
