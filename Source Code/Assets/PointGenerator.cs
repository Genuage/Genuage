//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;
public class PointGenerator : MonoBehaviour
{
    public int pointNumber = 0;
    public int trajectoryNumber = 0;
    public int pointspertrajectory = 0;
    public int minpointspertrajectory = 0;
    public int maxpointspertrajectory = 0;


    private List<float[]> pointList;
    private List<float> xValues;
    private List<float> yValues;
    private List<float> zValues;
    private List<float> trajValues;
    private List<float> timeValues;

    public void Run()
    {
        pointList = new List<float[]>();
        xValues = new List<float>();
        yValues = new List<float>();
        zValues = new List<float>();
        trajValues = new List<float>();
        timeValues = new List<float>();
        float nbr = (pointNumber / pointspertrajectory);
        trajectoryNumber = (int)Mathf.Round(nbr);
        Debug.Log(trajectoryNumber);
        for (int i = 0; i < trajectoryNumber; i++)
        {
            createtrajectory(i, pointspertrajectory);
        }
        pointList.Add(xValues.ToArray());
        pointList.Add(yValues.ToArray());
        pointList.Add(zValues.ToArray());
        pointList.Add(trajValues.ToArray());
        pointList.Add(timeValues.ToArray());

        CloudLoader.instance.LoadFromConnection(pointList);
    }

    private void createtrajectory(int id, int point_number)
    {
        //int StartTime = Random.Range(0, 200);

        for(int i = 0; i < point_number; i++)
        {
            createpoint(id, i);
        }
    }

    private void createpoint(float trajectory_id, float time)
    {
        //Random random = new Random();
        //float[] pointdata = new float[5];
        xValues.Add(Random.Range(0f, 1f));
        yValues.Add(Random.Range(0f, 1f));
        zValues.Add(Random.Range(0f, 1f));
        trajValues.Add(trajectory_id);
        timeValues.Add(time);
        
    }
}
