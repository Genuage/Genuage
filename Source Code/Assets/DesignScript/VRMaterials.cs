using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMaterials : MonoBehaviour
{
    public static VRMaterials instance = null;
    public Material _default;
    public Material _selected_red;
    

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
