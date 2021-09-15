// dllmain.cpp : Définit le point d'entrée de l'application DLL.
#include <stdlib.h>
#include <stdio.h>
#include "pch.h"
#include "function_tracking_new_cloud.h"
#include "function_tracking_new_cloud_terminate.h"
#include "rt_nonfinite.h"
#include "coder_array.h"

#define DllExport __declspec(dllexport)
static coder::array<double, 2U> argInit_Unboundedx4_real_T();
static double argInit_real_T();

//POINT CLOUD INFORMATION
int PointNumber; //The number of values sent in the various arrays
float* xCoordinatesArray; //x positions for each point
float* yCoordinatesArray; //y positions for each point
float* zCoordinatesArray; //z positions for each point
float* TimeArray; //time of apparition for each point
float* TrajectoryIDsArray; //trajectory identifier for each point, points with the same trajectoryid belong to the same trajectory
float* PhiAngleArray; //Phi angle for each point's orientation, UA
float* ThetaAngleArray; //Theta angle for each point's orientation, UA
float* PointSizesArray; //sizes for each points

//PARAMETERS
//These three arrays are provided to set any kind of numerical parameter as needed by your algorythm
int* IntParametersArray;
float* FloatParametersArray;
double* DoubleParametersArray;


float* ResultsArray; //Use this to hold the results to send between the two functions
int lines;
int columns;
coder::array<double, 2U> Data;
coder::array<double, 2U> Results;

// Function Definitions
static coder::array<double, 2U> argInit_Unboundedx4_real_T(int x, int y)
{
	coder::array<double, 2U> result;
	// Set the size of the array.
	// Change this size to the value that the application requires.
	result.set_size(x,y);
	// Loop over the array to initialize each element.
	for (int idx0{ 0 }; idx0 < result.size(0); idx0++) {
		for (int idx1{ 0 }; idx1 < y; idx1++) {
			// Set the value of the array element.
			// Change this value to the value that the application requires.
			result[idx0 + result.size(0) * idx1] = argInit_real_T();
		}
	}
	return result;
}

static double argInit_real_T()
{
	return 0.0;
}



extern "C" DllExport void CalculateDataDimensions(int point_number, void* x_coordinates, void* y_coordinates, void* z_coordinates,
	void* colors, void* times, void* trajectory_numbers, void* phi_angle,
	void* theta_angle, void* point_sizes,
	void* int_params, void* float_params, void* double_params,
	int* array_lines_number, int* array_columns_number)
{
	PointNumber = point_number;
	xCoordinatesArray = (float*)x_coordinates;
	yCoordinatesArray = (float*)y_coordinates;
	zCoordinatesArray = (float*)z_coordinates;
	TimeArray = (float*)times;
	TrajectoryIDsArray = (float*)trajectory_numbers;
	PhiAngleArray = (float*)phi_angle;
	ThetaAngleArray = (float*)theta_angle;
	PointSizesArray = (float*)point_sizes;

	IntParametersArray = (int*)int_params;
	FloatParametersArray = (float*)float_params;
	DoubleParametersArray = (double*)double_params;


	//create dataset
	//call function
	Data = argInit_Unboundedx4_real_T(point_number, 4);
	for (int idx0{ 0 }; idx0 < Data.size(0); idx0++) {
		
		Data[idx0 + (Data.size(0) * 0)] = (double)TimeArray[idx0];
		Data[idx0 + (Data.size(0) * 1)] = (double)xCoordinatesArray[idx0];
		Data[idx0 + (Data.size(0) * 2)] = (double)yCoordinatesArray[idx0];
		Data[idx0 + (Data.size(0) * 3)] = (double)zCoordinatesArray[idx0];
	}
	//Results = argInit_Unboundedx4_real_T(PointNumber, 4); // Bad, should return all available datas

	function_tracking_new_cloud(Data, 30, 30, 50, Results);
	lines = Results.size(0);
	columns = Results.size(1);
	*array_lines_number = lines;
	*array_columns_number = columns;
}

extern "C" DllExport void Execute(int point_number, void* x_coordinates, void* y_coordinates, void* z_coordinates,
	void* colors, void* times, void* trajectory_numbers, void* phi_angle,
	void* theta_angle, void* point_sizes,
	void* int_params, void* float_params, void* double_params,
	float** results)
{
	for (int i = 0; i < lines; i++) {
		for (int j = 0; j < columns; j++) {
			results[i][j] = (float)(Results[i + (Results.size(0) * j)]);
		}
	}

}



