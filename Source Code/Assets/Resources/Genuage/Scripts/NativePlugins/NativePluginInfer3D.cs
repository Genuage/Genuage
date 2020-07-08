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


using Data;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public unsafe class NativePluginInfer3D : NativePlugin
{
    /**
    public enum FunctionID
    {
        DFXFYFZPosterior = 0,
        D_LIKELYHOOD_NO_NOISE = 1,
        DPOSTERIOR_NO_NOISE_NON_INFORMATIVE_PRIOR = 2,
        D_POSTERIOR_NO_NOISE_CONGUGATEPRIOR = 3,
        DLIKELYHOOD_UNIFORM_NOISE = 4,
        DPOSTERIOR_UNIFORM_NOISE_NON_INFORMATIVE_PRIOR = 5,
        DLIKELYHOOD_ASYMETRIC_NOISE = 6,
        DPOSTERIOR_ASYMETRIC_NOISE_NON_INFORMATIVE = 7,
        DLIKELYHOOD_APPROX_NOISE_ONLY_Z = 8,
        DPOSTERIOR_APPROX_NOISE_ONLY_Z_NON_INFORMATIVE = 9,
        DVLIKELYHOOD_NO_NOISE = 10,
        DVPOSTERIOR_NO_NOISE_NON_INFORMATIVE = 11,
        DVPOSTERIOR_NO_NOISE_CONJUGATE_PRIOR = 12,
        DVLIKELYHOOD_UNIFORM_NOISE = 13,
        DVPOSTERIOR_UNIFORM_NOISE_NON_INFORMATIVE = 14,
        DVLIKELYHOOD_ASYMETRIC_NOISE = 15,
        DVPOSTERIOR_ASYMETRIC_NOISE_NON_INFORMATIVE = 16,
        DVLIKELYHOOD_APPROX_NOISE_ONLY_Z = 17,
        DVPOSTERIOR_APPROX_NOISE_ONLY_Z_NON_INfORMATIVE = 18,

    };
        **/

    private enum HeaderID
    {
        Diffusion = 0,
        Diffusion_Velocity = 1,
    };

    private enum ParamID
    {
        LIKELYHOOD_NO_NOISE = 0,
        LIKELYHOOD_UNIFORM_NOISE = 1,
        LIKELYHOOD_ASYMETRIC_NOISE = 2,
        LIKELYHOOD_APPROX_NOISE_ONLY_Z = 3,
        POSTERIOR_NO_NOISE_NON_INFORMATIVE_PRIOR = 4,
        POSTERIOR_NO_NOISE_CONGUGATEPRIOR = 5,
        POSTERIOR_UNIFORM_NOISE_NON_INFORMATIVE_PRIOR = 6,
        POSTERIOR_ASYMETRIC_NOISE_NON_INFORMATIVE = 7,
        POSTERIOR_APPROX_NOISE_ONLY_Z_NON_INFORMATIVE = 8,
    };

    public Dropdown headerDropdown;
    public Dropdown paramDropdown;
    public InputField SigmaInput;
    public InputField SigmaxyInput;
    public InputField SigmazInput;

    HeaderID headerID;
    ParamID paramID;

    public double Sigma = 0.0;
    public double SigmaXY = 0.0;
    public double SigmaZ = 0.0;


    public GameObject resultwindow;

    public Vector3 ArrowLocalPosition;

    int N;
    double* trajectories;
    double* xCoord;
    double* yCoord;
    double* zCoord;
    double* tCoord;

    double* Diffusion;
    double* ForceX;
    double* ForceY;
    double* ForceZ;

    [DllImport("Infer3DPlugin")]
    private static extern void Infer3D(int headerID, int paramID, double sigma, double sigmaxy, double sigmaz, int NumberOfPoints, void* TrajectoryNumber, void* xCoordinates, void* yCoordinates, void* zCoordinates, void* TimeStamp, double* Diffusion, double* ForceX, double* ForceY, double* ForceZ);

    private void Awake()
    {
        //ExecutePlugin();
    }

    protected override void LaunchPluginFunction(CloudData data)
    {
        HashSet<int> selectedpointsSet = data.globalMetaData.SelectedPointsList;
        HashSet<float> selectedtrajectorySet = data.globalMetaData.SelectedTRajectories;
        N = 0;

        List<double> TrajectoriesList = new List<double>();
        List<double> Xvalues = new List<double>();
        List<double> Yvalues = new List<double>();
        List<double> Zvalues = new List<double>();
        List<double> Tvalues = new List<double>();

        Debug.Log("lowtime " + data.globalMetaData.lowertimeLimit);

        Debug.Log("uptime " + data.globalMetaData.uppertimeLimit);

        float xaverage, yaverage, zaverage;
        float xsum = 0;
        float ysum = 0;
        float zsum = 0;
        int pointnumber = 0;
        foreach (float i in selectedtrajectorySet)
        {
            foreach (int j in data.pointTrajectoriesTable[i].metadata.selectedpointsIDList)
            {
                if (data.pointMetaDataTable[j].isHidden == false)
                {
                    Debug.Log("time " + data.pointDataTable[j].time);

                    if (data.pointDataTable[j].time >= data.globalMetaData.lowertimeLimit && data.pointDataTable[j].time <= data.globalMetaData.uppertimeLimit)
                    {
                        Debug.Log("check");
                        N++;
                        TrajectoriesList.Add(i);
                        Xvalues.Add((double)data.pointDataTable[j].position.x);
                        Yvalues.Add((double)data.pointDataTable[j].position.y);
                        Zvalues.Add((double)data.pointDataTable[j].position.z);
                        Tvalues.Add((double)data.pointDataTable[j].time);

                        xsum += data.pointDataTable[j].normed_position.x;
                        ysum += data.pointDataTable[j].normed_position.y;
                        zsum += data.pointDataTable[j].normed_position.z;
                        pointnumber++;
                    }
                }
            }
        }

        xaverage = xsum / pointnumber;
        yaverage = ysum / pointnumber;
        zaverage = zsum / pointnumber;

        ArrowLocalPosition = new Vector3(xaverage, yaverage, zaverage);

        //DEBUG
        /**
        N = 25;
        for(int i = 0; i < 5; i++)
        {
            TrajectoriesList.Add(i);
            Xvalues.Add((double)Random.Range(0,100));
            Yvalues.Add((double)Random.Range(0, 100));
            Zvalues.Add((double)Random.Range(0, 100));
            Tvalues.Add(0);
            TrajectoriesList.Add(i);
            Xvalues.Add((double)Random.Range(0, 100));
            Yvalues.Add((double)Random.Range(0, 100));
            Zvalues.Add((double)Random.Range(0, 100));
            Tvalues.Add(1);
            TrajectoriesList.Add(i);
            Xvalues.Add((double)Random.Range(0, 100));
            Yvalues.Add((double)Random.Range(0, 100));
            Zvalues.Add((double)Random.Range(0, 100));
            Tvalues.Add(2);
            TrajectoriesList.Add(i);
            Xvalues.Add((double)Random.Range(0, 100));
            Yvalues.Add((double)Random.Range(0, 100));
            Zvalues.Add((double)Random.Range(0, 100));
            Tvalues.Add(3);
            TrajectoriesList.Add(i);
            Xvalues.Add((double)Random.Range(0, 100));
            Yvalues.Add((double)Random.Range(0, 100));
            Zvalues.Add((double)Random.Range(0, 100));
            Tvalues.Add(4);
        }
    
        Debug.Log(TrajectoriesList.Count);
        **/
        //END DEBUG
        double[] TrajectoriesArray = TrajectoriesList.ToArray();
        double[] XvaluesArray = Xvalues.ToArray();
        double[] YvaluesArray = Yvalues.ToArray();
        double[] ZvaluesArray = Zvalues.ToArray();
        double[] TvaluesArray = Tvalues.ToArray();

        fixed (double* Trajectoriespointer = TrajectoriesArray)
        {
            fixed (double* xvalues = XvaluesArray)
            {
                fixed (double* yvalues = YvaluesArray)
                {
                    fixed (double* zvalues = ZvaluesArray)
                    {
                        fixed (double* tvalues = TvaluesArray)
                        {
                            this.trajectories = Trajectoriespointer;
                            this.xCoord = xvalues;
                            this.yCoord = yvalues;
                            this.zCoord = zvalues;
                            this.tCoord = tvalues;

                            double diffusion = 145.0;
                            double forceX = 14.0;
                            double forceY = 16.0;
                            double forceZ = 17.0;

                            Diffusion = &diffusion;
                            ForceX = &forceX;
                            ForceY = &forceY;
                            ForceZ = &forceZ;

                            //functionID = (FunctionID)dropdown.value;

                            headerID = (HeaderID)headerDropdown.value;
                            paramID = (ParamID)paramDropdown.value;
                            // Debug.Log("FunctionID - "+(int)functionID);
                            double.TryParse(SigmaInput.text, out Sigma);
                            double.TryParse(SigmaxyInput.text, out SigmaXY);
                            double.TryParse(SigmazInput.text, out SigmaZ);

                            //Debug.Log("fid " + functionID);
                            Debug.Log("Sigma " + Sigma);
                            Debug.Log("Sigmaxy " + SigmaXY);
                            Debug.Log("Sigmaz " + SigmaZ);


                            Infer3D((int)headerID, (int)paramID, Sigma, SigmaXY, SigmaZ, N, trajectories, xCoord, yCoord, zCoord, tCoord, Diffusion, ForceX, ForceY, ForceZ);
                            double diffusionres = *Diffusion;
                            double forceXres = *ForceX;
                            double forceYres = *ForceY;
                            double forceZres = *ForceZ;
                            diffusionres = Math.Round(diffusionres, 3);
                            forceXres = Math.Round(forceXres, 3);
                            forceYres = Math.Round(forceYres, 3);
                            forceZres = Math.Round(forceZres, 3);

                            ResultsString = "diffusion : " + diffusionres + "\n" + "forceX : " + forceXres + "\n"
                                            + "forceY : " + forceYres + "\n" + "forceZ : " + forceZres + "\n";
                            Debug.Log("N : " + N);
                            Debug.Log("diffusion : " + *Diffusion);
                            Debug.Log("forceX : " + *ForceX);
                            Debug.Log("forceY : " + *ForceY);
                            Debug.Log("forceZ : " + *ForceZ);

                            GameObject go = new GameObject("ARROW");
                            go.transform.position = Vector3.zero;
                            go.transform.SetParent(data.transform, true);

                            //go.transform.SetParent(data.transform);
                            go.AddComponent<GenerateArrow>();
                            go.GetComponent<GenerateArrow>().GenerateTriangularArrowMesh(new Vector3((float)*ForceX, (float)*ForceY, (float)*ForceZ).normalized);
                            go.transform.localPosition = ArrowLocalPosition;
                            Debug.Log("ArrowCalculatedpos : " + ArrowLocalPosition);
                            Debug.Log("ArrowWorldPos : " + go.transform.position);
                            Debug.Log("ArrowLocalPos : " + go.transform.localPosition);


                        }
                    }
                }
            }
        }
    }


    protected override void DisplayResults()
    {
        resultwindow.SetActive(true);
        resultwindow.GetComponent<ModalWindow>().text.text = ResultsString;
    }

}
