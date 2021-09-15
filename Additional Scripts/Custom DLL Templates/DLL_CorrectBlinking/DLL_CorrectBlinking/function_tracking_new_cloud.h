//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// function_tracking_new_cloud.h
//
// Code generation for function 'function_tracking_new_cloud'
//

#ifndef FUNCTION_TRACKING_NEW_CLOUD_H
#define FUNCTION_TRACKING_NEW_CLOUD_H

// Include files
#include "rtwtypes.h"
#include "coder_array.h"
#include <cstddef>
#include <cstdlib>

// Function Declarations
extern void function_tracking_new_cloud(const coder::array<double, 2U> &data,
                                        double max_frame, double max_dxy,
                                        double max_dz,
                                        coder::array<double, 2U> &new_cloud);

#endif
// End of code generation (function_tracking_new_cloud.h)
