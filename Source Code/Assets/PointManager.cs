using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
public class PointManager : MonoBehaviour
{
    public transfer_function1 pointManager;
    public float x;
    public float y;

    public void AddPoints(float x, float y)
    {
        pointManager.AddPoints(x, y);
        
    }
   public void Start()
    {   
        AddPoints(x,y);
 




    }


}