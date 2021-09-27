//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// _coder_DiffusionCoef_api.h
//
// Code generation for function 'DiffusionCoef'
//

#ifndef _CODER_DIFFUSIONCOEF_API_H
#define _CODER_DIFFUSIONCOEF_API_H

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
real_T DiffusionCoef(coder::array<real_T, 2U> *tracks, real_T f_position,
                     real_T f_time, real_T dt, int32_T c);

void DiffusionCoef_api(const mxArray *const prhs[5], const mxArray **plhs);

void DiffusionCoef_atexit();

void DiffusionCoef_initialize();

void DiffusionCoef_terminate();

void DiffusionCoef_xil_shutdown();

void DiffusionCoef_xil_terminate();

#endif
// End of code generation (_coder_DiffusionCoef_api.h)
