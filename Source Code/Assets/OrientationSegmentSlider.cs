using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
namespace DesktopInterface
{


    public class OrientationSegmentSlider : ISliderScript
    {
        // Start is called before the first frame update
        void Start()
        {
            slider = GetComponent<Slider>();
            InitializeSliderEvent();

        }

        public override void Execute(float value)
        {
            CloudUpdater.instance.ChangeOrientationSegmentSize(value);
        }
    }
}