using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UpdateGlobalOptions : MonoBehaviour
{
    public Slider VRNearClippingPlaneSlider;
    public Slider BoxGraduationNumberSlider;
    public Slider BoxGraduationLengthSlider;
    //public Dropdown AngleUnitsDropdown;
    public void UpdateOptionsUI()
    {
        VRNearClippingPlaneSlider.value = ApplicationOptions.instance.GetVRCullingDistance();
        BoxGraduationNumberSlider.value = ApplicationOptions.instance.GetDefaultBoxScaleNumber();
    }

    public void UpdateOptions()
    {
        ApplicationOptions.instance.UpdateOptions(VRNearClippingPlaneSlider.value, (int)BoxGraduationNumberSlider.value, BoxGraduationLengthSlider.value);
    }

    private void OnDisable()
    {
        UpdateOptions();
    }
}
