using UnityEngine;
using UnityEngine.UI;
public class slider_behaviour : MonoBehaviour
{
    public Slider slider;
    public Text valueText;
    private void Start()
    {

        valueText.text = "Raymarching Steps : " + (32 * slider.value).ToString();
    }
    public void OnSliderValueChanged()
    {

        valueText.text = "Raymarching Steps : "+(32*slider.value).ToString();
    }
}