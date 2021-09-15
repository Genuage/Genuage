//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// CalculateDensity.cpp
//
// Code generation for function 'CalculateDensity'
//

// Include files
#include "pch.h"
#include "CalculateDensity.h"
#include "pointCloud.h"
#include "pointCloudArray.h"
#include "coder_array.h"
#include "cvstCG_ComputeMetric.h"
#include "cvstCG_kdtree.h"
#include <cmath>

// Function Definitions
void CalculateDensity(const coder::array<float, 2U> &positions, double radius,
                      coder::array<double, 2U> &density)
{
  coder::pointCloud PtCloud;
  coder::pointclouds::internal::codegen::pc::pointCloudArray r;
  coder::array<float, 2U> allDists;
  coder::array<float, 2U> location;
  coder::array<float, 2U> r1;
  coder::array<float, 1U> dists_;
  coder::array<unsigned int, 1U> indices_;
  coder::array<short, 2U> ii;
  coder::array<boolean_T, 2U> b_x;
  coder::array<boolean_T, 1U> b_isFinite;
  coder::array<boolean_T, 1U> r2;
  float point[3];
  int i;
  int nx;
  unsigned int x;
  // filename='C:\Users\Thomas\Downloads\005_FISH-QUANT__all_spots_210303_th.txt';
  // radius = 500;
  // density_result = [];
  // a=importdata(filename);
  // density_result = calculate_density(a(:,2:4), radius);
  density.set_size(1, positions.size(0));
  PtCloud.HasKdtreeConstructed = false;
  PtCloud.HasLocationHandleAllocated = false;
  PtCloud.Location.set_size(positions.size(0), 3);
  nx = positions.size(0) * 3;
  for (i = 0; i < nx; i++) {
    PtCloud.Location[i] = positions[i];
  }
  PtCloud.Color.set_size(0, 0);
  PtCloud.Normal.set_size(0, 0);
  PtCloud.Intensity.set_size(0, 0);
  PtCloud.PointCloudArrayData.set_size(1, 1);
  PtCloud.PointCloudArrayData[0] = r;
  PtCloud.Kdtree = NULL;
  PtCloud.LocationHandle = NULL;
  PtCloud.matlabCodegenIsDeleted = false;
  i = positions.size(0);
  for (int b_i{0}; b_i < i; b_i++) {
    // PtCloud = pointCloud(positions);
    point[0] = positions[b_i];
    point[1] = positions[b_i + positions.size(0)];
    point[2] = positions[b_i + positions.size(0) * 2];
    nx = positions.size(0);
    if (positions.size(0) < 500) {
      double c;
      int b_ii;
      int idx;
      boolean_T exitg1;
      allDists.set_size(1, positions.size(0));
      for (idx = 0; idx < nx; idx++) {
        allDists[idx] = 0.0F;
      }
      point[0] = positions[b_i];
      point[1] = positions[b_i + positions.size(0)];
      point[2] = positions[b_i + positions.size(0) * 2];
      nx = positions.size(0);
      r1.set_size(3, positions.size(0));
      for (idx = 0; idx < nx; idx++) {
        r1[3 * idx] = positions[idx];
        r1[3 * idx + 1] = positions[idx + positions.size(0)];
        r1[3 * idx + 2] = positions[idx + positions.size(0) * 2];
      }
      ComputeMetric_ssd_single(&point[0], &r1[0], &allDists[0], 1U,
                               static_cast<unsigned int>(positions.size(0)),
                               3U);
      c = radius * radius;
      b_x.set_size(1, allDists.size(1));
      nx = allDists.size(1);
      for (idx = 0; idx < nx; idx++) {
        b_x[idx] = (allDists[idx] <= c);
      }
      nx = b_x.size(1);
      idx = 0;
      ii.set_size(1, b_x.size(1));
      b_ii = 0;
      exitg1 = false;
      while ((!exitg1) && (b_ii <= nx - 1)) {
        if (b_x[b_ii]) {
          idx++;
          ii[idx - 1] = static_cast<short>(b_ii + 1);
          if (idx >= nx) {
            exitg1 = true;
          } else {
            b_ii++;
          }
        } else {
          b_ii++;
        }
      }
      if (b_x.size(1) == 1) {
        if (idx == 0) {
          ii.set_size(1, 0);
        }
      } else {
        if (1 > idx) {
          idx = 0;
        }
        ii.set_size(ii.size(0), idx);
      }
      indices_.set_size(ii.size(1));
      nx = ii.size(1);
      for (idx = 0; idx < nx; idx++) {
        indices_[idx] = static_cast<unsigned int>(ii[idx]);
      }
      if (indices_.size(0) != 0) {
        dists_.set_size(indices_.size(0));
        nx = indices_.size(0);
        for (idx = 0; idx < nx; idx++) {
          dists_[idx] = allDists[static_cast<int>(indices_[idx]) - 1];
        }
        b_isFinite.set_size(dists_.size(0));
        nx = dists_.size(0);
        for (idx = 0; idx < nx; idx++) {
          b_isFinite[idx] = std::isinf(dists_[idx]);
        }
        r2.set_size(dists_.size(0));
        nx = dists_.size(0);
        for (idx = 0; idx < nx; idx++) {
          r2[idx] = std::isnan(dists_[idx]);
        }
        nx = b_isFinite.size(0);
        for (idx = 0; idx < nx; idx++) {
          b_isFinite[idx] = ((!b_isFinite[idx]) && (!r2[idx]));
        }
        nx = b_isFinite.size(0);
        idx = 0;
        for (b_ii = 0; b_ii < nx; b_ii++) {
          if (b_isFinite[b_ii]) {
            idx++;
          }
        }
        indices_.set_size(idx);
      }
    } else {
      void *ptrDists;
      void *ptrIndices;
      boolean_T createIndex;
      if (!PtCloud.HasLocationHandleAllocated) {
        location.set_size(positions.size(0), 3);
        nx = positions.size(0) * 3;
        for (int idx{0}; idx < nx; idx++) {
          location[idx] = positions[idx];
        }
        PtCloud.LocationHandle = NULL;
        kdtreeGetLocationDataPointer_single(
            &location[0], static_cast<unsigned int>(positions.size(0)), 3U,
            &PtCloud.LocationHandle);
        PtCloud.HasLocationHandleAllocated = true;
      }
      if (!PtCloud.HasKdtreeConstructed) {
        PtCloud.Kdtree = NULL;
        kdtreeConstruct_single(&PtCloud.Kdtree);
        PtCloud.HasKdtreeConstructed = true;
        createIndex = true;
      } else {
        createIndex =
            kdtreeNeedsReindex_single(PtCloud.Kdtree, PtCloud.LocationHandle);
      }
      if (createIndex) {
        kdtreeIndex_single(PtCloud.Kdtree, PtCloud.LocationHandle,
                           static_cast<unsigned int>(positions.size(0)), 3U,
                           4.0, 1.0, 0.0);
      }
      ptrIndices = NULL;
      ptrDists = NULL;
      kdtreeRadiusSearch_single(PtCloud.Kdtree, &point[0], 1U, 3U,
                                static_cast<float>(radius), 0.0, 0.0F,
                                &ptrIndices, &ptrDists, &x, 1000, 500U);
      indices_.set_size(static_cast<int>(x));
      dists_.set_size(static_cast<int>(x));
      kdtreeRadiusSearchSetOutputs_single(
          ptrIndices, ptrDists, &(indices_.data())[0], &(dists_.data())[0]);
    }
    density[b_i] = indices_.size(0);
  }
}

// End of code generation (CalculateDensity.cpp)
