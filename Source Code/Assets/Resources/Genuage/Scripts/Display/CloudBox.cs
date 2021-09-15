/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson, Bassam Hajj and Clement Caporal
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. All advertising materials mentioning features or use of this software
   must display the following acknowledgement:
   This product includes software developed by the Institut Curie, Insitut Pasteur and CNRS.
4. Neither the name of the Institut Curie, Insitut Pasteur and CNRS nor the
   names of its contributors may be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
**/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;
using Data;
using DesktopInterface;
using Display;
/// <summary>
/// Box use to show to the user the collider box around the CloudPoints
/// </summary>
public class CloudBox : MonoBehaviour
{

    CloudData _cloud_status;
    GameObject _box;
    Material _material;
    bool _is_grabbed;

    List<GameObject> TextList;

    public void Activate () {
        TextList = new List<GameObject>();
        _cloud_status = GetComponent<CloudData>();
        _box = new GameObject("Box");
        _box.AddComponent<DragMouse>();
        _box.GetComponent<DragMouse>().enabled = !DesktopApplication.instance.VR_Enabled;
        _box.AddComponent<MeshFilter>();
        _material = new Material(Shader.Find("Unlit/Color"));
        _material.SetColor("_Color", Color.white);
        _box.AddComponent<MeshRenderer>().material = _material;
        _box.transform.SetParent(transform.parent,false);
        CreateBox();
        AddFollowComponents(this.gameObject, _box);
        AddDraggableComponents(_box);
        transform.parent.GetComponent<CloudObjectRefference>().box = _box;
    }

    private void AddDraggableComponents(GameObject go)
    {
        go.AddComponent<Rigidbody>().useGravity = false;
        go.GetComponent<Rigidbody>().isKinematic = true;
        go.AddComponent<BoxCollider>();
        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<BoxCollider>().size = Vector3.one;
        go.AddComponent<VRTK_InteractableObject>().isGrabbable = true;
        go.AddComponent<VRTK_ChildOfControllerGrabAttach>().precisionGrab = true;
        go.GetComponent<VRTK_InteractableObject>().grabAttachMechanicScript = go.GetComponent<VRTK_ChildOfControllerGrabAttach>();
        go.GetComponent<VRTK_InteractableObject>().InteractableObjectTouched += Hovered;
        go.GetComponent<VRTK_InteractableObject>().InteractableObjectUntouched += Unhovered;
        go.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += Grabbed;
        go.GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += Ungrabbed;
    }

    private void AddFollowComponents(GameObject go, GameObject object_to_follow)
    {
        go.AddComponent<VRTK_RigidbodyFollow>();
        go.GetComponent<VRTK_RigidbodyFollow>().followsScale = false;
        go.GetComponent<VRTK_RigidbodyFollow>().gameObjectToFollow = object_to_follow;
    }

    public void CreateBox()
    {
        Mesh mesh = _box.GetComponent<MeshFilter>().mesh;
        
        List<Vector3> vertices = new List<Vector3>(){
            new Vector3(- 0.5f, - 0.5f, - 0.5f),
            new Vector3(0.5f, - 0.5f, - 0.5f),
            new Vector3(0.5f, 0.5f, - 0.5f),
            new Vector3(- 0.5f, 0.5f, - 0.5f),
            new Vector3(- 0.5f, 0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,   0.5f),
            new Vector3( 0.5f, - 0.5f,   0.5f),
            new Vector3(- 0.5f, - 0.5f,   0.5f),
        };
        List<int> _lines = new List<int>(){
            0, 1,
            0, 3,
            0, 7,
            6, 1,
            6, 7,
            1, 2,
            4, 7,
            6, 5,
            5, 4,
            5, 2,
            3, 2,
            3, 4
        };

        _box.transform.localScale = Vector3.Scale(_cloud_status.globalMetaData.box_scale, _cloud_status.globalMetaData.scale);
        foreach(GameObject go in TextList)
        {
            Destroy(go);
        }
        TextList.Clear();

        //Add if in Options
        if (ApplicationOptions.instance.GetGraduationActivated() == true)
        {
            GenerateScaleBars(vertices, _lines);
        }
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.SetIndices(_lines.ToArray(), MeshTopology.Lines, 0);


        _box.tag = "PointCloud";
    }

    private void GenerateScaleBars(List<Vector3> verts, List<int> lines)
    {
        //int graduationnumber = ApplicationOptions.instance.GetDefaultBoxScaleNumber();
        /**
        int graduationnumberX = _cloud_status.globalMetaData.ScaleBarNumberX;
        int graduationnumberY = _cloud_status.globalMetaData.ScaleBarNumberY;
        int graduationnumberZ = _cloud_status.globalMetaData.ScaleBarNumberZ;
        **/
        float GraduationDistanceX = _cloud_status.globalMetaData.ScaleBarDistanceX;
        float GraduationDistanceY = _cloud_status.globalMetaData.ScaleBarDistanceY;
        float GraduationDistanceZ = _cloud_status.globalMetaData.ScaleBarDistanceZ;
        float Xrange = (_cloud_status.globalMetaData.xMax - _cloud_status.globalMetaData.xMin);
        float Yrange = (_cloud_status.globalMetaData.yMax - _cloud_status.globalMetaData.yMin);
        float Zrange = (_cloud_status.globalMetaData.zMax - _cloud_status.globalMetaData.zMin);
        float graduationnumberX = Xrange / GraduationDistanceX;
        int graduationnumberY = 0;
        int graduationnumberZ = 0;

        float graduationlength = ApplicationOptions.instance.GetGraduationLength();
        Vector3 point1 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 point2 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 point3 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 point4 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3[] pointArray = new Vector3[] { point1,point2,point3,point4 }; //,point2,point3,point4};

        foreach(Vector3 v in pointArray)
        {
            float TotalValue = 0;
            //X
            while (TotalValue < Xrange)
            //for (int i = 0; i <= graduationnumberX; i++)
            {
                
                //Debug.Log(_box.transform.localScale);
                Vector3 displacementVector = new Vector3((float)(TotalValue / Xrange), 0f, 0f);
                //Debug.Log(displacementVector);

                Vector3 newpointVector;
                //x rows
                if (v.x < 0)
                {
                    newpointVector = v + displacementVector;
                }
                else
                {
                    newpointVector = v - displacementVector;
                }
                verts.Add(newpointVector);
                lines.Add(verts.Count - 1);
                Vector3 endvector;
                if (v.z < 0)
                {
                    endvector = newpointVector + new Vector3(0f, 0f, graduationlength / _box.transform.localScale.z);
                }
                else
                {
                    endvector = newpointVector - new Vector3(0f, 0f, graduationlength / _box.transform.localScale.z);
                }

                //Vector3 endvector = newpointVector + new Vector3(0f,0f, graduationlength );

                verts.Add(endvector);
                lines.Add(verts.Count - 1);

                //new line
                verts.Add(newpointVector);
                lines.Add(verts.Count - 1);
                if (v.y < 0)
                {
                    endvector = newpointVector + new Vector3(0f, graduationlength / _box.transform.localScale.y, 0f);
                }
                else
                {
                    endvector = newpointVector - new Vector3(0f, graduationlength / _box.transform.localScale.y, 0f);
                }
                verts.Add(endvector);
                lines.Add(verts.Count - 1);


                if (v == new Vector3(-0.5f, -0.5f, -0.5f))
                {
                    //GameObject container = new GameObject();
                    //container.transform.SetParent(_box.transform);
                    GameObject text_object = new GameObject("Text");
                    //text_object.transform.position = endvector;
                    
                    text_object.transform.localScale = Vector3.one * 0.015f;
                    text_object.transform.SetParent(_box.transform, true);
                    text_object.transform.localPosition = new Vector3(newpointVector.x,
                                                                 -0.55f,
                                                                 -0.5f);
                    text_object.AddComponent<MeshRenderer>();
                    text_object.AddComponent<TextMesh>();
                    float mark = TotalValue;
                    float roundedmark = Mathf.Round(mark);
                    text_object.GetComponent<TextMesh>().text = roundedmark.ToString();
                    text_object.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
                    text_object.GetComponent<TextMesh>().color = Color.white;

                    text_object.AddComponent<StaringLabel>();
                    text_object.transform.localScale = new Vector3(0.015f, 0.03f, 0.015f);

                    TextList.Add(text_object);
                }

                TotalValue += GraduationDistanceX;

            }

            TotalValue = 0;
            //y
            while (TotalValue < Yrange)
            //for (int i = 0; i <= graduationnumberX; i++)
            {

                //Debug.Log(_box.transform.localScale);
                Vector3 displacementVector = new Vector3(0f, (float)(TotalValue / Yrange), 0f);

//y rows
                Vector3 newpointVector;

                
                if (v.y < 0)
                {
                    newpointVector = v + displacementVector;
                }
                else
                {
                    newpointVector = v - displacementVector;
                }
                verts.Add(newpointVector);
                lines.Add(verts.Count - 1);

                Vector3 endvector;

                //endvector = newpointVector + new Vector3(graduationlength * _box.transform.localScale.x, 0f, 0f);
                if (v.x < 0)
                {
                    endvector = newpointVector + new Vector3(graduationlength / _box.transform.localScale.x, 0f, 0f);
                }
                else
                {
                    endvector = newpointVector - new Vector3(graduationlength / _box.transform.localScale.x, 0f, 0f);
                }

                if (v == new Vector3(-0.5f, -0.5f, -0.5f))
                {
                    //GameObject container = new GameObject();
                    //container.transform.SetParent(_box.transform);
                    GameObject text_object = new GameObject("Text");
                    //text_object.transform.position = endvector;
                    text_object.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
                    text_object.transform.SetParent(_box.transform, true);
                    text_object.transform.localPosition = new Vector3(-0.55f ,
                                                                 newpointVector.y ,
                                                                 -0.5f);
                    text_object.AddComponent<MeshRenderer>();
                    text_object.AddComponent<TextMesh>();
                    float mark = TotalValue;
                    float roundedmark = Mathf.Round(mark);
                    text_object.GetComponent<TextMesh>().text = roundedmark.ToString();
                    text_object.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
                    text_object.GetComponent<TextMesh>().color = Color.white;

                    text_object.AddComponent<StaringLabel>();
                    text_object.transform.localScale = new Vector3(0.015f, 0.03f, 0.015f);
                    TextList.Add(text_object);
                }

                //endvector = newpointVector + new Vector3(graduationlength , 0f, 0f);
                verts.Add(endvector);
                lines.Add(verts.Count - 1);

                //newline
                verts.Add(newpointVector);
                lines.Add(verts.Count - 1);
                if (v.z < 0)
                {
                    endvector = newpointVector + new Vector3(0f, 0f, graduationlength / _box.transform.localScale.z);
                }
                else
                {
                    endvector = newpointVector - new Vector3(0f, 0f, graduationlength / _box.transform.localScale.z);
                }
                verts.Add(endvector);
                lines.Add(verts.Count - 1);
                TotalValue += GraduationDistanceY;


            }
            TotalValue = 0;
            //y
            while (TotalValue < Zrange)
            //for (int i = 0; i <= graduationnumberX; i++)
            {

                //Debug.Log(_box.transform.localScale);
                Vector3 displacementVector = new Vector3(0f, 0f, (float)(TotalValue / Zrange));

                Vector3 newpointVector;

                if (v.z < 0)
                {
                    newpointVector = v + displacementVector;
                }
                else
                {
                    newpointVector = v - displacementVector;
                }
                verts.Add(newpointVector);
                lines.Add(verts.Count - 1);
                Vector3 endvector;

                if (v.x < 0)
                {
                    endvector = newpointVector + new Vector3(graduationlength / _box.transform.localScale.x, 0f, 0f);
                }
                else
                {
                    endvector = newpointVector - new Vector3(graduationlength / _box.transform.localScale.x, 0f, 0f);
                }

                //if(graduationlength.)
                //endvector = newpointVector + new Vector3(graduationlength * _box.transform.localScale.x, 0f, 0f);
                //endvector = newpointVector + new Vector3(graduationlength , 0f, 0f);
                verts.Add(endvector);
                lines.Add(verts.Count - 1);

                if (v == new Vector3(-0.5f, -0.5f, -0.5f))
                {
                    //GameObject container = new GameObject();
                    //container.transform.SetParent(_box.transform);
                    GameObject text_object = new GameObject("Text");
                    //text_object.transform.position = endvector;
                    text_object.transform.localScale = Vector3.one * 0.015f;
                    text_object.transform.SetParent(_box.transform, true);
                    text_object.transform.localPosition = new Vector3(-0.55f ,
                                             -0.5f ,
                                             newpointVector.z );

                    text_object.AddComponent<MeshRenderer>();
                    text_object.AddComponent<TextMesh>();
                    float mark = TotalValue;
                    float roundedmark = Mathf.Round(mark);
                    text_object.GetComponent<TextMesh>().text = roundedmark.ToString();
                    text_object.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
                    text_object.GetComponent<TextMesh>().color = Color.white;

                    text_object.AddComponent<StaringLabel>();

                    text_object.transform.localScale = new Vector3(0.015f, 0.03f, 0.015f);
                    TextList.Add(text_object);
                }

                //newline
                verts.Add(newpointVector);
                lines.Add(verts.Count - 1);
                if (v.y < 0)
                {
                    endvector = newpointVector + new Vector3(0f, graduationlength / _box.transform.localScale.y, 0f);
                }
                else
                {
                    endvector = newpointVector - new Vector3(0f, graduationlength / _box.transform.localScale.y, 0f);
                }
                verts.Add(endvector);
                lines.Add(verts.Count - 1);
                TotalValue += GraduationDistanceZ;

                //Debug.Log(i);
            }
        }
    }

    public void Hovered(object o, InteractableObjectEventArgs e)
    {
        if (!_is_grabbed)
        {
            _material.color = UIColors._hovered;
        }
    }

    public void Unhovered(object o, InteractableObjectEventArgs e)
    {
        if (!_is_grabbed)
        {
            _material.color = UIColors._movable;
        }
    }

    public void Grabbed(object o, InteractableObjectEventArgs e)
    {
        _is_grabbed = true;
        _material.color = UIColors._clicked;
    }

    public void Ungrabbed(object o, InteractableObjectEventArgs e)
    {
        _is_grabbed = false;
        _material.color = UIColors._hovered;
    }
}
