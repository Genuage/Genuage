using UnityEngine;
using UnityEngine.EventSystems;

public class PointsMouse : MonoBehaviour
{

    public GameObject pointPrefab;
    public Transform squareArea;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseWithinSquareArea())
            {
           
                   Vector3 mousePosition = Input.mousePosition;

                CreatePoint(mousePosition);

            }
        }
    }

    void CreatePoint(Vector3 position)
    {   
        Instantiate(pointPrefab, position, Quaternion.identity);
        Debug.Log("Point created at: " + position);
    }





    bool IsMouseWithinSquareArea()
    {
        Vector3 mousePosition = Input.mousePosition;

        Vector3 squareAreaPosition = squareArea.position;

        Vector3 squareAreaScale = squareArea.localScale;

        float minX = squareAreaPosition.x - squareAreaScale.x / 2f;
        float maxX = squareAreaPosition.x + squareAreaScale.x / 2f;
        float minY = squareAreaPosition.y - squareAreaScale.y / 2f;
        float maxY = squareAreaPosition.y + squareAreaScale.y / 2f;
   
        if (Input.GetMouseButtonDown(0))
        {
   
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
        }
        return false;

    }
    


}
