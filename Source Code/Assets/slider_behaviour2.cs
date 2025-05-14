
using UnityEngine;
using UnityEngine.UI;
public class slider_behaviour2 : MonoBehaviour
{
    public Slider slider;
    public Text valueText;
    private void Start()
    {

        valueText.text = slider.value.ToString();
    }
    public void OnSliderValueChanged()
    {

        valueText.text = "Opacity : "+slider.value.ToString();

    }
}

