using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Data;
using static UnityEngine.UI.CanvasScaler;
using Boo.Lang;
using UnityEditor;
using System;
//using System.Numerics;

namespace DesktopInterface
{


    public class ConeButton : IButtonScript
    {
        const float DEGREE_MAX = 180f;
        const float RADIANS_MAX = 6.28319f;

        private int cone_subdivisions = 15;
        private int circle_subdivisions = 15;
        private float ConeSize =  0.01f;
        public GameObject GO;

        List<Vector3> vertices; 
        List<int> triangles;
        List<Vector2> UV1List = new List<Vector2>();
        public Texture2D texture;

        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
        }

        public override void Execute()
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();

            //construct end of cone like in the orientation shader for each point
            AngleUnit angle_unit = data.globalMetaData.angleUnit;
            List<float> xvalues = new List<float>();
            List<float> yvalues = new List<float>();
            List<float> zvalues = new List<float>();

            List<Vector2> uv = new List<Vector2>();

            List<Vector2> UV2List = new List<Vector2>();
            List<Vector2> UV3List = new List<Vector2>();

            //List<Color> color = new List<Color>();
            List<float> coloruv = new List<float>();

            List<Vector3> FrontConeBaseCenterPointList = new List<Vector3>();
            List<Vector3> BackConeBaseCenterPointList = new List<Vector3>();

            List<Vector3> ConeEndPointList = new List<Vector3>();

            List<Vector3> FrontConePerpendicularPointList = new List<Vector3>();
            List<Vector3> BackConePerpendicularPointList = new List<Vector3>();

            List<Vector3> FrontConeCirclePointsList = new List<Vector3>();
            List<Vector3> BackConeCirclePointsList = new List<Vector3>();

            vertices = new List<Vector3>();
            triangles = new List<int>();

            //System.Random random = new System.Random();
            int cpt = 0;

            foreach (var kvp in data.pointDataTable)
            {
                if (data.pointMetaDataTable[kvp.Key].isHidden == false)
                {
                    cpt++;


                    float theta = kvp.Value.theta_angle;
                    float wobble = kvp.Value.wobble_angle;
                    //float theta = random.Next(0, 180);

                    Vector3 HypothenusPoint = CalculateHypothenusPoint2D(theta, wobble, angle_unit, ConeSize);
                    Vector3 SquareAnglePoint = CalculateRightAnglePoint2D(theta, wobble, angle_unit, ConeSize);

                    //CalculateOrientationAngle2D(HypothenusPoint, SquareAnglePoint, theta, wobble, angle_unit,ConeSize);

                    //Debug.Log(HypothenusPoint);
                    //Debug.Log(SquareAnglePoint);
                    Vector3 FrontConeHypothenusPoint = kvp.Value.normed_position + HypothenusPoint;
                    Vector3 BackConeHypothenusPoint = kvp.Value.normed_position - HypothenusPoint;






                    Vector3 FrontConeCircleCenterPoint = kvp.Value.normed_position + SquareAnglePoint;
                    Vector3 BackConeCircleCenterPoint = kvp.Value.normed_position - SquareAnglePoint;

                    AddCircleMesh(kvp.Value.normed_position, FrontConeCircleCenterPoint, FrontConeHypothenusPoint, theta, wobble, angle_unit, true);
                    AddCircleMesh(kvp.Value.normed_position, BackConeCircleCenterPoint, BackConeHypothenusPoint, theta, wobble, angle_unit, false);


                }
            }
            //Debug.Log("Verts : " + vertices.Count + " Trigs : " + triangles.Count + " iterations = " + cpt);
            //create all circle points by rotating from the first point



            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.vertices = vertices.ToArray();
            mesh.SetIndices(triangles.ToArray(), MeshTopology.Triangles, 0);
            mesh.uv = UV1List.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();


            //GO.GetComponent<MeshFilter>().mesh = mesh;


            GameObject child = new GameObject();
            child.transform.SetParent(data.transform, false);
            child.AddComponent<MeshFilter>();
            child.AddComponent<MeshRenderer>();
            child.GetComponent<MeshFilter>().mesh = mesh;
            Material material = new Material(Shader.Find("Genuage/MapSurface"));
            material.SetColor("_Color", new Color(1f,1f,1f,0.8f));
            material.SetTexture("_MainTex", texture);




            child.GetComponent<MeshRenderer>().material = material;
            data.orientationObject = child;
        }

        private void AddCircleMesh(Vector3 point_position, Vector3 center, Vector3 hypothenuspoint, float theta, float wobble, AngleUnit unit, bool FrontCone)
        {
            float UV_Division = 0.0f;
            if(unit == AngleUnit.DEGREES)
            {
                UV_Division = theta / DEGREE_MAX;
            }
            else
            {
                UV_Division = theta / RADIANS_MAX;
            }

            List<Vector3> LastCircleList = new List<Vector3>();
            List<int> LastCircleIDs = new List<int>();
            //Add base circle
            vertices.Add(point_position);
            int ConeEndPointID = vertices.Count - 1;
            UV1List.Add(new Vector3(UV_Division, 0f));

            vertices.Add(center);
            int FrontConeCenterPointID = vertices.Count - 1;
            UV1List.Add(new Vector3(UV_Division, 0f));

            vertices.Add(hypothenuspoint);
            int FrontConeFirstCirclePoint = vertices.Count - 1;
            UV1List.Add(new Vector3(UV_Division, 0f));


            LastCircleList.Add(hypothenuspoint);
            LastCircleIDs.Add(FrontConeFirstCirclePoint);

            float angleincrement = 360f / cone_subdivisions;
            int cpt = 0;
            for (int i = 1; i <= cone_subdivisions; i++)
            {
                Vector3 offset = hypothenuspoint - center;
                Vector3 rotationAxis = center - point_position;
                var rotation = Quaternion.AngleAxis(i * angleincrement, rotationAxis);
                Vector3 newOffset = rotation * offset;
                Vector3 newFrontCirclePoint = center + newOffset;
                //FrontConeCirclePointsList.Add(newFrontCirclePoint);
                vertices.Add(newFrontCirclePoint);
                UV1List.Add(new Vector3(UV_Division, 0f));

                LastCircleList.Add(newFrontCirclePoint);
                LastCircleIDs.Add(vertices.Count - 1);

                //Debug.Log("new cone circle point : " + newFrontCirclePoint.ToString("F4"));

                //base triangle
                //triangles.Add(vertices.Count - 1);
                //triangles.Add(FrontConeCenterPointID);
                //triangles.Add(vertices.Count - 2);


                //length triangle
                triangles.Add(vertices.Count - 1);
                triangles.Add(vertices.Count - 2);
                triangles.Add(ConeEndPointID);



                //Vector3 newBackCirclePoint = Quaternion.AngleAxis(angleincrement * i, BackConeCenterVector) * BackFirstCirclePoint;
                //BackConeCirclePointsList.Add(newBackCirclePoint);

            }

            List<Vector3> newCirclePointsList = new List<Vector3>();
            List<int> newCircleIDs = new List<int>();

            float increment = wobble / circle_subdivisions;

            //Now Make all circles recursively
            for (int i = 1; i < circle_subdivisions; i++)
            {
                newCirclePointsList = new List<Vector3>();
                newCircleIDs = new List<int>();

                Vector3 newCenterPoint = CalculateRightAnglePoint2D(theta, wobble - (increment * (i)), unit, ConeSize);

                Vector3 newHypothenusPoint = CalculateHypothenusPoint2D(theta, wobble - (increment * (i)), unit, ConeSize);
                Vector3 ConeHypothenusPoint;
                Vector3 ConeCenterPoint;

                if (FrontCone)
                {
                    ConeHypothenusPoint = point_position + newHypothenusPoint;
                    ConeCenterPoint = point_position + newCenterPoint;

                }
                else
                {
                    ConeHypothenusPoint = point_position - newHypothenusPoint;
                    ConeCenterPoint = point_position - newCenterPoint;

                }


                vertices.Add(ConeHypothenusPoint);
                UV1List.Add(new Vector3(UV_Division, 0f));

                newCirclePointsList.Add(ConeHypothenusPoint);
                newCircleIDs.Add(vertices.Count - 1);


                for (int j = 1; j<= cone_subdivisions; j++)
                {

                    Vector3 offset = ConeHypothenusPoint - ConeCenterPoint;
                    Vector3 rotationAxis = ConeCenterPoint - point_position;
                    var rotation = Quaternion.AngleAxis(j * angleincrement, rotationAxis);
                    Vector3 newOffset = rotation * offset;
                    Vector3 newFrontCirclePoint = ConeCenterPoint + newOffset;
                    //FrontConeCirclePointsList.Add(newFrontCirclePoint);
                    vertices.Add(newFrontCirclePoint);
                    UV1List.Add(new Vector3(UV_Division, 0f));

                    //Debug.Log("x = "+ newFrontCirclePoint.x + "y = "+ newFrontCirclePoint.y+"z = "+ newFrontCirclePoint.z);
                    newCirclePointsList.Add(newFrontCirclePoint);
                    newCircleIDs.Add(vertices.Count - 1);

                }

                for(int k = 0; k < cone_subdivisions; k++)
                {
                    
                    triangles.Add(LastCircleIDs[k]);
                    triangles.Add(LastCircleIDs[k+1]);
                    triangles.Add(newCircleIDs[k + 1]);

                    triangles.Add(LastCircleIDs[k]);
                    triangles.Add(newCircleIDs[k+1]);
                    triangles.Add(newCircleIDs[k]);
                    

                }
                
                triangles.Add(LastCircleIDs[0]);
                triangles.Add(LastCircleIDs[LastCircleIDs.Count-1]);
                triangles.Add(newCircleIDs[newCircleIDs.Count - 1]);

                triangles.Add(LastCircleIDs[0]);
                triangles.Add(newCircleIDs[newCircleIDs.Count - 1]);
                triangles.Add(newCircleIDs[0]);
                
                
                LastCircleIDs.Clear();
                LastCircleIDs = newCircleIDs;
                LastCircleList = newCirclePointsList;
            }

            //fill the hole at the top

            //Vector3 newCenterPoint2 = CalculateRightAnglePoint2D(theta, wobble - (increment * (i)), unit, ConeSize);



        }



        private Vector3 CalculateHypothenusPoint2D(float theta, float wobble, AngleUnit unit, float lineSize)
        
        {
                    float Angle, HypothenusDistance, ConeHeight;
                    Angle = wobble / 2;
                    HypothenusDistance = lineSize;
                    Vector3 ConeHypothenusPoint = Vector3.zero;
                    float x, y, z;
                    switch (unit)
                    {
                        case AngleUnit.DEGREES:
                            x = (HypothenusDistance * Mathf.Cos(Mathf.Deg2Rad * (theta + Angle)));
                            y = (HypothenusDistance * Mathf.Sin(Mathf.Deg2Rad * (theta + Angle)));
                            z = 0f;
                            ConeHypothenusPoint = new Vector3(x, y, z);

                        break;
                        case AngleUnit.RADIANS:
                            x = (HypothenusDistance * Mathf.Cos(theta + Angle));
                            y = (HypothenusDistance * Mathf.Sin(theta + Angle));
                            z = 0f;
                            ConeHypothenusPoint = new Vector3(x, y, z);


                        break;

                    }
                    return ConeHypothenusPoint;
        }

        private Vector3 CalculateRightAnglePoint2D(float theta, float wobble, AngleUnit unit, float lineSize)
        {
            float Angle, HypothenusDistance, ConeHeight;
            Angle = wobble / 2;
            HypothenusDistance = lineSize;
            Vector3 ConeCenterPoint = Vector3.zero;
            float x, y, z;

            switch (unit)
            {
                case AngleUnit.DEGREES:
                    ConeHeight = Mathf.Cos(Mathf.Deg2Rad * Angle) * HypothenusDistance;
                    x = (ConeHeight * Mathf.Cos(Mathf.Deg2Rad * (theta)));
                    y = (ConeHeight * Mathf.Sin(Mathf.Deg2Rad * (theta)));
                    z = 0f;
                    ConeCenterPoint = new Vector3(x, y, z);
                    break;
                case AngleUnit.RADIANS:
                    ConeHeight = Mathf.Cos(Angle) * HypothenusDistance;

                    x = (ConeHeight * Mathf.Cos((theta)));
                    y = (ConeHeight * Mathf.Sin((theta)));
                    z = 0f;
                    ConeCenterPoint = new Vector3(x, y, z);
                    break;
            }
            return ConeCenterPoint;
        }

        /**
        private void CalculateOrientationAngle2D(List<float> xvalues, List<float> yvalues, List<float> zvalues,
                                         List<float> coloruv, float theta, AngleUnit unit, float lineSize)
        {

            switch (unit)
            {
                case AngleUnit.DEGREES:
                    xvalues.Add(lineSize * Mathf.Cos(Mathf.Deg2Rad * theta));
                    yvalues.Add(lineSize * Mathf.Sin(Mathf.Deg2Rad * theta));
                    zvalues.Add(0f);
                    coloruv.Add(theta / 180f);
                    break;
                case AngleUnit.RADIANS:
                    xvalues.Add(lineSize * Mathf.Cos(theta));
                    yvalues.Add(lineSize * Mathf.Sin(theta));
                    zvalues.Add(0f);
                    coloruv.Add(theta / 6.28319f);
                    break;

            }
        }
        **/
    }
        }
