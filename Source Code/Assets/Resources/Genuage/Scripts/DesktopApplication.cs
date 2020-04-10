/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson, Bassam Hajj and Clement caporal
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
using UnityEngine.EventSystems;
using Display;
using Data;

public class DesktopApplication : MonoBehaviour
{

    public static DesktopApplication instance = null;

    public GameObject CameraManagerPrefab;
    public GameObject CanvasPrefab;
    public GameObject CloudLoaderPrefab;
    public GameObject MainCameraPrefab;
    public GameObject CloudStoragePrefab;
    public GameObject CloudSelectorPrefab;
    public GameObject CloudUpdaterPrefab;
    public GameObject CloudSaverPrefab;
    public GameObject ColorMapManagerPrefab;
    public GameObject VRUIManagerPrefab;
    public GameObject EventSystemPrefab;
    public GameObject ModalWindowManagerPrefab;
    public GameObject VRTKPrefab;
    public GameObject GroundPrefab;

    public GameObject CameraManager;
    public GameObject Canvas;
    public GameObject CloudLoaderGO;
    public GameObject MainCamera;
    public GameObject CloudStorageGO;
    public GameObject CloudSelectorGO;
    public GameObject CloudUpdaterGO;
    public GameObject CloudSaverGO;
    public GameObject ColorMapManagerGO;
    public GameObject VRUIManagerGO;
    public GameObject EventSystem;
    public GameObject ModalWindowManager;
    public GameObject VRTK;
    public GameObject Ground;


    public bool VR_Enabled = false;



    public Vector3 cameraInitialPosition;
    public Quaternion cameraInitialRotation;
    public float timesinceClick;
    public float catchtime = 0.25f;

    public Font defaultFont;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InstantiateApplication();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InstantiateApplication()
    {
        GameObject target = new GameObject("target");
        MainCamera = Instantiate(MainCameraPrefab) as GameObject;
        cameraInitialPosition = MainCamera.transform.position;
        cameraInitialRotation = MainCamera.transform.rotation;
        VRTK = Instantiate(VRTKPrefab) as GameObject;
        MainCamera.GetComponent<DragMouseOrbit>().target = target.transform;
        CameraManager = Instantiate(CameraManagerPrefab) as GameObject;
        CameraManager.GetComponent<CameraManager>().desktop_camera = MainCamera.GetComponent<Camera>();
        CameraManager.GetComponent<CameraManager>().vr_camera = VRTK.GetComponent<VRCameraRef>().VRCamera;
        CloudLoaderGO = Instantiate(CloudLoaderPrefab) as GameObject;
        CloudStorageGO = Instantiate(CloudStoragePrefab) as GameObject;
        CloudSelectorGO = Instantiate(CloudSelectorPrefab) as GameObject;
        CloudUpdaterGO = Instantiate(CloudUpdaterPrefab) as GameObject;
        CloudSaverGO = Instantiate(CloudSaverPrefab) as GameObject;
        ColorMapManagerGO = Instantiate(ColorMapManagerPrefab) as GameObject;
        VRUIManagerGO = Instantiate(VRUIManagerPrefab) as GameObject;
        EventSystem = Instantiate(EventSystemPrefab) as GameObject;
        ModalWindowManager = Instantiate(ModalWindowManagerPrefab) as GameObject;
        Ground = Instantiate(GroundPrefab) as GameObject;
        Canvas = Instantiate(CanvasPrefab) as GameObject;

        //OnAllCloudsDeleted();
        //CloudSelector.instance.OnFirstSelection += OnFirstCloudLoad;
        //CloudSelector.instance.OnNoSelection += OnAllCloudsDeleted;
    }
    /**
    private void OnFirstCloudLoad()
    {
        Canvas.transform.GetChild(1).gameObject.SetActive(true);
        Canvas.transform.GetChild(2).gameObject.SetActive(true);
        Canvas.transform.GetChild(3).gameObject.SetActive(true);

    }

    private void OnAllCloudsDeleted()
    {
        Canvas.transform.GetChild(1).gameObject.SetActive(false);
        Canvas.transform.GetChild(2).gameObject.SetActive(false);
        Canvas.transform.GetChild(3).gameObject.SetActive(false);
    }
    **/
}
