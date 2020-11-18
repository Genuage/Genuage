using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Display;
using Data;
using Zinnia.Data.Type;


public class ApplicationOptions : MonoBehaviour
{
    private const float VRDEFAULTCULLINGDISTANCE = 0.15f;

    public static ApplicationOptions instance = null;

    private float VRCullingDistance = VRDEFAULTCULLINGDISTANCE;
    //private AngleUnit DefaultAngleUnit = AngleUnit.DEGREES;
    private bool BoxGraduationActivated = false;
    private int BoxGraduationNumber = 10; //number of ticks on each dimensions
    private float BoxGraduationLength = 0.05f; //Length of those ticks
    


    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }

    public int GetDefaultBoxScaleNumber()
    {
        return BoxGraduationNumber;
    }

    public float GetVRCullingDistance()
    {
        return VRCullingDistance;
    }

    public float GetGraduationLength()
    {
        return BoxGraduationLength;
    }

    public bool GetGraduationActivated()
    {
        return BoxGraduationActivated;
    }

    public void UpdateOptions(float vrcullingDistance, int boxscalenumber, float graduationLength)
    {
        UpdateVRCullingDistance(vrcullingDistance);
        UpdateBoxGraduationNumber(boxscalenumber);
        UpdateBoxGraduationLength(graduationLength);
    }

    public void UpdateVRCullingDistance(float vrcullingDistance)
    {
        CameraManager.instance.vr_camera.nearClipPlane = vrcullingDistance;

    }

    public void UpdateBoxGraduationNumber(int boxgraduationnumber)
    {
        BoxGraduationNumber = boxgraduationnumber;
        CloudUpdater.instance.ReloadAllBoxes();
    }

    public void UpdateBoxGraduationLength(float boxgraduationlength)
    {
        BoxGraduationLength = boxgraduationlength;
        CloudUpdater.instance.ReloadAllBoxes();
    }
    
}
