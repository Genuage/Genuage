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


using UnityEngine;
using System.Linq;
using Data;
using Valve.VR.InteractionSystem;

public class SendMatrixController : MonoBehaviour
{
    public Transform box;
    public Transform cloud;
    public Transform square;
    public bool FixedPlane1Active = false;
    public bool FixedPlane2Active = false;

    public bool FixedPlane3Active = false;

    public bool FixedPlane4Active = false;

    public bool FixedPlane5Active = false;
    public bool FixedPlane6Active = false;
    public bool FixedPlane7Active = false;
    public bool FixedPlane8Active = false;
    public bool FixedPlane9Active = false;
    public bool FixedPlane10Active = false;

    int planeNumbers = 0;


    GameObject[] goArray = new GameObject[10];

    private bool ControllerClippingPlaneOn = false;
    
    Vector3 positiondifference1;
    Vector3 FixedDirection1;
    Quaternion FixedBoxRotation1;

    void Awake()
    {
        CloudSelector.instance.OnSelectionChange += ReloadGoArray;

    }
    //public Transform plane;
    // Update is called once per frame
    void Update()
    {
        if (ControllerClippingPlaneOn)
        {
            //CloudData data = CloudUpdater.instance.loadCurrentStatus();
            if (box != null)
            {
                Shader.SetGlobalMatrix("_BoxWorldToLocalMatrix", box.worldToLocalMatrix);
                Shader.SetGlobalVector("_BoxLocalScale", box.localScale);
            }
            //Debug.Log(transform.parent.position);
            Shader.SetGlobalVector("_ControllerPlaneNormal", square.forward);
            Shader.SetGlobalVector("_ControllerWorldPosition", square.position);
            Shader.SetGlobalMatrix("_ControllerWorldToLocalMatrix", square.worldToLocalMatrix);
        }   

        //TEST WITH WORLD POSITIONS
        //TODO : Find a way to abstract the strings and vectors that need to be sent



    }

    public void ReloadGoArray(int id)
    {
        CloudData data = CloudUpdater.instance.LoadCurrentStatus();
        planeNumbers = 0;
        if(data.globalMetaData.ClippingPlanesList != null)
        {
            goArray = data.globalMetaData.ClippingPlanesList;

            for (int i = 0; i < goArray.Length; i++)
            {
                if (goArray.ElementAtOrDefault(i) != null)
                {
                    planeNumbers = i + 1;
                    goArray[i].GetComponent<SendClippingPlaneInfo>().enableClippingPlane();

                }
            }
            Debug.Log("planeNumber : "+ planeNumbers);
        }
        else
        {
            goArray = new GameObject[10];
        }


    }

    public void RemoveAllClippingPlanes()
    {
        foreach ( GameObject go in goArray)
        {
            Destroy(go);
        }
        planeNumbers = 0;

    }

    public void ActivateClippingPlane()
    {
        ControllerClippingPlaneOn = true;
    }

    public void DeactivateClippingPlane()
    {
        ControllerClippingPlaneOn = true;
    }

    public void CreateFixedClippingPlane()
    {
        planeNumbers++;
        if(planeNumbers > 10)
        {
            planeNumbers = 1;
        }
        if (box != null)
        {
            Shader.SetGlobalMatrix("_BoxWorldToLocalMatrix", box.worldToLocalMatrix);
            Shader.SetGlobalVector("_BoxLocalScale", box.localScale);
        }
        Vector3 SquarePointspacePosition =  square.position;

        if (goArray.ElementAtOrDefault(planeNumbers-1) != null)
        {
            Destroy(goArray[planeNumbers - 1]);
        }

        GameObject go = new GameObject("Clipping plane proxy");
        go.transform.position = square.position;
        go.transform.rotation = square.rotation;
        go.AddComponent<SendClippingPlaneInfo>();

        go.transform.SetParent(cloud, true);

        switch (planeNumbers)
        {
            case 1:
                //IDEA : USE GAMEOBJECT AS PROXY, SIMILAR TO VRO Anchors
                go.GetComponent<SendClippingPlaneInfo>().Keyword1 = "_ControllerFixedLocalPosition1";
                go.GetComponent<SendClippingPlaneInfo>().Keyword2 = "_ControllerFixedPlaneNormal1";
                go.GetComponent<SendClippingPlaneInfo>().Keyword3 = "FIXED_CLIPPING_PLANE_1";
                go.GetComponent<SendClippingPlaneInfo>().jobON = true;
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_1");

                FixedPlane1Active = true;
                break;
            case 2:
                go.GetComponent<SendClippingPlaneInfo>().Keyword1 = "_ControllerFixedLocalPosition2";
                go.GetComponent<SendClippingPlaneInfo>().Keyword2 = "_ControllerFixedPlaneNormal2";
                go.GetComponent<SendClippingPlaneInfo>().Keyword3 = "FIXED_CLIPPING_PLANE_2";
                go.GetComponent<SendClippingPlaneInfo>().jobON = true;
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_2");

                FixedPlane2Active = true;
                break;
            case 3:
                go.GetComponent<SendClippingPlaneInfo>().Keyword1 = "_ControllerFixedLocalPosition3";
                go.GetComponent<SendClippingPlaneInfo>().Keyword2 = "_ControllerFixedPlaneNormal3";
                go.GetComponent<SendClippingPlaneInfo>().Keyword3 = "FIXED_CLIPPING_PLANE_3";
                go.GetComponent<SendClippingPlaneInfo>().jobON = true;
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_3");
                FixedPlane3Active = true;
                break;
            case 4:
                go.GetComponent<SendClippingPlaneInfo>().Keyword1 = "_ControllerFixedLocalPosition4";
                go.GetComponent<SendClippingPlaneInfo>().Keyword2 = "_ControllerFixedPlaneNormal4";
                go.GetComponent<SendClippingPlaneInfo>().Keyword3 = "FIXED_CLIPPING_PLANE_4";
                go.GetComponent<SendClippingPlaneInfo>().jobON = true;
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_4");

                FixedPlane4Active = true;
                break;
            case 5:
                go.GetComponent<SendClippingPlaneInfo>().Keyword1 = "_ControllerFixedLocalPosition5";
                go.GetComponent<SendClippingPlaneInfo>().Keyword2 = "_ControllerFixedPlaneNormal5";
                go.GetComponent<SendClippingPlaneInfo>().Keyword3 = "FIXED_CLIPPING_PLANE_5";
                go.GetComponent<SendClippingPlaneInfo>().jobON = true;
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_5");
                FixedPlane1Active = true;
                break;
            case 6:
                go.GetComponent<SendClippingPlaneInfo>().Keyword1 = "_ControllerFixedLocalPosition6";
                go.GetComponent<SendClippingPlaneInfo>().Keyword2 = "_ControllerFixedPlaneNormal6";
                go.GetComponent<SendClippingPlaneInfo>().Keyword3 = "FIXED_CLIPPING_PLANE_6";
                go.GetComponent<SendClippingPlaneInfo>().jobON = true;
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_6");

                FixedPlane1Active = true;
                break;
            case 7:
                go.GetComponent<SendClippingPlaneInfo>().Keyword1 = "_ControllerFixedLocalPosition7";
                go.GetComponent<SendClippingPlaneInfo>().Keyword2 = "_ControllerFixedPlaneNormal7";
                go.GetComponent<SendClippingPlaneInfo>().Keyword3 = "FIXED_CLIPPING_PLANE_7";
                go.GetComponent<SendClippingPlaneInfo>().jobON = true;
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_7");
                FixedPlane1Active = true;
                break;
            case 8:
                go.GetComponent<SendClippingPlaneInfo>().Keyword1 = "_ControllerFixedLocalPosition8";
                go.GetComponent<SendClippingPlaneInfo>().Keyword2 = "_ControllerFixedPlaneNormal8";
                go.GetComponent<SendClippingPlaneInfo>().Keyword3 = "FIXED_CLIPPING_PLANE_8";
                go.GetComponent<SendClippingPlaneInfo>().jobON = true;
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_8");
                FixedPlane1Active = true;
                break;
            case 9:
                go.GetComponent<SendClippingPlaneInfo>().Keyword1 = "_ControllerFixedLocalPosition9";
                go.GetComponent<SendClippingPlaneInfo>().Keyword2 = "_ControllerFixedPlaneNormal9";
                go.GetComponent<SendClippingPlaneInfo>().Keyword3 = "FIXED_CLIPPING_PLANE_9";
                go.GetComponent<SendClippingPlaneInfo>().jobON = true;
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_9");

                FixedPlane1Active = true;
                break;
            case 10:
                go.GetComponent<SendClippingPlaneInfo>().Keyword1 = "_ControllerFixedLocalPosition10";
                go.GetComponent<SendClippingPlaneInfo>().Keyword2 = "_ControllerFixedPlaneNormal10";
                go.GetComponent<SendClippingPlaneInfo>().Keyword3 = "FIXED_CLIPPING_PLANE_10";
                go.GetComponent<SendClippingPlaneInfo>().jobON = true;
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_10");
                FixedPlane1Active = true;
                break;
        }

        goArray[planeNumbers - 1] = go;
        CloudUpdater.instance.UpdateFreezableClippingPlanes(goArray);
        /**
        Shader.SetGlobalVector("_ControllerFixedLocalPosition1", cloud.worldToLocalMatrix.MultiplyPoint3x4(SquarePointspacePosition));
        Shader.SetGlobalVector("_ControllerFixedPlaneNormal1", cloud.worldToLocalMatrix.MultiplyPoint3x4(square.forward).normalized);
        Debug.Log("position : " + cloud.worldToLocalMatrix.MultiplyPoint3x4(SquarePointspacePosition));
        Debug.Log("direction : " + cloud.worldToLocalMatrix.MultiplyPoint3x4(square.forward).normalized);
        Shader.EnableKeyword("FIXED_CLIPPING_PLANE_1");
        FixedPlane1Active = true;
        **/

    }
}
