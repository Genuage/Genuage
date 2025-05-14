using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class slider_component3 : MonoBehaviour
{
    public Material material1;
    public Slider slider1;
    void Start()
    {
        slider1.onValueChanged.AddListener(OnValueChanged);
    }


    void OnValueChanged(float value)
    {
        material1.SetFloat("_Transparency", slider1.value);

    }
}
