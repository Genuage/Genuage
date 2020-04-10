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
using System.Runtime.InteropServices;
using UnityEngine;

public unsafe class TESTNativePLuging : MonoBehaviour
{
    [DllImport("Infer3DPlugin")]
    private static extern void Infer3D(int NumberOfPoints, void* TrajectoryNumber, void* xCoordinates, void* yCoordinates, void* zCoordinates, void* TimeStamp, double* Diffusion, double* ForceX, double* ForceY, double* ForceZ);
	
    void Awake()
    {
        // Calls the FooPluginFunction inside the plugin
        // And prints 5 to the console
        //print(FooPluginFunction());
        int n = 6;
        int N = n;
        double* trajectories = stackalloc double[6] { 0.0, 0, 0, 1, 1, 1 };
        double* xCoord = stackalloc double[6] { 0.1,0.5,0.9,0.2,0.3,0.4};
        double* yCoord = stackalloc double[6] { 0.6, 0.7, 0.9, 0.5, 0.3, 0.2 };
        double* zCoord = stackalloc double[6] { 0.45, 0.62, 0.2, 0.01, 0.75, 0.35 };
        double* tCoord = stackalloc double[6] { 0.1, 0.2, 0.3, 0.1, 0.2, 0.3 };

        double diffusion = 0.0;
        double forceX = 0.0;
        double forceY = 0.0;
        double forceZ = 0.0;

        double* Diffusion = &diffusion;
        double* ForceX = &forceX;
        double* ForceY = &forceY;
        double* ForceZ = &forceZ;


        Infer3D(N, trajectories, xCoord, yCoord, zCoord, tCoord, Diffusion, ForceX, ForceY, ForceZ);
        Debug.Log(N);
        
        Debug.Log("diffusion : " + *Diffusion);
        Debug.Log("forceX : " + *ForceX);
        Debug.Log("forceY : " + *ForceY);
        Debug.Log("forceZ : " + *ForceZ);
        

    }
}
