using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopInterface
{
    public class VolumeRenderingRaymarchingSlider : MonoBehaviour
    {
        public Slider slider;
        public Material mat;
        private void Awake()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(ChangeOpacity);
        }

        private void ChangeOpacity(float value)
        {
            mat.SetFloat("_NumSteps", value*32);
        }
    }
}
