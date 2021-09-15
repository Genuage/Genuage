//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// pointCloud.h
//
// Code generation for function 'pointCloud'
//

#ifndef POINTCLOUD_H
#define POINTCLOUD_H

// Include files
#include "pointCloudArray.h"
#include "rtwtypes.h"
#include "coder_array.h"
#include "omp.h"
#include <cstddef>
#include <cstdlib>

// Type Definitions
namespace coder {
class pointCloud {
public:
  void matlabCodegenDestructor();
  ~pointCloud();
  pointCloud();
  boolean_T matlabCodegenIsDeleted;
  array<float, 2U> Location;
  array<unsigned char, 2U> Color;
  array<float, 2U> Normal;
  array<float, 2U> Intensity;
  void *Kdtree;
  void *LocationHandle;
  boolean_T HasKdtreeConstructed;
  boolean_T HasLocationHandleAllocated;
  array<pointclouds::internal::codegen::pc::pointCloudArray, 2U>
      PointCloudArrayData;
};

} // namespace coder

#endif
// End of code generation (pointCloud.h)
