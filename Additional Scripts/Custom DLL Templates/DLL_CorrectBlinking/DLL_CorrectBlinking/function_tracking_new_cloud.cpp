//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// function_tracking_new_cloud.cpp
//
// Code generation for function 'function_tracking_new_cloud'
//

// Include files
#include "pch.h"

#include "function_tracking_new_cloud.h"
#include "colon.h"
#include "find.h"
#include "mean.h"
#include "minOrMax.h"
#include "repmat.h"
#include "rt_nonfinite.h"
#include "coder_array.h"
#include <cmath>

// Type Definitions
struct cell_wrap_0 {
  coder::array<double, 2U> f1;
};

// Function Declarations
static int div_s32(int numerator, int denominator);

// Function Definitions
static int div_s32(int numerator, int denominator)
{
  int quotient;
  if (denominator == 0) {
    if (numerator >= 0) {
      quotient = MAX_int32_T;
    } else {
      quotient = MIN_int32_T;
    }
  } else {
    unsigned int b_denominator;
    unsigned int b_numerator;
    if (numerator < 0) {
      b_numerator = ~static_cast<unsigned int>(numerator) + 1U;
    } else {
      b_numerator = static_cast<unsigned int>(numerator);
    }
    if (denominator < 0) {
      b_denominator = ~static_cast<unsigned int>(denominator) + 1U;
    } else {
      b_denominator = static_cast<unsigned int>(denominator);
    }
    b_numerator /= b_denominator;
    if ((numerator < 0) != (denominator < 0)) {
      quotient = -static_cast<int>(b_numerator);
    } else {
      quotient = static_cast<int>(b_numerator);
    }
  }
  return quotient;
}

void function_tracking_new_cloud(const coder::array<double, 2U> &data,
                                 double max_frame, double max_dxy,
                                 double max_dz,
                                 coder::array<double, 2U> &new_cloud)
    //TODO : Get number of points and replace all the magic numbers in arrays
{
  coder::array<cell_wrap_0, 1U> MapPoint30Frame_init;
  coder::array<cell_wrap_0, 1U> MapPointFrame;
  coder::array<cell_wrap_0, 1U> MapPointFrame_init;
  coder::array<double, 2U> Data_result;
  coder::array<double, 2U> c_Datax;
  coder::array<double, 2U> r2;
  coder::array<double, 2U> r4;
  coder::array<double, 2U> r5;
  coder::array<double, 2U> x;
  coder::array<double, 2U> y;
  coder::array<double, 1U> Datax;
  coder::array<double, 1U> Datay;
  coder::array<double, 1U> Dataz;
  coder::array<double, 1U> b_Datax;
  coder::array<double, 1U> b_data;
  coder::array<int, 1U> b_I;
  coder::array<int, 1U> r;
  coder::array<int, 1U> r1;
  coder::array<int, 1U> r3;
  coder::array<int, 1U> r6;
  coder::array<boolean_T, 2U> same;
  coder::array<boolean_T, 1U> idx;
  double b_MapPointFrame[2];
  double nb_frames;
  int b_tmp_data[10000];
  unsigned int idxi_data[10000];
  unsigned int idxj_data[10000];
  int tmp_data[10000];
  int b_i;
  int i;
  int i1;
  int loop_ub;
  int nx;
  int partialTrueCount;
  int result;
  int result_idx_1_tmp;
  int unnamed_idx_0_tmp;
  short siz_idx_0;
  signed char b_input_sizes_idx_1;
  signed char c_input_sizes_idx_1;
  signed char input_sizes_idx_1;
  signed char sizes_idx_1;
  boolean_T idx_notlabeled_data[10000];
  boolean_T empty_non_axis_sizes;
  // function_tracking
  //    Tracking of molecules among all the detections in all the frames of the
  //    acquistion Inputs data = matrix of detections. 1st column: nb of frame;
  //    columns 2-4: position of the detection (x, y, z) max_frame = max number
  //    of following frames in which the molecules of a given frame are tracked
  //    max_dxy = max distance between two detections in xy plane so that it
  //    belongs to the same molecule [nm] max_dz = max distance between two
  //    detections in z direction so that it belongs to the same molecule [nm]
  //    Outputs
  //    Data_result = matrix of results. column 1-4: same as entry; column 5:
  //    index of the detected molecule; column6: ocurrence of the molecule;
  //    column 7: total nb of coccurences of this molecule
  // Isolate coordinates of all points
  loop_ub = data.size(0);
  Datax.set_size(data.size(0));
  for (i = 0; i < loop_ub; i++) {
    Datax[i] = data[i + data.size(0) * 2];
  }
  loop_ub = data.size(0);
  Datay.set_size(data.size(0));
  for (i = 0; i < loop_ub; i++) {
    Datay[i] = data[i + data.size(0)];
  }
  loop_ub = data.size(0);
  Dataz.set_size(data.size(0));
  for (i = 0; i < loop_ub; i++) {
    Dataz[i] = data[i + data.size(0) * 3];
  }
  loop_ub = data.size(0);
  b_data.set_size(data.size(0));
  for (i = 0; i < loop_ub; i++) {
    b_data[i] = data[i];
  }
  nb_frames = coder::internal::maximum(b_data);
  //  nb of frames in the acquisition
  // Create the final matrix of results
  if (data.size(0) != 0) {
    result = data.size(0);
  } else {
    result = 0;
  }
  empty_non_axis_sizes = (result == 0);
  if (empty_non_axis_sizes || (data.size(0) != 0)) {
    input_sizes_idx_1 = 4;
  } else {
    input_sizes_idx_1 = 0;
  }
  if (empty_non_axis_sizes || (data.size(0) != 0)) {
    b_input_sizes_idx_1 = 1;
  } else {
    b_input_sizes_idx_1 = 0;
  }
  if (empty_non_axis_sizes || (data.size(0) != 0)) {
    c_input_sizes_idx_1 = 1;
  } else {
    c_input_sizes_idx_1 = 0;
  }
  if (empty_non_axis_sizes || (data.size(0) != 0)) {
    sizes_idx_1 = 1;
  } else {
    sizes_idx_1 = 0;
  }
  nx = input_sizes_idx_1;
  result_idx_1_tmp = b_input_sizes_idx_1;
  i = input_sizes_idx_1 + b_input_sizes_idx_1;
  i1 = i + c_input_sizes_idx_1;
  Data_result.set_size(result, i1 + sizes_idx_1);
  for (partialTrueCount = 0; partialTrueCount < nx; partialTrueCount++) {
    for (unnamed_idx_0_tmp = 0; unnamed_idx_0_tmp < result;
         unnamed_idx_0_tmp++) {
      Data_result[unnamed_idx_0_tmp + Data_result.size(0) * partialTrueCount] =
          data[unnamed_idx_0_tmp + data.size(0) * partialTrueCount];
    }
  }
  for (partialTrueCount = 0; partialTrueCount < result_idx_1_tmp;
       partialTrueCount++) {
    for (unnamed_idx_0_tmp = 0; unnamed_idx_0_tmp < result;
         unnamed_idx_0_tmp++) {
      Data_result[unnamed_idx_0_tmp + Data_result.size(0) * input_sizes_idx_1] =
          0.0;
    }
  }
  loop_ub = c_input_sizes_idx_1;
  for (partialTrueCount = 0; partialTrueCount < loop_ub; partialTrueCount++) {
    for (unnamed_idx_0_tmp = 0; unnamed_idx_0_tmp < result;
         unnamed_idx_0_tmp++) {
      Data_result[unnamed_idx_0_tmp + Data_result.size(0) * i] = 0.0;
    }
  }
  loop_ub = sizes_idx_1;
  for (i = 0; i < loop_ub; i++) {
    for (partialTrueCount = 0; partialTrueCount < result; partialTrueCount++) {
      Data_result[partialTrueCount + Data_result.size(0) * i1] = 0.0;
    }
  }
  // column5=index du point %column6=ocurrence %column7=total nb of coccurence
  // Step 1: create dico points in each frame
  unnamed_idx_0_tmp = static_cast<int>(coder::internal::maximum(b_data) + 1.0);
  i = static_cast<int>(nb_frames + 1.0);
  MapPointFrame_init.set_size(i);
  i = static_cast<int>(nb_frames + 1.0);
  MapPoint30Frame_init.set_size(i);
  for (i = 0; i < unnamed_idx_0_tmp; i++) {
    MapPointFrame_init[i].f1.set_size(1, 0);
    MapPoint30Frame_init[i].f1.set_size(1, 0);
  }
  MapPointFrame.set_size(MapPointFrame_init.size(0));
  MapPointFrame_init.set_size(MapPoint30Frame_init.size(0));
  nb_frames = 0.0;
  nx = 0;
  // [];
  i = Data_result.size(0);
  for (b_i = 0; b_i < i; b_i++) {
    if (Data_result[b_i] != nb_frames) {
      unsigned int j;
      MapPointFrame[static_cast<int>(nb_frames + 1.0) - 1].f1.set_size(1, nx);
      for (i1 = 0; i1 < nx; i1++) {
        MapPointFrame[static_cast<int>(nb_frames + 1.0) - 1].f1[i1] =
            idxi_data[i1];
      }
      nx = 0;
      result_idx_1_tmp = 0;
      j = static_cast<unsigned int>(b_i + 1);
      while ((j <= static_cast<unsigned int>(Data_result.size(0))) &&
             (Data_result[static_cast<int>(j) - 1] <= nb_frames + max_frame)) {
        i1 = result_idx_1_tmp;
        result_idx_1_tmp++;
        idxj_data[i1] = j;
        j++;
      }
      MapPointFrame_init[static_cast<int>(nb_frames + 1.0) - 1].f1.set_size(
          1, result_idx_1_tmp);
      for (i1 = 0; i1 < result_idx_1_tmp; i1++) {
        MapPointFrame_init[static_cast<int>(nb_frames + 1.0) - 1].f1[i1] =
            idxj_data[i1];
      }
      nb_frames = Data_result[b_i];
    }
    i1 = nx;
    nx++;
    idxi_data[i1] = static_cast<unsigned int>(b_i + 1);
  }
  MapPointFrame[static_cast<int>(nb_frames + 1.0) - 1].f1.set_size(1, nx);
  for (i = 0; i < nx; i++) {
    MapPointFrame[static_cast<int>(nb_frames + 1.0) - 1].f1[i] = idxi_data[i];
  }
  MapPointFrame_init[static_cast<int>(nb_frames + 1.0) - 1].f1.set_size(1, nx);
  for (i = 0; i < nx; i++) {
    MapPointFrame_init[static_cast<int>(nb_frames + 1.0) - 1].f1[i] =
        idxi_data[i];
  }
  // Step 2: Loop over all the frames
  nb_frames = 0.0;
  for (b_i = 0; b_i < unnamed_idx_0_tmp; b_i++) {
    //      if mod(i,500) == 1
    //          fprintf('i = %d \n',i)
    //      end
    loop_ub = MapPointFrame[b_i].f1.size(1);
    if (MapPointFrame[b_i].f1.size(1) != 0) {
      double b;
      // label the points detected at time i, only if they are not already
      // labeled
      nx = MapPointFrame[b_i].f1.size(1);
      for (i = 0; i < loop_ub; i++) {
        idx_notlabeled_data[i] =
            (Data_result[(static_cast<int>(MapPointFrame[b_i].f1[i]) +
                          Data_result.size(0) * 4) -
                         1] == 0.0);
      }
      // condition for not being labeled
      // indices of the points which remain to label
      nx--;
      result = 0;
      for (result_idx_1_tmp = 0; result_idx_1_tmp <= nx; result_idx_1_tmp++) {
        if (idx_notlabeled_data[result_idx_1_tmp]) {
          result++;
        }
      }
      partialTrueCount = 0;
      for (result_idx_1_tmp = 0; result_idx_1_tmp <= nx; result_idx_1_tmp++) {
        if (idx_notlabeled_data[result_idx_1_tmp]) {
          tmp_data[partialTrueCount] = result_idx_1_tmp + 1;
          partialTrueCount++;
        }
      }
      for (i = 0; i < result; i++) {
        b_tmp_data[i] = static_cast<int>(static_cast<unsigned int>(
                            MapPointFrame[b_i].f1[tmp_data[i] - 1])) -
                        1;
      }
      result = 0;
      for (result_idx_1_tmp = 0; result_idx_1_tmp <= nx; result_idx_1_tmp++) {
        if (idx_notlabeled_data[result_idx_1_tmp]) {
          result++;
        }
      }
      b = nb_frames + static_cast<double>(static_cast<short>(result));
      if (b < nb_frames + 1.0) {
        y.set_size(1, 0);
      } else if ((std::isinf(nb_frames + 1.0) || std::isinf(b)) &&
                 (nb_frames + 1.0 == b)) {
        y.set_size(1, 1);
        y[0] = rtNaN;
      } else if (nb_frames + 1.0 == nb_frames + 1.0) {
        loop_ub = static_cast<int>(b - (nb_frames + 1.0));
        y.set_size(1, loop_ub + 1);
        for (i = 0; i <= loop_ub; i++) {
          y[i] = (nb_frames + 1.0) + static_cast<double>(i);
        }
      } else {
        coder::eml_float_colon(nb_frames + 1.0, b, y);
      }
      loop_ub = y.size(1);
      for (i = 0; i < loop_ub; i++) {
        Data_result[b_tmp_data[i] + Data_result.size(0) * 4] = y[i];
      }
      // number the points of next frame without any label yet
      result = 0;
      for (result_idx_1_tmp = 0; result_idx_1_tmp <= nx; result_idx_1_tmp++) {
        if (idx_notlabeled_data[result_idx_1_tmp]) {
          result++;
        }
      }
      nb_frames += static_cast<double>(static_cast<short>(result));
      // coordinates at time i
      // Distance matrices
      // Tracking : create matrix same of size [nb of points at time i X nb of
      // points at time i+1]
      loop_ub = MapPointFrame[b_i].f1.size(1);
      b_Datax.set_size(MapPointFrame[b_i].f1.size(1));
      for (i = 0; i < loop_ub; i++) {
        b_Datax[i] = Datax[static_cast<int>(MapPointFrame[b_i].f1[i]) - 1];
      }
      b_MapPointFrame[0] = 1.0;
      b_MapPointFrame[1] = MapPointFrame_init[b_i].f1.size(1);
      coder::repmat(b_Datax, b_MapPointFrame, r2);
      c_Datax.set_size(1, MapPointFrame_init[b_i].f1.size(1));
      loop_ub = MapPointFrame_init[b_i].f1.size(1);
      for (i = 0; i < loop_ub; i++) {
        c_Datax[i] = Datax[static_cast<int>(MapPointFrame_init[b_i].f1[i]) - 1];
      }
      b_MapPointFrame[0] = MapPointFrame[b_i].f1.size(1);
      b_MapPointFrame[1] = 1.0;
      coder::repmat(c_Datax, b_MapPointFrame, r4);
      r5.set_size(r2.size(0), r2.size(1));
      loop_ub = r2.size(0) * r2.size(1);
      for (i = 0; i < loop_ub; i++) {
        r5[i] = r2[i] - r4[i];
      }
      x.set_size(static_cast<int>(static_cast<short>(r5.size(0))),
                 static_cast<int>(static_cast<short>(r5.size(1))));
      nx = static_cast<short>(r5.size(0)) * static_cast<short>(r5.size(1));
      for (result = 0; result < nx; result++) {
        x[result] = r5[result] * r5[result];
      }
      loop_ub = MapPointFrame[b_i].f1.size(1);
      b_Datax.set_size(MapPointFrame[b_i].f1.size(1));
      for (i = 0; i < loop_ub; i++) {
        b_Datax[i] = Datay[static_cast<int>(MapPointFrame[b_i].f1[i]) - 1];
      }
      b_MapPointFrame[0] = 1.0;
      b_MapPointFrame[1] = MapPointFrame_init[b_i].f1.size(1);
      coder::repmat(b_Datax, b_MapPointFrame, r2);
      c_Datax.set_size(1, MapPointFrame_init[b_i].f1.size(1));
      loop_ub = MapPointFrame_init[b_i].f1.size(1);
      for (i = 0; i < loop_ub; i++) {
        c_Datax[i] = Datay[static_cast<int>(MapPointFrame_init[b_i].f1[i]) - 1];
      }
      b_MapPointFrame[0] = MapPointFrame[b_i].f1.size(1);
      b_MapPointFrame[1] = 1.0;
      coder::repmat(c_Datax, b_MapPointFrame, r4);
      r5.set_size(r2.size(0), r2.size(1));
      loop_ub = r2.size(0) * r2.size(1);
      for (i = 0; i < loop_ub; i++) {
        r5[i] = r2[i] - r4[i];
      }
      r2.set_size(static_cast<int>(static_cast<short>(r5.size(0))),
                  static_cast<int>(static_cast<short>(r5.size(1))));
      nx = static_cast<short>(r5.size(0)) * static_cast<short>(r5.size(1));
      for (result = 0; result < nx; result++) {
        r2[result] = r5[result] * r5[result];
      }
      loop_ub = x.size(0) * x.size(1);
      for (i = 0; i < loop_ub; i++) {
        x[i] = x[i] + r2[i];
      }
      nx = x.size(0) * x.size(1);
      for (result = 0; result < nx; result++) {
        x[result] = std::sqrt(x[result]);
      }
      loop_ub = MapPointFrame[b_i].f1.size(1);
      b_Datax.set_size(MapPointFrame[b_i].f1.size(1));
      for (i = 0; i < loop_ub; i++) {
        b_Datax[i] = Dataz[static_cast<int>(MapPointFrame[b_i].f1[i]) - 1];
      }
      b_MapPointFrame[0] = 1.0;
      b_MapPointFrame[1] = MapPointFrame_init[b_i].f1.size(1);
      coder::repmat(b_Datax, b_MapPointFrame, r2);
      c_Datax.set_size(1, MapPointFrame_init[b_i].f1.size(1));
      loop_ub = MapPointFrame_init[b_i].f1.size(1);
      for (i = 0; i < loop_ub; i++) {
        c_Datax[i] = Dataz[static_cast<int>(MapPointFrame_init[b_i].f1[i]) - 1];
      }
      b_MapPointFrame[0] = MapPointFrame[b_i].f1.size(1);
      b_MapPointFrame[1] = 1.0;
      coder::repmat(c_Datax, b_MapPointFrame, r4);
      r5.set_size(r2.size(0), r2.size(1));
      loop_ub = r2.size(0) * r2.size(1);
      for (i = 0; i < loop_ub; i++) {
        r5[i] = r2[i] - r4[i];
      }
      nx = r5.size(0) * r5.size(1);
      r2.set_size(static_cast<int>(static_cast<short>(r5.size(0))),
                  static_cast<int>(static_cast<short>(r5.size(1))));
      for (result = 0; result < nx; result++) {
        r2[result] = std::abs(r5[result]);
      }
      same.set_size(x.size(0), x.size(1));
      loop_ub = x.size(0) * x.size(1);
      for (i = 0; i < loop_ub; i++) {
        same[i] = ((x[i] < max_dxy) + (r2[i] < max_dz) == 2);
      }
      // ==1 if same particle (boolean matrix); line=frame i ; column=frame
      // (i+j)
      coder::eml_find(same, b_I);
      i = b_I.size(0);
      if (0 <= b_I.size(0) - 1) {
        siz_idx_0 = static_cast<short>(same.size(0));
      }
      for (result = 0; result < i; result++) {
        nx = div_s32(b_I[result] - 1, static_cast<int>(siz_idx_0));
        result_idx_1_tmp = (b_I[result] - nx * siz_idx_0) - 1;
        if (same[result_idx_1_tmp + same.size(0) * nx] &&
            (Data_result[(static_cast<int>(static_cast<unsigned int>(
                              MapPointFrame_init[b_i].f1[nx])) +
                          Data_result.size(0) * 4) -
                         1] !=
             Data_result[(static_cast<int>(static_cast<unsigned int>(
                              MapPointFrame[b_i].f1[result_idx_1_tmp])) +
                          Data_result.size(0) * 4) -
                         1])) {
          // if the point is detected both at time i and i+j
          //  Label and count the occurence of the point if not already done
          Data_result[(static_cast<int>(static_cast<unsigned int>(
                           MapPointFrame_init[b_i].f1[nx])) +
                       Data_result.size(0) * 4) -
                      1] =
              Data_result[(static_cast<int>(static_cast<unsigned int>(
                               MapPointFrame[b_i].f1[result_idx_1_tmp])) +
                           Data_result.size(0) * 4) -
                          1];
          //  assign the index of the point
        }
      }
    }
  }
  // create a new cloud
  loop_ub = Data_result.size(0);
  Datax.set_size(Data_result.size(0));
  for (i = 0; i < loop_ub; i++) {
    Datax[i] = Data_result[i + Data_result.size(0) * 4];
  }
  nb_frames = coder::internal::maximum(Datax);
  i = static_cast<int>(nb_frames);
  new_cloud.set_size(i, 4);
  loop_ub = static_cast<int>(nb_frames) << 2;
  for (i1 = 0; i1 < loop_ub; i1++) {
    new_cloud[i1] = 0.0;
  }
  for (result_idx_1_tmp = 0; result_idx_1_tmp < i; result_idx_1_tmp++) {
    // loop over all the molecules
    loop_ub = Data_result.size(0);
    idx.set_size(Data_result.size(0));
    for (i1 = 0; i1 < loop_ub; i1++) {
      idx[i1] = (Data_result[i1 + Data_result.size(0) * 4] ==
                 static_cast<double>(result_idx_1_tmp) + 1.0);
    }
    // detect all the apparition of this molecule
    nx = idx.size(0) - 1;
    result = 0;
    for (b_i = 0; b_i <= nx; b_i++) {
      if (idx[b_i]) {
        result++;
      }
    }
    r.set_size(result);
    partialTrueCount = 0;
    for (b_i = 0; b_i <= nx; b_i++) {
      if (idx[b_i]) {
        r[partialTrueCount] = b_i + 1;
        partialTrueCount++;
      }
    }
    loop_ub = r.size(0);
    Datax.set_size(r.size(0));
    for (i1 = 0; i1 < loop_ub; i1++) {
      Datax[i1] = Data_result[r[i1] - 1];
    }
    new_cloud[result_idx_1_tmp] = std::round(coder::mean(Datax));
    // mean frame
    nx = idx.size(0) - 1;
    result = 0;
    for (b_i = 0; b_i <= nx; b_i++) {
      if (idx[b_i]) {
        result++;
      }
    }
    r1.set_size(result);
    partialTrueCount = 0;
    for (b_i = 0; b_i <= nx; b_i++) {
      if (idx[b_i]) {
        r1[partialTrueCount] = b_i + 1;
        partialTrueCount++;
      }
    }
    loop_ub = r1.size(0);
    Datax.set_size(r1.size(0));
    for (i1 = 0; i1 < loop_ub; i1++) {
      Datax[i1] = Data_result[(r1[i1] + Data_result.size(0)) - 1];
    }
    new_cloud[result_idx_1_tmp + new_cloud.size(0)] = coder::mean(Datax);
    // mean position y
    nx = idx.size(0) - 1;
    result = 0;
    for (b_i = 0; b_i <= nx; b_i++) {
      if (idx[b_i]) {
        result++;
      }
    }
    r3.set_size(result);
    partialTrueCount = 0;
    for (b_i = 0; b_i <= nx; b_i++) {
      if (idx[b_i]) {
        r3[partialTrueCount] = b_i + 1;
        partialTrueCount++;
      }
    }
    loop_ub = r3.size(0);
    Datax.set_size(r3.size(0));
    for (i1 = 0; i1 < loop_ub; i1++) {
      Datax[i1] = Data_result[(r3[i1] + Data_result.size(0) * 2) - 1];
    }
    new_cloud[result_idx_1_tmp + new_cloud.size(0) * 2] = coder::mean(Datax);
    // mean position x
    nx = idx.size(0) - 1;
    result = 0;
    for (b_i = 0; b_i <= nx; b_i++) {
      if (idx[b_i]) {
        result++;
      }
    }
    r6.set_size(result);
    partialTrueCount = 0;
    for (b_i = 0; b_i <= nx; b_i++) {
      if (idx[b_i]) {
        r6[partialTrueCount] = b_i + 1;
        partialTrueCount++;
      }
    }
    loop_ub = r6.size(0);
    Datax.set_size(r6.size(0));
    for (i1 = 0; i1 < loop_ub; i1++) {
      Datax[i1] = Data_result[(r6[i1] + Data_result.size(0) * 3) - 1];
    }
    new_cloud[result_idx_1_tmp + new_cloud.size(0) * 3] = coder::mean(Datax);
    // mean position z

    //Debug
    //new_cloud = Data_result;
  }
}

// End of code generation (function_tracking_new_cloud.cpp)
