using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;


public class ToggleNanValues : MonoBehaviour

{
    public Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        toggle.onValueChanged.AddListener(Execute);
    }

    private void Execute(bool value)
    {
        CloudUpdater.instance.HideNaNValues(toggle.isOn);
    }
}
