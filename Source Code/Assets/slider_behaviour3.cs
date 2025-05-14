using UnityEngine;
using UnityEngine.UI;
public class slider_behaviour3 : MonoBehaviour
{
    public Slider slider1;
    public Text valueText1;
    private void Start()
    {

        valueText1.text = slider1.value.ToString();
    }
    public void OnSliderValueChanged()
    {

        valueText1.text = slider1.value.ToString();
    }
}