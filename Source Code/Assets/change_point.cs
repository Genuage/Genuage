using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_point : MonoBehaviour
{
    public transfer_function1 pointManager;
    public float x;
    public float y;
    public float newX;
    public float newY;
  public void Start()
    {
       Changepoint(x, y,newX,newY);
    }
   void Changepoint(float x, float y, float newX, float newY)
   {
       pointManager.changeP( x, y,  newX,  newY);
}
}
