using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendClippingPlaneInfo : MonoBehaviour
{
    public string PositionKeyword;
    public string PlaneNormalKeyword;
    public string ClipPlaneEnableKeyword;

    public bool jobON = false;
    // Update is called once per frame
    void Update()
    {
        if (jobON)
        {
            Shader.SetGlobalVector(PositionKeyword, this.transform.position);
            Shader.SetGlobalVector(PlaneNormalKeyword, this.transform.forward);
            //Shader.EnableKeyword(Keyword3);

        }
    }

    public void enableClippingPlane()
    {
        Shader.EnableKeyword(ClipPlaneEnableKeyword);
        jobON = true;

    }

    void OnDisable()
    {
        if (gameObject.activeInHierarchy == false)
        {
            Shader.DisableKeyword(ClipPlaneEnableKeyword);
            jobON = false;
        }
    }

    void OnEnable()
    {
        if (gameObject.activeInHierarchy == true)
        {
            Shader.EnableKeyword(ClipPlaneEnableKeyword);
            jobON = true;
        }
    }

    void OnDestroy()
    {
        //Shader.DisableKeyword(Keyword3);
        jobON = false;
    }
}
