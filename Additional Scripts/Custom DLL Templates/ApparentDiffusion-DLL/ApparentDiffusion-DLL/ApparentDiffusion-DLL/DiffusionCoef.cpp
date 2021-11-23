//
// Academic License - for use in teaching, academic research, and meeting
// course requirements at degree granting institutions only.  Not for
// government, commercial, or other organizational use.
//
// DiffusionCoef.cpp
//
// Code generation for function 'DiffusionCoef'
//

// Include files
#include "pch.h"
#include "DiffusionCoef.h"
#include "find.h"
#include "polyfit.h"
#include "coder_array.h"
#include <cmath>

// Function Definitions
double DiffusionCoef(coder::array<double, 2U> &tracks, double f_position,
                     double f_time, double dt, int c)
{
  coder::array<double, 2U> A;
  coder::array<double, 2U> MSD;
  coder::array<double, 2U> b_A;
  coder::array<double, 2U> r;
  coder::array<double, 1U> b_MSD;
  coder::array<double, 1U> ind;
  coder::array<int, 1U> ind_end;
  coder::array<boolean_T, 1U> b_tracks;
  double dv[2];
  double D_coef;
  double a;
  double delta_t_max;
  double dr2;
  int i;
  int i1;
  int i2;
  int loop_ub;
  int nx;
  // function to calculate the apparent diffusion coefficient of
  // single-particle data
  // inputs:
  // (i) tracks : contains the list of localizations of the track of interrset
  //  in tracks the column should be organized as follows X, Y, Z, I, Frame
  //  number
  // (ii) f_position : conversion factor to change the positions to nanometers
  // (ii) f_time : conversion factor to change the format to frame number
  // (ii) dt : time interval between consecutive frames in ms
  // (iii) c : dimensionality factor  2 for 1D, 4 for 2D, 6 for 3D
  // output:
  //  D_coef : diffusion coefficient in um2/s
  loop_ub = tracks.size(0);
  r.set_size(tracks.size(0), 3);
  for (i = 0; i < 3; i++) {
    for (i1 = 0; i1 < loop_ub; i1++) {
      r[i1 + r.size(0) * i] = tracks[i1 + tracks.size(0) * i] / f_position;
    }
  }
  nx = r.size(0) * 3;
  for (loop_ub = 0; loop_ub < nx; loop_ub++) {
    r[loop_ub] = std::round(r[loop_ub]);
  }
  loop_ub = r.size(0);
  for (i = 0; i < 3; i++) {
    for (i1 = 0; i1 < loop_ub; i1++) {
      tracks[i1 + tracks.size(0) * i] = r[i1 + r.size(0) * i];
    }
  }
  loop_ub = tracks.size(0);
  ind.set_size(tracks.size(0));
  for (i = 0; i < loop_ub; i++) {
    ind[i] = tracks[i + tracks.size(0) * 4] / f_time;
  }
  nx = ind.size(0);
  for (loop_ub = 0; loop_ub < nx; loop_ub++) {
    ind[loop_ub] = std::round(ind[loop_ub]);
  }
  loop_ub = ind.size(0);
  for (i = 0; i < loop_ub; i++) {
    tracks[i + tracks.size(0) * 4] = ind[i];
  }
  MSD.set_size(0, 0);
  dr2 = 0.0;
  // starting delta
  delta_t_max = std::floor((tracks[(tracks.size(0) + tracks.size(0) * 4) - 1] -
                            tracks[tracks.size(0) * 4]) -
                           5.0);
  //  to be able to average at least 5
  if (delta_t_max > 1.0) {
    // MSD{i}(:,:) =[];
    A.set_size(0, 2);
    i = static_cast<int>(delta_t_max);
    for (nx = 0; nx < i; nx++) {
      double k;
      k = 0.0;
      i1 = tracks.size(0);
      for (int j{0}; j < i1; j++) {
        //  over all the time points of the track
        delta_t_max =
            tracks[j + tracks.size(0) * 4] + (static_cast<double>(nx) + 1.0);
        if (delta_t_max < tracks[(tracks.size(0) + tracks.size(0) * 4) - 1]) {
          boolean_T y;
          loop_ub = tracks.size(0);
          b_tracks.set_size(tracks.size(0));
          for (i2 = 0; i2 < loop_ub; i2++) {
            b_tracks[i2] = (tracks[i2 + tracks.size(0) * 4] == delta_t_max);
          }
          coder::eml_find(b_tracks, ind_end);
          ind.set_size(ind_end.size(0));
          loop_ub = ind_end.size(0);
          for (i2 = 0; i2 < loop_ub; i2++) {
            ind[i2] = ind_end[i2];
          }
          b_tracks.set_size(ind.size(0));
          loop_ub = ind.size(0);
          for (i2 = 0; i2 < loop_ub; i2++) {
            b_tracks[i2] =
                (tracks[(static_cast<int>(ind[i2]) + tracks.size(0) * 4) - 1] ==
                 delta_t_max);
          }
          y = (b_tracks.size(0) != 0);
          if (y) {
            boolean_T exitg1;
            loop_ub = 0;
            exitg1 = false;
            while ((!exitg1) && (loop_ub <= b_tracks.size(0) - 1)) {
              if (!b_tracks[loop_ub]) {
                y = false;
                exitg1 = true;
              } else {
                loop_ub++;
              }
            }
          }
          if (y) {
            double b_a;
            delta_t_max = tracks[j] - tracks[static_cast<int>(ind[0]) - 1];
            a = tracks[j + tracks.size(0)] -
                tracks[(static_cast<int>(ind[0]) + tracks.size(0)) - 1];
            b_a = tracks[j + tracks.size(0) * 2] -
                  tracks[(static_cast<int>(ind[0]) + tracks.size(0) * 2) - 1];
            dr2 += (delta_t_max * delta_t_max + a * a) + b_a * b_a;
            //
            k++;
          }
        }
      }
      if (k != 0.0) {
        loop_ub = A.size(0);
        b_A.set_size(A.size(0) + 1, 2);
        for (i1 = 0; i1 < 2; i1++) {
          for (i2 = 0; i2 < loop_ub; i2++) {
            b_A[i2 + b_A.size(0) * i1] = A[i2 + A.size(0) * i1];
          }
        }
        b_A[A.size(0)] = static_cast<double>(nx) + 1.0;
        b_A[A.size(0) + b_A.size(0)] = dr2 / k;
        A.set_size(b_A.size(0), 2);
        loop_ub = b_A.size(0) * 2;
        for (i1 = 0; i1 < loop_ub; i1++) {
          A[i1] = b_A[i1];
        }
      }
      // MSD{i}(:,:) = [ MSD{i}(:,:) ;  ]; %all in nm^2
      dr2 = 0.0;
    }
    MSD.set_size(A.size(0), 2);
    loop_ub = A.size(0) * 2;
    for (i = 0; i < loop_ub; i++) {
      MSD[i] = A[i];
    }
    // all in nm^2
  }
  //  Calculate Diffusion coefficient over the first 5xdt
  // fit with linear function between 2dt and 5or7dt (there is a paper speaking
  // about that 1991)
  loop_ub = MSD.size(0);
  b_tracks.set_size(MSD.size(0));
  for (i = 0; i < loop_ub; i++) {
    b_tracks[i] = (MSD[i] == 2.0);
  }
  coder::eml_find(b_tracks, ind_end);
  ind.set_size(ind_end.size(0));
  loop_ub = ind_end.size(0);
  for (i = 0; i < loop_ub; i++) {
    ind[i] = ind_end[i];
  }
  loop_ub = MSD.size(0);
  b_tracks.set_size(MSD.size(0));
  for (i = 0; i < loop_ub; i++) {
    b_tracks[i] = (MSD[i] == 5.0);
  }
  coder::eml_find(b_tracks, ind_end);
  if (static_cast<int>(ind[0]) > ind_end[0]) {
    i = 0;
    i1 = 0;
    i2 = 0;
    nx = 0;
  } else {
    i = static_cast<int>(ind[0]) - 1;
    i1 = ind_end[0];
    i2 = static_cast<int>(ind[0]) - 1;
    nx = ind_end[0];
  }
  // D_coef = 0.0;
  // everything is still in nm^2
  // [a,b]=fit(delta_t,y_MSD,'poly1');
  // Y=a.p1*delta_t+a.p2;
  // calculate Diffusion coefficient from the slope and change unit to um^2/s
  // D_coef=10^-6*a.p1/(c*dt*10^-3);
  loop_ub = i1 - i;
  ind.set_size(loop_ub);
  for (i1 = 0; i1 < loop_ub; i1++) {
    ind[i1] = MSD[i + i1];
  }
  nx -= i2;
  b_MSD.set_size(nx);
  for (i1 = 0; i1 < nx; i1++) {
    b_MSD[i1] = MSD[(i2 + i1) + MSD.size(0)];
  }
  a = std::round(static_cast<double>(c) * dt);
  /*
  if (a < 2.147483648E+9) {
    if (a >= -2.147483648E+9) {
      i1 = static_cast<int>(a);
    } else {
      i1 = MIN_int32_T;
    }
  } else if (a >= 2.147483648E+9) {
    i1 = MAX_int32_T;
  } else {
    i1 = 0;
  }
  */
  coder::polyfit(ind, b_MSD, dv);
  float temp_coeff= (1.0E-6 * dv[0]) / (a * 0.001);
      //std::round(1.0E-6 * dv[0] / std::round(static_cast<double>(i1) * 0.001));
  /*
  if (delta_t_max < 2.147483648E+9) {
    if (delta_t_max >= -2.147483648E+9) {
      i1 = static_cast<int>(delta_t_max);
    } else {
      i1 = MIN_int32_T;
    }
  } else if (delta_t_max >= 2.147483648E+9) {
    i1 = MAX_int32_T;
  } else {
    i1 = 0;
  }
  */
  D_coef = temp_coeff;
  ind.set_size(loop_ub);
  for (i1 = 0; i1 < loop_ub; i1++) {
    ind[i1] = MSD[i + i1];
  }
  b_MSD.set_size(nx);
  for (i = 0; i < nx; i++) {
    b_MSD[i] = MSD[(i2 + i) + MSD.size(0)];
  }
  if (a < 2.147483648E+9) {
    if (a >= -2.147483648E+9) {
      i = static_cast<int>(a);
    } else {
      i = MIN_int32_T;
    }
  } else if (a >= 2.147483648E+9) {
    i = MAX_int32_T;
  } else {
    i = 0;
  }
  coder::polyfit(ind, b_MSD, dv);
  a = std::round(1.0E-6 * dv[0] / std::round(static_cast<double>(i) * 0.001));
  if (a < 2.147483648E+9) {
    if (a >= -2.147483648E+9) {
      i = static_cast<int>(a);
    } else {
      i = MIN_int32_T;
    }
  } else if (a >= 2.147483648E+9) {
    i = MAX_int32_T;
  } else {
    i = 0;
  }
  if (i < 0) {
    D_coef = 0.0;
  }
  return D_coef;
}

// End of code generation (DiffusionCoef.cpp)
