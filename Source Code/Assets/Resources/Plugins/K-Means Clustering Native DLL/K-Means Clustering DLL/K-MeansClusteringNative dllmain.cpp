// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <stdio.h>
#include <stdlib.h>
#include "kmeans.h"

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



extern "C" DllExport void Execute(int point_number, void* x_coordinates, void* y_coordinates, void* z_coordinates,
	void* colors, void* times, void* trajectory_numbers, void* phi_angle,
	void* theta_angle, void* point_sizes,
	int cluster_number, int iteration_number,
	float* results, float* centroids_x, float* centroids_y, float* centroids_z)
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

	//Your code goes here

	//The results parameter is an array of size equal to the point_number parameter, 
	//all entries in the array must be filled by your algorythm, after this function's execution, 
	//Genuage will access the array and create a new column in the data from it's values.
	double* DoublexCoordinatesArray = new double[PointNumber];
	double* DoubleyCoordinatesArray = new double[PointNumber];
	double* DoublezCoordinatesArray = new double[PointNumber];

	for (int i = 0; i < PointNumber; i++) {
		DoublexCoordinatesArray[i] = (double)xCoordinatesArray[i];
		DoubleyCoordinatesArray[i] = (double)yCoordinatesArray[i];
		DoublezCoordinatesArray[i] = (double)zCoordinatesArray[i];
	}
	int clusterNumber = cluster_number;//IntParametersArray[0];
	int iterationNumber = iteration_number;//IntParametersArray[1];

	int* resultIntArray = new int[PointNumber];
	double* DoubleCentroidCoordinateArray_x = new double[cluster_number];
	double* DoubleCentroidCoordinateArray_y = new double[cluster_number];
	double* DoubleCentroidCoordinateArray_z = new double[cluster_number];


	LaunchKMeans(PointNumber, clusterNumber, iterationNumber,
				 DoublexCoordinatesArray, DoubleyCoordinatesArray, DoublezCoordinatesArray, 
				 resultIntArray,
				 DoubleCentroidCoordinateArray_x, DoubleCentroidCoordinateArray_y, DoubleCentroidCoordinateArray_z);
	//Basic Code Example
	//for (int i = 0; i < PointNumber; i++) {
	//	results[i] = FloatParametersArray[0];
	//}

	for (int i = 0; i < PointNumber; i++) {
		results[i] = (float)resultIntArray[i];
	}

	for (int j = 0; j < clusterNumber; j++) {
		centroids_x[j] = (float)DoubleCentroidCoordinateArray_x[j];
		centroids_y[j] = (float)DoubleCentroidCoordinateArray_y[j];
		centroids_z[j] = (float)DoubleCentroidCoordinateArray_z[j];
	}
	delete[] DoublexCoordinatesArray;
	delete[] DoubleyCoordinatesArray;
	delete[] DoublezCoordinatesArray;
	delete[] resultIntArray;
	delete[] DoubleCentroidCoordinateArray_x;
	delete[] DoubleCentroidCoordinateArray_y;
	delete[] DoubleCentroidCoordinateArray_z;
}


