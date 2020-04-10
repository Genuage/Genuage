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
using VRTK;
using Data;

public abstract class CloudChild : MonoBehaviour
{
    public bool isChild = false;
    public GameObject container;
    public int id;

    public CloudData clouddata;

    private void Start()
    {
        isChild = false;
    }

    private void OnTriggerEnter(Collider cloud_box)
    {
        if (!isChild)
        {


            if (cloud_box.tag == "PointCloud")
            {
                isChild = true;

                container = new GameObject("container");
                container.transform.position = this.transform.position;
                
                container.transform.SetParent(cloud_box.transform, true);
                //container.transform.rotation = Quaternion.identity;
                container.transform.localRotation = Quaternion.identity;
                container.transform.localScale = Vector3.one;
                //transform.SetParent(container.transform, true);
                
                if (!GetComponent<VRTK_TransformFollow>())
                {
                    gameObject.AddComponent<VRTK_TransformFollow>();
                }

                GetComponent<VRTK_TransformFollow>().followsScale = false;

                GetComponent<VRTK_TransformFollow>().gameObjectToChange = this.gameObject;
                GetComponent<VRTK_TransformFollow>().gameObjectToFollow = container;
                GetComponent<VRTK_TransformFollow>().moment = VRTK_TransformFollow.FollowMoment.OnLateUpdate;

                CloudData cloud_data = cloud_box.transform.parent.GetComponentInChildren<CloudData>();
                clouddata = cloud_data;
                PutInMemory(cloud_data);

            }
        }
    }

    public abstract void PutInMemory(CloudData cloud_data);
}
