using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInput : MonoBehaviour
{
    int ct = 0;
    private List<Vector3> pointsList;

    private void Start()
    {
        
        pointsList = new List<Vector3>();
        AskForPoints();
    }

    private void AskForPoints()
    {
        while (true)
        {
            Vector3 point1 = GetPointFromUser("Enter Point 1 (x, y): ");
            Vector3 point2 = GetPointFromUser("Enter Point 2 (x, y): ");

            pointsList.Add(point1);
            pointsList.Add(point2);

            string continueInput = GetUserInput("Do you want to add more points? (Y/N): ");
            if (continueInput.ToLower() != "y")
                break;
        }

        PrintPoints();
    }

    private Vector3 GetPointFromUser(string prompt)
    {
        Vector3 point = Vector3.zero;
        bool validInput = false;

        while (!validInput)
        {
            string input = GetUserInput(prompt);

            string[] coordinates = input.Split(',');
            if (coordinates.Length != 2)
            {
                Debug.Log("Invalid input");
                continue;
            }

            float x, y;
            if (!float.TryParse(coordinates[0], out x) || !float.TryParse(coordinates[1], out y) )
            {
                Debug.Log("Invalid input! Please enter valid numeric values.");
                continue;
            }

            point = new Vector3(x, y, ct);
            ct++;
            validInput = true;
        }

        return point;
    }

    private string GetUserInput(string prompt)
    {
        Debug.Log(prompt);
        return Console.ReadLine();
    }

    private void PrintPoints()
    {
        Debug.Log("Points List:");
        foreach (Vector3 point in pointsList)
        {
            Debug.Log(point);
        }
    }
}

