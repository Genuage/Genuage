using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider_component : MonoBehaviour
{
    public Material material;
    public Slider slider;
    void Start()
    {
        slider.onValueChanged.AddListener(OnValueChanged);
    }


    void OnValueChanged(float value)
    {
        material.SetFloat("_Threshold",slider.value);
    
    }
}
