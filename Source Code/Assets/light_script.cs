using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class light_script : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_lightPosition", gameObject.transform.position);
        Shader.SetGlobalVector("_direction", gameObject.transform.forward);
    }
}
