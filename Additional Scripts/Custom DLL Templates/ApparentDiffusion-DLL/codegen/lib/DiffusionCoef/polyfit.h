//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// polyfit.h
//
// Code generation for function 'polyfit'
//

#ifndef POLYFIT_H
#define POLYFIT_H

// Include files
#include "rtwtypes.h"
#include "coder_array.h"
#include <cstddef>
#include <cstdlib>

// Function Declarations
namespace coder {
void polyfit(const ::coder::array<double, 1U> &x,
             const ::coder::array<double, 1U> &y, double p[2]);

}

#endif
// End of code generation (polyfit.h)
