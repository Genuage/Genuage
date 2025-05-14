using Grafic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FollowMouse : MonoBehaviour
{   
    private bool isMoving;
    public RawImage rawimg;
    public float moveSpeed = 2f;
    public List<Vector3> points ;
    public Material mat;

    Vector2 previousMousePosition;
    Vector2 deltaMousePosition;
  
    public void Update()
    {
        create_list_points l = FindObjectOfType<create_list_points>();
        points = l.points;

        RectTransform rt = transform.parent.GetComponent<RectTransform>();
        float width = rt.rect.width;
        float height = rt.rect.height;
        Vector3 LowerCorner = new Vector3(-width / 2, -height / 2, 0);
        Vector3 HigherCorner = new Vector3(width / 2, height / 2, 0);

        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out localMousePosition);
        

        if (Input.GetMouseButtonDown(0))
        {
            isMoving = true;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
        }
        RectTransform rt1 = transform.GetComponent<RectTransform>();
        float width1 = rt1.rect.width;
        float height1 = rt1.rect.height;
        float coordx = transform.localPosition.x-width1/2;
        float coordy=transform.localPosition.y - height1 / 2; ;
        float coordx1 = transform.localPosition.x + width1 / 2; 
        float coordy1 = transform.localPosition.y + height1 / 2; 
      
        Vector3 LowerCorner1 = new Vector3(coordx,coordy, 0);
        Vector3 HigherCorner1 = new Vector3(coordx1, coordy1, 0);
        
        if (isMoving && Bounded(localMousePosition,LowerCorner1,HigherCorner1) && EventSystem.current.IsPointerOverGameObject() && Bounded(localMousePosition, LowerCorner, HigherCorner))
        {
    


            Vector3 oldposition = transform.localPosition;

            transform.localPosition = Vector2.MoveTowards(oldposition, localMousePosition, moveSpeed * Time.deltaTime);
            IDcounter prefabInstance = gameObject.GetComponent<IDcounter>();
            int prefabID = prefabInstance.InstanceID;
            l.changeP(prefabID, transform.localPosition.x, transform.localPosition.y);
            l.StartSort();
            l.textureStart();
            //line_draw line = FindObjectOfType<line_draw>();
            //line.points = points;
            //line.MeshCreater();
        }

        previousMousePosition = localMousePosition;

    }




    bool Bounded(Vector3 mousePosition, Vector3 LowerCorner, Vector3 HigherCorner)
    {


        if ((mousePosition.x > LowerCorner.x) && (mousePosition.y > LowerCorner.y))
        {
            if ((mousePosition.x < HigherCorner.x) && (mousePosition.y < HigherCorner.y))
            {

                return true;
            }


        }
        return false;

    }
    }

 



