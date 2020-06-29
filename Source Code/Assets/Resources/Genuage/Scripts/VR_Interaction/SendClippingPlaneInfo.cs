using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendClippingPlaneInfo : MonoBehaviour
{
    public string Keyword1;
    public string Keyword2;
    public string Keyword3;

    public bool jobON = false;
    // Update is called once per frame
    void Update()
    {
        if (jobON)
        {
            Shader.SetGlobalVector(Keyword1, this.transform.position);
            Shader.SetGlobalVector(Keyword2, this.transform.forward);
            //Shader.EnableKeyword(Keyword3);

        }
    }

    public void enableClippingPlane()
    {
        Shader.EnableKeyword(Keyword3);
        jobON = true;

    }

    void OnDisable()
    {
        if (gameObject.activeInHierarchy == false)
        {
            Shader.DisableKeyword(Keyword3);
            jobON = false;
        }
    }

    void OnEnable()
    {
        if (gameObject.activeInHierarchy == true)
        {
            Shader.EnableKeyword(Keyword3);
            jobON = true;
        }
    }

    void OnDestroy()
    {
        //Shader.DisableKeyword(Keyword3);
        jobON = false;
    }
}
