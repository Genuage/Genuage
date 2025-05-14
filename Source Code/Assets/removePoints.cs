using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class removePoints : MonoBehaviour
{
    public transfer_function1 pointManager;
    public float x,y;
   public void RemoveElementByIndex(float x, float y)
    {
        pointManager.RemovePoint(x,y);
    }

   public void Start()
    {  
        
        RemoveElementByIndex( x,  y);
    }

 

}
