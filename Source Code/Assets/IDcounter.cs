using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDcounter : MonoBehaviour
{
    public static int nextID = 1; 

    private int instanceID;

    public int InstanceID
    {
        get { return instanceID; }
    }

    private void Awake()
    {
        instanceID = nextID;
        nextID++;
    }
}
