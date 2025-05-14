using Grafic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RawImageClick : MonoBehaviour
{

    public GameObject rawImagePrefab;  
    public Transform parentTransform; 
    public List<Vector3> points;
    private IDcounter[] PointidCounters;

    private void addPoints()
    {
        
            create_list_points l = FindObjectOfType<create_list_points>();

            points = l.points;
            PointidCounters = FindObjectsOfType<IDcounter>();
            Debug.Log(PointidCounters.Length);
            for (int i = 0; i < PointidCounters.Length; i++)
            {

                float transform_x = PointidCounters[i].transform.localPosition.x;
                float transform_y = PointidCounters[i].transform.localPosition.y;

                int prefabID = PointidCounters[i].InstanceID;
                points.Add(new Vector3(transform_x, transform_y, prefabID));
              
            
            
            
            }

        //canvas_script2 ar;
        //ar = FindObjectOfType<canvas_script2>();
        //ar.MeshCreater();
        //line_draw line = FindObjectOfType<line_draw>();
        //line.points = points;
        //line.MeshCreater();
        l.textureStart();
    }
    private void Start()
    {
        addPoints();
    }

    private void Update()
    {
        create_list_points l = FindObjectOfType<create_list_points>();
   
        points = l.points;

        int ok = 1;
        if (Input.GetMouseButtonDown(1))
        {
            RectTransform RT = GetComponent<RectTransform>();
            float Selfwidth = RT.rect.width;
            float Selfheight = RT.rect.height;
            Vector3 SelfLowerCorner = new Vector3(transform.position.x - Selfwidth / 2, transform.position.y - Selfheight / 2, 0);
            Vector3 SelfHigherCorner = new Vector3(transform.position.x + Selfwidth / 2, transform.position.y + Selfheight / 2, 0);
            Vector3 mousePosition = Input.mousePosition;
            Vector3 pos = transform.position;
            //Debug.Log("mousepos : " + mousePosition + " Highcorner : " + SelfHigherCorner + " Lowcorner : " + SelfLowerCorner + " position : " + pos);
            if (Bounded(mousePosition, SelfLowerCorner, SelfHigherCorner))

            {
                for (int a = 0; a < points.Count; a++)
                {
                    if (points[a].x == mousePosition.x && points[a].y == mousePosition.y)
                    {


                        ok = 0;


                        foreach (Transform child in parentTransform)
                        {
                            RectTransform rt = child.GetComponent<RectTransform>();
                            float width = rt.rect.width;
                            float height = rt.rect.height;
                            Vector3 LowerCorner = new Vector3(child.transform.position.x - width / 2, child.transform.position.y - height / 2, 0);
                            Vector3 HigherCorner = new Vector3(child.transform.position.x + width / 2, child.transform.position.y + height / 2, 0);
                            //if (child.transform.position.x == mousePosition.x && child.transform.position.y == mousePosition.y)
                            // Destroy(child.gameObject);
                            //  if (child.transform.position.x == mousePosition.x && child.transform.position.y == mousePosition.y)
                            //   Destroy(child.gameObject);
                            if (Bounded(mousePosition, LowerCorner, HigherCorner))

                            {

                                Destroy(child.gameObject);
                            }
                        }
                        points.RemoveAt(a);
                        canvas_script2 ar;
                        ar = FindObjectOfType<canvas_script2>();
                        ar.MeshCreater();
                        line_draw line = FindObjectOfType<line_draw>();
                        line.points = points;
                        line.MeshCreater();

                    }
                }
                if (ok == 1)
                {



                    GameObject newRawImage = Instantiate(rawImagePrefab, mousePosition, Quaternion.identity);
                    IDcounter prefabInstance = newRawImage.GetComponent<IDcounter>();
                    int prefabID = prefabInstance.InstanceID;
                    points.Add(new Vector3(mousePosition.x, mousePosition.y, prefabID));

                    newRawImage.transform.SetParent(parentTransform);
                    newRawImage.transform.localScale = Vector3.one;

                    //line_draw line = FindObjectOfType<line_draw>();
                    //line.points = points;
                    //line.MeshCreater();
                }
                l.StartSort();

                l.textureStart();
            }
        }
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





