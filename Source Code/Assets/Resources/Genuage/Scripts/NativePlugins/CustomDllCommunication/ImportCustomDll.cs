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

using Data;
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using IO;
enum DllLoadOptions
{
    ADD_COLUMN = 0,
    CALCULATE_VALUE = 1,
    CREATE_CLOUD = 2
};


public unsafe class ImportCustomDll : MonoBehaviour
{

    struct CloudDataArrays{
        public float[] TrajectoriesList { get; }
        public float[] Xvalues { get; }
        public float[] Yvalues { get; }
        public float[] Zvalues { get; }
        public float[] Cvalues { get; }
        public float[] Tvalues { get; }
        public float[] PHvalues { get; }
        public float[] THvalues { get; }
        public float[] Svalues { get; }
        public int pointnumber { get; }

        public CloudDataArrays(int Pointnumber, float[] trajectoriesList, float[] xvalues, float[] yvalues,
                           float[] zvalues, float[] cvalues, float[] tvalues,
                           float[] phvalues, float[] thvalues, float[] svalues)
        {
            TrajectoriesList = trajectoriesList;
            Xvalues = xvalues;
            Yvalues = yvalues;
            Zvalues = zvalues;
            Cvalues = cvalues;
            Tvalues = tvalues;
            PHvalues = phvalues;
            THvalues = thvalues;
            Svalues = svalues;
            pointnumber = Pointnumber;
        }
    }

    // import necessary API as shown in other examples
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibrary(string lib);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void FreeLibrary(IntPtr module);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetProcAddress(IntPtr module, string proc);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool SetDllDirectory(string lpPathName);
    private IntPtr LoadedDll;


    delegate void AddColumnFunction(int point_number, void* x_coordinates, void* y_coordinates, void* z_coordinates,
                                  void* colors, void* times, void* trajectory_numbers, void* phi_angle,
                                  void* theta_angle, void* point_sizes,
                                  void* int_params, void* float_params, void* double_params,
                                  float* results);

    delegate void CalculateValueFunction(int point_number, void* x_coordinates, void* y_coordinates, void* z_coordinates,
                                  void* colors, void* times, void* trajectory_numbers, void* phi_angle,
                                  void* theta_angle, void* point_sizes,
                                  void* int_params, void* float_params, void* double_params,
                                  float* results);


    delegate void GetArraySizeFunction(int point_number, void* x_coordinates, void* y_coordinates, void* z_coordinates,
                                  void* colors, void* times, void* trajectory_numbers, void* phi_angle,
                                  void* theta_angle, void* point_sizes,
                                  void* int_params, void* float_params, void* double_params, 
                                  int* Result_array_lines, int* Result_array_column); 

    delegate void CreateNewCloudFunction(int point_number, void* x_coordinates, void* y_coordinates, void* z_coordinates,
                                  void* colors, void* times, void* trajectory_numbers, void* phi_angle,
                                  void* theta_angle, void* point_sizes,
                                  void* int_params, void* float_params, void* double_params,
                                  float** results); 

    //UI ELEMENTS
    //TODO : Remove and put in appropriate interface object
    public Dropdown dropdown;
    public InputField intParametersInputField;
    public InputField floatParametersInputField;
    public InputField doubleParametersInputField;

    private bool selectionSent = false;
    private Dictionary<int, int> IDKEYDICT;
    public void Launch()
    {
        IDKEYDICT = new Dictionary<int, int>();
        var extensions = new[]
            {
                new ExtensionFilter("DLL", "dll"),
                new ExtensionFilter("All Files", "*" )
            };
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, true, (string[] paths) => { InitiateLoad(paths); });

    }

    private void InitiateLoad(string[] paths)
    {
        foreach (string path in paths)
        {
            LoadDLL(path);
        }

    }


    private void LoadDLL(string dllpath)
    {
        string directory = Path.GetDirectoryName(dllpath);
        string filename = Path.GetFileName(dllpath);
        Debug.Log(directory);
        Debug.Log(filename);

        SetDllDirectory(directory);
        LoadedDll = LoadLibrary(filename);
        if (LoadedDll == IntPtr.Zero) { Debug.Log("DLL_ID is null"); }
        IntPtr FunctionAdress = GetProcAddress(LoadedDll, "Execute");
        if (FunctionAdress == IntPtr.Zero) { Debug.Log("FUNC_ID is null"); }
        Debug.Log("DLL LOADED");

        //TODO : Replace by Case statement and add create cloud option
        if ((DllLoadOptions)dropdown.value == DllLoadOptions.ADD_COLUMN)
        {
            Debug.Log("Executing Add Column Function");
            AddColumnFunction columnfunction = (AddColumnFunction)Marshal.GetDelegateForFunctionPointer(FunctionAdress, typeof(AddColumnFunction));
            ExecuteColumnFunction(columnfunction);

        }
        else if ((DllLoadOptions)dropdown.value == DllLoadOptions.CALCULATE_VALUE)
        {
            Debug.Log("Executing Calculate Value Function");

            CalculateValueFunction valuefunction = (CalculateValueFunction)Marshal.GetDelegateForFunctionPointer(FunctionAdress, typeof(CalculateValueFunction));
            ExecuteValueFunction(valuefunction);

        }
        else if ((DllLoadOptions)dropdown.value == DllLoadOptions.CREATE_CLOUD)
        {
            Debug.Log("Executing Create New Cloud Function");

            CreateNewCloudFunction cloudFunction = (CreateNewCloudFunction)Marshal.GetDelegateForFunctionPointer(FunctionAdress, typeof(CreateNewCloudFunction));
            ExecuteNewCloudFunction(cloudFunction);

        }
        FunctionAdress = IntPtr.Zero;
        Debug.Log("Freeing Library");
        FreeLibrary(LoadedDll);
        Debug.Log("Library Freed");
    }

    private CloudDataArrays CreateCloudArrays()
    {
        List<float> TrajectoriesList = new List<float>();
        List<float> Xvalues = new List<float>();
        List<float> Yvalues = new List<float>();
        List<float> Zvalues = new List<float>();
        List<float> Cvalues = new List<float>();
        List<float> Tvalues = new List<float>();
        List<float> PHvalues = new List<float>();
        List<float> THvalues = new List<float>();
        List<float> Svalues = new List<float>();

        CloudData data = CloudUpdater.instance.LoadCurrentStatus();
        int pointnumber = 0;

        if (data.globalMetaData.SelectedPointsList.Count == 0)
        {
            selectionSent = false;
            Debug.Log("Sending all points");
            foreach (var i in data.pointDataTable)
            {
                TrajectoriesList.Add(i.Value.trajectory);
                Xvalues.Add(i.Value.position.x);
                Yvalues.Add(i.Value.position.y);
                Zvalues.Add(i.Value.position.z);
                Cvalues.Add(i.Value.color_index);
                Tvalues.Add(i.Value.time);
                PHvalues.Add(i.Value.phi_angle);
                THvalues.Add(i.Value.theta_angle);
                Svalues.Add(i.Value.size);
                pointnumber++;

            }
        }
        else
        {
            selectionSent = true;
            Debug.Log("Sending selected points");

            foreach (var i in data.globalMetaData.SelectedPointsList)
            {
                TrajectoriesList.Add(data.pointDataTable[i].trajectory);
                Xvalues.Add(data.pointDataTable[i].position.x);
                Yvalues.Add(data.pointDataTable[i].position.y);
                Zvalues.Add(data.pointDataTable[i].position.z);
                Cvalues.Add(data.pointDataTable[i].color_index);
                Tvalues.Add(data.pointDataTable[i].time);
                PHvalues.Add(data.pointDataTable[i].phi_angle);
                THvalues.Add(data.pointDataTable[i].theta_angle);
                Svalues.Add(data.pointDataTable[i].size);
                pointnumber++;
                IDKEYDICT.Add(Xvalues.Count - 1, i); //We map the id of the point with 
                                                     //its index in the lists, just in case

            }

        }


        float[] XvaluesArray = Xvalues.ToArray();
        float[] YvaluesArray = Yvalues.ToArray();
        float[] ZvaluesArray = Zvalues.ToArray();
        float[] CvaluesArray = Cvalues.ToArray();
        float[] TvaluesArray = Tvalues.ToArray();
        float[] TrajectoriesArray = TrajectoriesList.ToArray();
        float[] PHvaluesArray = PHvalues.ToArray();
        float[] THvaluesArray = THvalues.ToArray();
        float[] SvaluesArray = Svalues.ToArray();

        CloudDataArrays Str = new CloudDataArrays(pointnumber, TrajectoriesArray, XvaluesArray, YvaluesArray, ZvaluesArray,
                                             CvaluesArray, TvaluesArray, PHvaluesArray, THvaluesArray, SvaluesArray);
        return Str;
    }

    private void ExecuteColumnFunction(AddColumnFunction myFunc)
    {
        //TODO : Add ID Values
        CloudData data = CloudUpdater.instance.LoadCurrentStatus();

        CloudDataArrays DataArrays = CreateCloudArrays();

        //TODO : Add dedicated function to Grab parameters 
        List<int> intparamList = new List<int>();
        string intParamsString = intParametersInputField.text;
        if (intParamsString.Length > 0)
        {
            string[] entries = intParamsString.Split(';');

            for (int k = 0; k < entries.Length; k++)
            {
                int parsed = int.Parse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                intparamList.Add(parsed);
            }
        }

        List<float> floatparamList = new List<float>();
        string floatParamsString = floatParametersInputField.text;
        if (floatParamsString.Length > 0)
        {
            string[] entries = floatParamsString.Split(';');

            for (int k = 0; k < entries.Length; k++)
            {
                float parsed = Single.Parse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                floatparamList.Add(parsed);
            }
        }

        List<double> doubleparamList = new List<double>();
        string doubleParamsString = doubleParametersInputField.text;
        if (doubleParamsString.Length > 0)
        {
            string[] entries = doubleParamsString.Split(';');

            for (int k = 0; k < entries.Length; k++)
            {
                double parsed = Double.Parse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                doubleparamList.Add(parsed);
                Debug.Log("double : " + k + " - " + parsed);
            }
        }


        int[] IntParams = intparamList.ToArray();
        float[] FloatParams = floatparamList.ToArray();
        double[] DoubleParams = doubleparamList.ToArray();
        //int[] IntParams = new int[1];

        float[] Results = new float[DataArrays.pointnumber];
        /**
        DEBUG 2D Array
        float[,] Results = new float[5,pointnumber];
        //How do we make this dynamic ?

        **/
        /**
        float[][] Results = new float[3][];
        for(int i = 0; i < Results.Length; i++)
        {
            Results[i] = new float[pointnumber];
        }
        **/
        //float[int, pointnumber]

        //Debug.Log("nullcheck : " + Results[0]);


        fixed (float* Trajectoriespointer = DataArrays.TrajectoriesList, xvalues = DataArrays.Xvalues, yvalues = DataArrays.Yvalues,
               zvalues = DataArrays.Zvalues, cvalues = DataArrays.Cvalues, tvalues = DataArrays.Tvalues, thvalues = DataArrays.THvalues,
               phvalues = DataArrays.PHvalues, svalues = DataArrays.Svalues, floatparams = FloatParams, results = Results)
        {
            fixed (int* intparams = IntParams)
            {
                fixed (double* doubleparams = DoubleParams)
                {
                    myFunc(DataArrays.pointnumber, xvalues, yvalues, zvalues, cvalues, tvalues, Trajectoriespointer,
                           phvalues, thvalues, svalues, intparams, floatparams, doubleparams, results);
                    //TODO
                    //Add *results in cloudData columns
                    //Calculate min and max for new column
                    //Check if we are working on a selection, if yes, put 0 as value for the unselected points

                    //Check if the right number of answers is provided
                    bool checkfull = true;

                    for (int i = 0; i < Results.Length; i++)
                    {
                        if (float.IsNaN(Results[i]))
                        {
                            Debug.Log("Result array is not full");
                            checkfull = false;
                            //Results = new float[0];
                        }
                    }

                    //if checkfull
                    if (checkfull)
                    {


                        float max = Mathf.NegativeInfinity;
                        float min = Mathf.Infinity;
                        if (selectionSent == false)
                        {
                            for (int i = 0; i < Results.Length; i++)
                            {
                                if (Results[i] <= min)
                                {
                                    min = Results[i];
                                }

                                if (Results[i] >= max)
                                {
                                    max = Results[i];
                                }
                            }
                            float[] restocopy = new float[DataArrays.pointnumber];
                            Results.CopyTo(restocopy, 0);
                            data.columnData.Add(restocopy);
                            ColumnMetadata metadata = new ColumnMetadata();
                            metadata.ColumnID = data.columnData.Count - 1;
                            metadata.MaxValue = max;
                            metadata.MinValue = min;
                            metadata.MinThreshold = min;
                            metadata.MaxThreshold = max;
                            metadata.Range = max - min;
                            data.globalMetaData.columnMetaDataList.Add(metadata);

                        }
                        else
                        {
                            float[] FullResults = new float[data.pointDataTable.Count];
                            for (int k = 0; k < data.pointDataTable.Count; k++)
                            {
                                FullResults[k] = 0f;
                            }
                            for (int j = 0; j < Results.Length; j++)
                            {
                                FullResults[IDKEYDICT[j]] = Results[j];
                            }
                            for (int i = 0; i < FullResults.Length; i++)
                            {
                                if (FullResults[i] <= min)
                                {
                                    min = FullResults[i];
                                }

                                if (FullResults[i] >= max)
                                {
                                    max = FullResults[i];
                                }
                            }
                            float[] restocopy = new float[data.pointDataTable.Count];
                            FullResults.CopyTo(restocopy, 0);
                            data.columnData.Add(restocopy);
                            ColumnMetadata metadata = new ColumnMetadata();
                            metadata.ColumnID = data.columnData.Count - 1;
                            metadata.MaxValue = max;
                            metadata.MinValue = min;
                            metadata.MinThreshold = min;
                            metadata.MaxThreshold = max;
                            metadata.Range = max - min;
                            data.globalMetaData.columnMetaDataList.Add(metadata);

                        }

                        ModalWindowManager.instance.CreateModalWindow("Column has been added to the data." + "\n");
                    }
                    else
                    {
                        ModalWindowManager.instance.CreateModalWindow("An error happened, the result column contains some unnasigned number." + "\n");
                    }
                    //endif
                }
            }
        }
    }

    private void ExecuteValueFunction(CalculateValueFunction myFunc)
    {
        List<float> TrajectoriesList = new List<float>();
        List<float> Xvalues = new List<float>();
        List<float> Yvalues = new List<float>();
        List<float> Zvalues = new List<float>();
        List<float> Cvalues = new List<float>();
        List<float> Tvalues = new List<float>();
        List<float> PHvalues = new List<float>();
        List<float> THvalues = new List<float>();
        List<float> Svalues = new List<float>();

        CloudData data = CloudUpdater.instance.LoadCurrentStatus();
        int pointnumber = 0;


        if (data.globalMetaData.SelectedPointsList.Count == 0)
        {

            Debug.Log("Sending all points");
            foreach (var i in data.pointDataTable)
            {
                TrajectoriesList.Add(i.Value.trajectory);
                Xvalues.Add(i.Value.position.x);
                Yvalues.Add(i.Value.position.y);
                Zvalues.Add(i.Value.position.z);
                Cvalues.Add(i.Value.color_index);
                Tvalues.Add(i.Value.time);
                PHvalues.Add(i.Value.phi_angle);
                THvalues.Add(i.Value.theta_angle);
                Svalues.Add(i.Value.size);
                pointnumber++;

            }
        }
        else
        {
            Debug.Log("Sending selected points");
            foreach (var i in data.globalMetaData.SelectedPointsList)
            {
                TrajectoriesList.Add(data.pointDataTable[i].trajectory);
                Xvalues.Add(data.pointDataTable[i].position.x);
                Yvalues.Add(data.pointDataTable[i].position.y);
                Zvalues.Add(data.pointDataTable[i].position.z);
                Cvalues.Add(data.pointDataTable[i].color_index);
                Tvalues.Add(data.pointDataTable[i].time);
                PHvalues.Add(data.pointDataTable[i].phi_angle);
                THvalues.Add(data.pointDataTable[i].theta_angle);
                Svalues.Add(data.pointDataTable[i].size);
                pointnumber++;

            }

        }
        float[] TrajectoriesArray = TrajectoriesList.ToArray();
        float[] XvaluesArray = Xvalues.ToArray();
        float[] YvaluesArray = Yvalues.ToArray();
        float[] ZvaluesArray = Zvalues.ToArray();
        float[] CvaluesArray = Cvalues.ToArray();
        float[] TvaluesArray = Tvalues.ToArray();
        float[] PHvaluesArray = PHvalues.ToArray();
        float[] THvaluesArray = THvalues.ToArray();
        float[] SvaluesArray = Svalues.ToArray();

        List<int> intparamList = new List<int>();
        string intParamsString = intParametersInputField.text;
        if (intParamsString.Length > 0)
        {
            string[] entries = intParamsString.Split(';');

            for (int k = 0; k < entries.Length; k++)
            {
                int parsed = int.Parse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                intparamList.Add(parsed);
            }
        }

        List<float> floatparamList = new List<float>();
        string floatParamsString = floatParametersInputField.text;
        if (floatParamsString.Length > 0)
        {
            string[] entries = floatParamsString.Split(';');

            for (int k = 0; k < entries.Length; k++)
            {
                float parsed = Single.Parse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                floatparamList.Add(parsed);
            }
        }

        List<double> doubleparamList = new List<double>();
        string doubleParamsString = doubleParametersInputField.text;
        if (doubleParamsString.Length > 0)
        {
            string[] entries = doubleParamsString.Split(';');

            for (int k = 0; k < entries.Length; k++)
            {
                double parsed = Double.Parse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                doubleparamList.Add(parsed);
            }
        }


        int[] IntParams = intparamList.ToArray();
        //int[] IntParams = new int[1];
        float[] FloatParams = floatparamList.ToArray();
        double[] DoubleParams = doubleparamList.ToArray();
        if (IntParams.Length > 0)
        {


            float[] Results = new float[IntParams[0]];
            /**
            float[][] Results = new float[3][];
            for(int i = 0; i < Results.Length; i++)
            {
                Results[i] = new float[pointnumber];
            }
            **/
            //float[int, pointnumber]

            Debug.Log("nullcheck : " + Results[0]);


            fixed (float* Trajectoriespointer = TrajectoriesArray, xvalues = XvaluesArray, yvalues = YvaluesArray,
                   zvalues = ZvaluesArray, cvalues = CvaluesArray, tvalues = TvaluesArray, thvalues = THvaluesArray,
                   phvalues = PHvaluesArray, svalues = SvaluesArray, floatparams = FloatParams, results = Results)
            {
                fixed (int* intparams = IntParams)
                {
                    fixed (double* doubleparams = DoubleParams)
                    {
                        //fixed (float** result = Results)
                        //{
                        //*result = new float[1][1];
                        myFunc(pointnumber, xvalues, yvalues, zvalues, cvalues, tvalues, Trajectoriespointer,
                               phvalues, thvalues, svalues, intparams, floatparams, doubleparams, results);
                        //TODO

                        //Add *results in cloudData 
                        //Add option to put names for the different results
                        string resultstring = "";
                        for (int i = 0; i < Results.Length; i++)
                        {
                            if (!float.IsNaN(Results[i]))
                            {
                                float res = Results[i];
                                Debug.Log(Results[i]);
                                resultstring = resultstring + "result " + i + " : " + res + "\n";
                            }
                        }

                        ModalWindowManager.instance.CreateModalWindow(resultstring);
                    }
                }
            }
        }
        else
        {
            ModalWindowManager.instance.CreateModalWindow("Error, the first int parameter in the list should be set as the number of values you wish to calculate.\nThe dll could not be loaded.");

        }
    }

    private void ExecuteNewCloudFunction(CreateNewCloudFunction myFunc)
    {
        //We need to call a first function to get the number of points the new cloud will contain.
        string functionName = "CalculateDataDimensions"; //TODO : Make name
        IntPtr FunctionAdress = GetProcAddress(LoadedDll, functionName);
        if (FunctionAdress == IntPtr.Zero) { Debug.Log("FUNC_ID is null"); }
        GetArraySizeFunction sizefunction = (GetArraySizeFunction)Marshal.GetDelegateForFunctionPointer(FunctionAdress, typeof(GetArraySizeFunction));
        int arraycolumn = 0;
        int arraylines = 0;

        CloudDataArrays DataArrays = CreateCloudArrays();


        //TODO : Add dedicated function to Grab parameters 
        List<int> intparamList = new List<int>();
        string intParamsString = intParametersInputField.text;
        if (intParamsString.Length > 0)
        {
            string[] entries = intParamsString.Split(';');

            for (int k = 0; k < entries.Length; k++)
            {
                int parsed = int.Parse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                intparamList.Add(parsed);
            }
        }

        List<float> floatparamList = new List<float>();
        string floatParamsString = floatParametersInputField.text;
        if (floatParamsString.Length > 0)
        {
            string[] entries = floatParamsString.Split(';');

            for (int k = 0; k < entries.Length; k++)
            {
                float parsed = Single.Parse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                floatparamList.Add(parsed);
            }
        }

        List<double> doubleparamList = new List<double>();
        string doubleParamsString = doubleParametersInputField.text;
        if (doubleParamsString.Length > 0)
        {
            string[] entries = doubleParamsString.Split(';');

            for (int k = 0; k < entries.Length; k++)
            {
                double parsed = Double.Parse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                doubleparamList.Add(parsed);
            }
        }

        float[][] Results;

        int[] IntParams = intparamList.ToArray();
        float[] FloatParams = floatparamList.ToArray();
        double[] DoubleParams = doubleparamList.ToArray();
        
        fixed (float* Trajectoriespointer = DataArrays.TrajectoriesList, xvalues = DataArrays.Xvalues, yvalues = DataArrays.Yvalues,
               zvalues = DataArrays.Zvalues, cvalues = DataArrays.Cvalues, tvalues = DataArrays.Tvalues, thvalues = DataArrays.THvalues,
               phvalues = DataArrays.PHvalues, svalues = DataArrays.Svalues, floatparams = FloatParams)
        {
            fixed (int* intparams = IntParams)
            {
                fixed (double* doubleparams = DoubleParams)
                {
                    int* LinesResult = &arraylines;
                    int* ColumnResult = &arraycolumn;

                    sizefunction(DataArrays.pointnumber,  xvalues, yvalues, zvalues,
                                 cvalues, tvalues, Trajectoriespointer, thvalues, phvalues, svalues, 
                                 intparams, floatparams, doubleparams, LinesResult, ColumnResult);


                    Debug.Log("lines : " + arraylines);
                    Debug.Log("columns : " + arraycolumn);

                    Results = new float[arraylines][];

                    for (int i = 0; i < arraylines; i++)
                    {
                        Results[i] = new float[arraycolumn];
                    }
                    //The function fills arraylines and arraycolumns
                }
            }
        }

        

        //Execute second function
        fixed (float* Trajectoriespointer = DataArrays.TrajectoriesList, xvalues = DataArrays.Xvalues, yvalues = DataArrays.Yvalues,
               zvalues = DataArrays.Zvalues, cvalues = DataArrays.Cvalues, tvalues = DataArrays.Tvalues, thvalues = DataArrays.THvalues,
               phvalues = DataArrays.PHvalues, svalues = DataArrays.Svalues, floatparams = FloatParams)
        {
          
            GCHandle[] pinnedArray = new GCHandle[Results.Length];
            float*[] ptrArray = new float*[Results.Length];
            for (int i = 0; i < Results.Length; i++)
            {
                pinnedArray[i] = GCHandle.Alloc(Results[i], GCHandleType.Pinned);
            }
            Debug.Log(Results.Length);
            for (int j = 0; j < Results.Length; j++)
            {
               //Debug.Log(j);
                // as you can see, this pointer will point to the first element of each array
                ptrArray[j] = (float*)pinnedArray[j].AddrOfPinnedObject();
            }

            // here is your double**
            fixed (float** results = &ptrArray[0])
            {



                fixed (int* intparams = IntParams)
                {
                    fixed (double* doubleparams = DoubleParams)
                    {
                        myFunc(DataArrays.pointnumber, xvalues, yvalues, zvalues,
                               cvalues, tvalues, Trajectoriespointer, thvalues, phvalues, svalues, 
                               intparams, floatparams, doubleparams, results);
                        //DEBUG
                        List<float[]> resultsList = new List<float[]>(); //List of length equal to the number of columns, each float[] inside contains all the values for the points in the column.
                        float[] TempArray;
                        for(int i = 0; i < arraycolumn; i++)
                        {
                            TempArray = new float[arraylines];
                            for(int j = 0; j<arraylines; j++)
                            {
                                TempArray[j] = Results[j][i];
                            }
                            resultsList.Add(TempArray);
                        }
                        /**
                         //DEBUG
                        for (int i = 0; i < Results.Length; i++) //Lines loop
                        {
                            for (int j = 0; j < Results[0].Length; j++) //Columns Loop
                            {
                                Debug.Log("i : " + i + " j : " + j + " value : " + Results[i][j]);
                            }
                        }
                        **/
                        CloudLoader.instance.LoadFromRawData(resultsList);

                    }
                }

                // unpin all the pinned objects,
                // otherwise they will live in memory till assembly unloading
                // even if they will went out of scope
                for (int i = 0; i < pinnedArray.Length; ++i)
                {
                    pinnedArray[i].Free();
                }
            }
        }
    }
}



