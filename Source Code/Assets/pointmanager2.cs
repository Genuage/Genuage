using Grafic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class pointmanager2 : MonoBehaviour
{    public List<Vector3> points; // List to store the points
    public line_draw l;
    public string[] coordinateValues;
    public InputField coordinatesInput; // Reference to the input field for coordinates
    public InputField coordinatesInput1; // Reference to the input field for new coordinates
    public transfer_function1 t;
    void Start()
    {

        //points = new List<Vector3>();
        //points = l.points;
        //points.Sort((a, b) => a.x.CompareTo(b.x));
   
    
    }
    void StartSort()
    {
        //Grafic.line_draw l = FindObjectOfType<line_draw>();
       // points = l.points;


        points.Sort(CompareByXY);



    }

    private int CompareByXY(Vector3 a, Vector3 b)
    {
        if (a.x < b.x)
        {
            return -1;
        }
        else if (a.x > b.x)
        {
            return 1;
        }
        else
        {
            if (a.y < b.y)
            {
                return -1;
            }
            else if (a.y > b.y)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
    // Function to change the coordinates of a point
    public void ChangeCoordinates()
    {   

        string inputText = coordinatesInput.text; // Get the input text from the input field

        // Parse the input text to extract the new coordinates
        string[] coordinateValues = inputText.Split(' ');
        if (coordinateValues.Length != 2)
        {
            Debug.LogWarning("Invalid coordinates format. Please enter x and y values separated by a comma.");
            return;
        }

        // Convert the coordinate values to floats
        float x, y;
        if (!float.TryParse(coordinateValues[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x) || !float.TryParse(coordinateValues[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y))
        {
            Debug.LogWarning("Invalid coordinate values. Please enter numeric values for x and y.");
            return;
        }
        //newx
        string inputText1 = coordinatesInput1.text; // Get the input text from the input field

        // Parse the input text to extract the new coordinates
        string[] coordinateValues1 = inputText1.Split(' ');
        if (coordinateValues1.Length != 2)
        {
            Debug.LogWarning("Invalid coordinates format. Please enter x and y values separated by a comma.");
            return;
        }

        // Convert the coordinate values to floats
        float newx, newy;
        if (!float.TryParse(coordinateValues1[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out newx) || !float.TryParse(coordinateValues1[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out newy))
        {
            Debug.LogWarning("Invalid coordinate values. Please enter numeric values for x and y.");
            return;
        }

        bool pointUpdated = false;
        for (int w = 0; w < points.Count; w++)
        {
            if (points[w].x == x && points[w].y == y)
            {
                //l.points[w].Set(newx, newy);
                // points[w].Set(newx, newy);
                points.RemoveAt(w);
                Vector2 newPoint = new Vector2(newx, newy);
                points.Add(newPoint);
                StartSort();
                Debug.Log("Point (" + x + ", " + y + ") updated with (" + points[w].x + ", " + points[w].y + ")");
                t.textureStart();
                pointUpdated = true;
            }
        }

    
        canvas_script2 a;
        a = FindObjectOfType<canvas_script2>();
        a.MeshCreater();
        l.MeshCreater();
    }

    int ct_virgula = 0;
    public void AddPoint()
    {
     
        string inputText = coordinatesInput.text; // Get the input text from the input field
        string[] coordinateValues = inputText.Split(' ');
        if (coordinateValues.Length != 2)
        {
            Debug.LogWarning("Invalid coordinates format. Please enter x and y values separated by a comma.");
            return;
        }

        // Convert the coordinate values to floats
        float x, y;

        if (!float.TryParse(coordinateValues[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x) || !float.TryParse(coordinateValues[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y))
        {
            Debug.LogWarning("Invalid coordinate values. Please enter numeric values for x and y.");
            return;
        }
        

        points.Add(new Vector2(x, y));
         StartSort();
        canvas_script2  a;
        a = FindObjectOfType<canvas_script2>();
        a.MeshCreater();
        l.MeshCreater();
        Debug.Log("New point added at coordinates: (" + x + ", " + y + ")!");
        t.textureStart();
    }

    public void RemovePoint()
    {
      
        string inputText = coordinatesInput.text;

        // Parse the input text to extract the new coordinates
        string[] coordinateValues = inputText.Split(' ');
        if (coordinateValues.Length != 2)
        {
            Debug.LogWarning("Invalid coordinates format. Please enter x and y values separated by a comma.");
            return;
        }

        // Convert the coordinate values to floats
        float x, y;
        if (!float.TryParse(coordinateValues[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x) || !float.TryParse(coordinateValues[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y))
        {
            Debug.LogWarning("Invalid coordinate values. Please enter numeric values for x and y.");
            return;
        }
     
        if (points.Count > 0)
        {
            for (int w = 0; w < points.Count; w++)
            {
                if (points[w].x == x && points[w].y == y)
                {
                    points.RemoveAt(w);
                    Debug.Log("Point deleted at coordinates: (" + x + ", " + y + ")");
                    StartSort();
                    t.textureStart();

                }

            }
        }
        else
        {
            Debug.LogWarning("No points available to remove");
        }
        canvas_script2 a;
        a = FindObjectOfType<canvas_script2>();
        a.MeshCreater();
        l.MeshCreater();
    }
}

