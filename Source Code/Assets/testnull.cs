using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testnull : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gO;

    private void Awake()
    {
        Debug.Log(gO);
    }
}
