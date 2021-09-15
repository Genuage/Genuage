//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// _coder_function_tracking_new_cloud_api.h
//
// Code generation for function 'function_tracking_new_cloud'
//

#ifndef _CODER_FUNCTION_TRACKING_NEW_CLOUD_API_H
#define _CODER_FUNCTION_TRACKING_NEW_CLOUD_API_H

// Include files
#include "coder_array_mex.h"
#include "emlrt.h"
#include "tmwtypes.h"
#include <algorithm>
#include <cstring>

// Variable Declarations
extern emlrtCTX emlrtRootTLSGlobal;
extern emlrtContext emlrtContextGlobal;

// Function Declarations
void function_tracking_new_cloud(coder::array<real_T, 2U> *data,
                                 real_T max_frame, real_T max_dxy,
                                 real_T max_dz,
                                 coder::array<real_T, 2U> *new_cloud);

void function_tracking_new_cloud_api(const mxArray *const prhs[4],
                                     const mxArray **plhs);

void function_tracking_new_cloud_atexit();

void function_tracking_new_cloud_initialize();

void function_tracking_new_cloud_terminate();

void function_tracking_new_cloud_xil_shutdown();

void function_tracking_new_cloud_xil_terminate();

#endif
// End of code generation (_coder_function_tracking_new_cloud_api.h)
