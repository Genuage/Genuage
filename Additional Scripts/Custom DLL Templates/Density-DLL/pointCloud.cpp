//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// pointCloud.cpp
//
// Code generation for function 'pointCloud'
//

// Include files
#include "pch.h"
#include "pointCloud.h"
#include "cvstCG_kdtree.h"

// Function Definitions
namespace coder {
pointCloud::pointCloud()
{
  this->matlabCodegenIsDeleted = true;
}

pointCloud::~pointCloud()
{
  this->matlabCodegenDestructor();
}

void pointCloud::matlabCodegenDestructor()
{
  if (!this->matlabCodegenIsDeleted) {
    this->matlabCodegenIsDeleted = true;
    if (this->HasLocationHandleAllocated) {
      kdtreeDeleteLocationDataPointer_single(this->LocationHandle);
      this->HasLocationHandleAllocated = false;
    }
    if (this->HasKdtreeConstructed) {
      kdtreeDeleteObj_single(this->Kdtree);
      this->HasKdtreeConstructed = false;
    }
  }
}

} // namespace coder

// End of code generation (pointCloud.cpp)
