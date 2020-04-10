#pragma once

#include "IUnityInterface.h"

#define DllExport __declspec(dllexport)

//void dfpmin(double p[], int ndim, double gtol, int iter[], double fret[], double (func)(double[]), void (dfunc)(double (func)(double[]), double[], double[]));
//void lnsrch(int ndim, double xold[], double fold, double g[], double p[], double xx[], double f[], double stpmax, int check[], double (func)(double[]));
//void dfunc(double (func)(double *), double *yy, double *ans);
extern "C" void Infer3D(int NumberOfPoints, void* TrajectoryNumber, void* xCoordinates, void* yCoordinates, void* zCoordinates, void* TimeStamp, double* Diffusion, double* ForceX, double* ForceY, double* ForceZ);
