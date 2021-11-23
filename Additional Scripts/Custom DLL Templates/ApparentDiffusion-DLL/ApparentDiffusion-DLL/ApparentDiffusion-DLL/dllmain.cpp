// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <stdio.h>

#include <stdlib.h>
#include <cmath>
#include "DiffusionCoef.h"
#include "DiffusionCoef_terminate.h"
#include "coder_array.h"

#define DllExport __declspec(dllexport)

//POINT CLOUD INFORMATION
int PointNumber; //The number of values sent in the various arrays
float* xCoordinatesArray; //x positions for each point
float* yCoordinatesArray; //y positions for each point
float* zCoordinatesArray; //z positions for each point
float* ColorArray; //
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


static int argInit_int32_T()
{
	return 0;
}

static double argInit_real_T()
{
	return 0.0;
}

static coder::array<double, 2U> argInit_Unboundedx5_real_T(int x)
{
	coder::array<double, 2U> result;
	// Set the size of the array.
	// Change this size to the value that the application requires.
	result.set_size(x, 5);
	// Loop over the array to initialize each element.
	for (int idx0{ 0 }; idx0 < result.size(0); idx0++) {
		for (int idx1{ 0 }; idx1 < 5; idx1++) {
			// Set the value of the array element.
			// Change this value to the value that the application requires.
			result[idx0 + result.size(0) * idx1] = argInit_real_T();
		}
	}
	return result;
}




extern "C" DllExport void Execute(int point_number, void* x_coordinates, void* y_coordinates, void* z_coordinates,
	void* colors, void* times, void* trajectory_numbers, void* phi_angle,
	void* theta_angle, void* point_sizes,
	void* int_params, void* float_params, void* double_params,
	float* results)
{
	//Assigning the point info and parameters sent from Genuage to local variables
	PointNumber = point_number;
	xCoordinatesArray = (float*)x_coordinates;
	yCoordinatesArray = (float*)y_coordinates;
	zCoordinatesArray = (float*)z_coordinates;
	ColorArray = (float*)colors;
	TimeArray = (float*)times;
	TrajectoryIDsArray = (float*)trajectory_numbers;
	PhiAngleArray = (float*)phi_angle;
	ThetaAngleArray = (float*)theta_angle;
	PointSizesArray = (float*)point_sizes;

	IntParametersArray = (int*)int_params;
	FloatParametersArray = (float*)float_params;
	DoubleParametersArray = (double*)double_params;

	//Your code goes here

	//The results parameter is an array of size equal to the first entry in the intparameterarray parameter, 
	//all entries in the array must be filled by your algorithm, after this function's execution, 
	//Genuage will access the array and display the values in a window.


	//Basic Code Example

	

	coder::array<double, 2U> tracks;
	double f_coefficient_tmp;
	// Initialize function 'DiffusionCoef' input arguments.
	// Initialize function input argument 'tracks'.
	tracks = argInit_Unboundedx5_real_T(PointNumber);

	//Data = argInit_Unboundedx4_real_T(point_number, 4);
	for (int idx0{ 0 }; idx0 < tracks.size(0); idx0++) {

		tracks[idx0 + (tracks.size(0) * 0)] = (double)xCoordinatesArray[idx0];
		tracks[idx0 + (tracks.size(0) * 1)] = (double)yCoordinatesArray[idx0];
		tracks[idx0 + (tracks.size(0) * 2)] = (double)zCoordinatesArray[idx0];
		tracks[idx0 + (tracks.size(0) * 3)] = (double)ColorArray[idx0];
		tracks[idx0 + (tracks.size(0) * 4)] = (double)TimeArray[idx0];
	}

	double positiontransform = (double)0.001;// DoubleParametersArray[0];//0.001;
	double timetransform = (double)0.02;// DoubleParametersArray[1];//0.02;
	double timeIncrement = (double)20.0;//IntParametersArray[1];//20;
	int dimension = 6;// IntParametersArray[2];//6;
	// Call the entry-point 'DiffusionCoef'.
	f_coefficient_tmp = DiffusionCoef(tracks, positiontransform, timetransform,
		timeIncrement, dimension);

	results[0] = (float)f_coefficient_tmp;
}


