// dllmain.cpp : Définit le point d'entrée de l'application DLL.
#include <stdlib.h>
#include <stdio.h>
#include "pch.h"
#include "coder_array.h"
#include "CalculateDensity.h"

#define DllExport __declspec(dllexport)
//static coder::array<double, 2U> argInit_Unboundedx3_real32_T();
static float argInit_real32_T();

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
coder::array<float, 2U> Data;
coder::array<double, 2U> Results;


static coder::array<float, 2U> argInit_1xUnbounded_real32_T(int PointNumber)
{
	coder::array<float, 2U> result;
	// Set the size of the array.
	// Change this size to the value that the application requires.
	result.set_size(1, PointNumber);
	// Loop over the array to initialize each element.
	for (int idx0{ 0 }; idx0 < 1; idx0++) {
		for (int idx1{ 0 }; idx1 < result.size(1); idx1++) {
			// Set the value of the array element.
			// Change this value to the value that the application requires.
			result[idx1] = argInit_real32_T();
		}
	}
	return result;
}

// Function Definitions
static coder::array<float, 2U> argInit_Unboundedx3_real32_T(int pointNbr)
{
	coder::array<float, 2U> result;
	// Set the size of the array.
	// Change this size to the value that the application requires.
	result.set_size(pointNbr, 3);
	// Loop over the array to initialize each element.
	for (int idx0{ 0 }; idx0 < result.size(0); idx0++) {
		for (int idx1{ 0 }; idx1 < 3; idx1++) {
			// Set the value of the array element.
			// Change this value to the value that the application requires.
			result[idx0 + result.size(0) * idx1] = argInit_real32_T();
		}
	}
	return result;
}
static float argInit_real32_T()
{
	return 0.0F;
}

static double argInit_real_T()
{
	return 0.0;
}

extern "C" DllExport void Execute(int point_number, void* x_coordinates, void* y_coordinates, void* z_coordinates,
	void* colors, void* times, void* trajectory_numbers, void* phi_angle,
	void* theta_angle, void* point_sizes,
	void* int_params, void* float_params, void* double_params,
	float* results)
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
	Data = argInit_Unboundedx3_real32_T(PointNumber);
	for (int idx0{ 0 }; idx0 < Data.size(0); idx0++) {

		//Data[idx0 + (Data.size(0) * 0)] = (double)TimeArray[idx0];
		Data[idx0 + (Data.size(0) * 0)] = (double)xCoordinatesArray[idx0];
		Data[idx0 + (Data.size(0) * 1)] = (double)yCoordinatesArray[idx0];
		Data[idx0 + (Data.size(0) * 2)] = (double)zCoordinatesArray[idx0];
	}
	//Results = argInit_1xUnbounded_real32_T(PointNumber);
	double radius = DoubleParametersArray[0];

	CalculateDensity(Data, radius, Results);

	for (int idx0{ 0 }; idx0 < 1; idx0++) {
		for (int idx1{ 0 }; idx1 < Results.size(1); idx1++) {
			// Set the value of the array element.
			// Change this value to the value that the application requires.
			results[idx1] = Results[idx1];
		}
	}

	

}


