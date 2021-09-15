//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// repmat.cpp
//
// Code generation for function 'repmat'
//

// Include files
#include "pch.h"
#include "repmat.h"
#include "rt_nonfinite.h"
#include "coder_array.h"
// Function Definitions
namespace coder {
void repmat(const ::coder::array<double, 1U> &a, const double varargin_1[2],
            ::coder::array<double, 2U> &b)
{
  int nrows;
  int ntilecols;
  b.set_size(
      static_cast<int>(static_cast<short>(a.size(0))),
      static_cast<int>(static_cast<short>(static_cast<int>(varargin_1[1]))));
  nrows = a.size(0);
  ntilecols = static_cast<int>(varargin_1[1]);
  for (int jtilecol{0}; jtilecol < ntilecols; jtilecol++) {
    int ibtile;
    ibtile = jtilecol * nrows;
    for (int k{0}; k < nrows; k++) {
      b[ibtile + k] = a[k];
    }
  }
}

void repmat(const ::coder::array<double, 2U> &a, const double varargin_1[2],
            ::coder::array<double, 2U> &b)
{
  int ncols;
  int ntilerows;
  b.set_size(
      static_cast<int>(static_cast<short>(static_cast<int>(varargin_1[0]))),
      static_cast<int>(static_cast<short>(a.size(1))));
  ncols = a.size(1);
  ntilerows = static_cast<int>(varargin_1[0]);
  for (int jcol{0}; jcol < ncols; jcol++) {
    int ibmat;
    ibmat = jcol * ntilerows;
    for (int itilerow{0}; itilerow < ntilerows; itilerow++) {
      b[ibmat + itilerow] = a[jcol];
    }
  }
}

} // namespace coder

// End of code generation (repmat.cpp)
