using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UpdateGlobalOptions : MonoBehaviour
{
    public Slider VRNearClippingPlaneSlider;
    public Toggle GraduationToggle;
    public Toggle GraduationNumbersToggle;
    //public Slider BoxGraduationNumberSlider;
    public Slider BoxGraduationLengthSlider;
    //public Dropdown AngleUnitsDropdown;
    public void UpdateOptionsUI()
    {
        //VRNearClippingPlaneSlider.value = ApplicationOptions.instance.GetVRCullingDistance();
        //BoxGraduationNumberSlider.value = ApplicationOptions.instance.GetDefaultBoxScaleNumber();
    }

    public void UpdateOptions()
    {
        ApplicationOptions.instance.UpdateOptions(VRNearClippingPlaneSlider.value);
        ApplicationOptions.instance.UpdateBoxGraduationActivation(GraduationToggle.isOn);
        ApplicationOptions.instance.UpdateBoxGraduationNumbersActivation(GraduationNumbersToggle.isOn);
        ApplicationOptions.instance.UpdateBoxGraduationLength(BoxGraduationLengthSlider.value);

    }

    private void OnDisable()
    {
        UpdateOptions();
    }
}
