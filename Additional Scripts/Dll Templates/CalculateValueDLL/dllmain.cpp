// dllmain.cpp : Defines the entry point for the DLL application.
#include <stdio.h>

#include <stdlib.h>

#define DllExport __declspec(dllexport)

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
	//float phisum = 0;
	//float thetasum = 0;
	//float phiaverage = 0;
	//float thetaaverage = 0;

	//for (int i = 0; i < PointNumber; i++) {
		//phisum += PhiAngleArray[i];
		//thetasum += ThetaAngleArray[i];
	//}
	//phiaverage = phisum / PointNumber;
	//thetaaverage = thetasum / PointNumber;


	//results[0] = phiaverage;
	//results[1] = thetaaverage;

}


