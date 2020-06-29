using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TogglePointSize : MonoBehaviour
{
    public Toggle toggle; 
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(Execute);
    }

    // Update is called once per frame
    void Execute(bool b)
    {
        if (b)
        {
            Shader.EnableKeyword("POINTSIZE");
        }
        else
        {
            Shader.DisableKeyword("POINTSIZE");
        }
    }
}
