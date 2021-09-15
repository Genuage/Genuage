//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// _coder_function_tracking_new_cloud_mex.h
//
// Code generation for function 'function_tracking_new_cloud'
//

#ifndef _CODER_FUNCTION_TRACKING_NEW_CLOUD_MEX_H
#define _CODER_FUNCTION_TRACKING_NEW_CLOUD_MEX_H

// Include files
#include "emlrt.h"
#include "mex.h"
#include "tmwtypes.h"

// Function Declarations
MEXFUNCTION_LINKAGE void mexFunction(int32_T nlhs, mxArray *plhs[],
                                     int32_T nrhs, const mxArray *prhs[]);

emlrtCTX mexFunctionCreateRootTLS();

void unsafe_function_tracking_new_cloud_mexFunction(int32_T nlhs,
                                                    mxArray *plhs[1],
                                                    int32_T nrhs,
                                                    const mxArray *prhs[4]);

#endif
// End of code generation (_coder_function_tracking_new_cloud_mex.h)
