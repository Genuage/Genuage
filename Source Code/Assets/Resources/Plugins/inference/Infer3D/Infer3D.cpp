/*

Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson, Bassam Hajj and Clement Caporal
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. All advertising materials mentioning features or use of this software
   must display the following acknowledgement:
   This product includes software developed by the Institut Curie, Insitut Pasteur and CNRS.
4. Neither the name of the Institut Curie, Insitut Pasteur and CNRS nor the
   names of its contributors may be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

#pragma once

//#include "Infer3D.h"


//#include "IUnityInterface.h"
//#include "optimization.h"
#include <math.h>
#include <iostream>
//#include <stdlib.h>

#define DllExport __declspec(dllexport)

/* Preprocessor Constants */
#define PI 3.1415926535897932384626433832795028841971693993751
#define STPMX 500.0
#define ITMAX 10000
#define EPS 3.e-11
#define TOLY (4.0*EPS)
#define NTAB 10
#define BIG 1.0e30
#define CON 1.4
#define CON2 (CON*CON)
#define SAFE 2.0
#define TOLX 1.0e-8
#define ALF 1.0e-4
#define GTOL 1.0e-16

static double maxarg1,maxarg2;
#define FMAX(a,b) ( maxarg1 = (a), maxarg2 = (b) , (maxarg1) > (maxarg2) ? \
(maxarg1) : (maxarg2) )

static double sqrarg;
#define SQR(a) ((sqrarg = (a)) == 0.0 ? 0.0 : sqrarg*sqrarg)

/* Global Variables */
/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


int N = 0;
double *X;
double *Y;
double *Z;
double *T;
double *Tr;
/*xxx we put here localisation error as a global variables, it may be moved depending 
 on where the error of localisation is accessed, at the moment let it be a fixed 
variable, we may allow it to moive with point */ 
double Sigma     = 0.0; // microns global noise if all the same in xyz
double Sigma_x_y = 0.0; // microns if assymetric noise x and y 
double Sigma_z   = 0.0;// mirons assymetric noise in z
int    DIM;
static FILE *debugFile;


enum HeaderID {
	DIFFUSION = 0,
	DIFFUSION_VELOCITY = 1,
};

enum ParamID {
	LIKELYHOOD_NO_NOISE = 0,
	LIKELYHOOD_UNIFORM_NOISE = 1,
	LIKELYHOOD_ASYMETRIC_NOISE = 2,
	LIKELYHOOD_APPROX_NOISE_ONLY_Z = 3,
	POSTERIOR_NO_NOISE_NON_INFORMATIVE_PRIOR = 4,
	POSTERIOR_NO_NOISE_CONGUGATEPRIOR = 5,
	POSTERIOR_UNIFORM_NOISE_NON_INFORMATIVE_PRIOR = 6,
	POSTERIOR_ASYMETRIC_NOISE_NON_INFORMATIVE = 7,
	POSTERIOR_APPROX_NOISE_ONLY_Z_NON_INFORMATIVE = 8,
};

/*
enum FunctionID {
	DFXFYFZPosterior = 0,
	D_LIKELYHOOD_NO_NOISE = 1,
	DPOSTERIOR_NO_NOISE_NON_INFORMATIVE_PRIOR = 2,
	D_POSTERIOR_NO_NOISE_CONGUGATEPRIOR = 3,
	DLIKELYHOOD_UNIFORM_NOISE = 4,
	DPOSTERIOR_UNIFORM_NOISE_NON_INFORMATIVE_PRIOR = 5,
	DLIKELYHOOD_ASYMETRIC_NOISE = 6,
	DPOSTERIOR_ASYMETRIC_NOISE_NON_INFORMATIVE = 7,
	DLIKELYHOOD_APPROX_NOISE_ONLY_Z = 8,
	DPOSTERIOR_APPROX_NOISE_ONLY_Z_NON_INFORMATIVE = 9,
	DVLIKELYHOOD_NO_NOISE = 10,
	DVPOSTERIOR_NO_NOISE_NON_INFORMATIVE = 11,
	DVPOSTERIOR_NO_NOISE_CONJUGATE_PRIOR = 12,
	DVLIKELYHOOD_UNIFORM_NOISE = 13,
	DVPOSTERIOR_UNIFORM_NOISE_NON_INFORMATIVE = 14,
	DVLIKELYHOOD_ASYMETRIC_NOISE = 15,
	DVPOSTERIOR_ASYMETRIC_NOISE_NON_INFORMATIVE = 16,
	DVLIKELYHOOD_APPROX_NOISE_ONLY_Z = 17,
	DVPOSTERIOR_APPROX_NOISE_ONLY_Z_NON_INfORMATIVE = 18,

};
*/

/* Optimization Functions */

void lnsrch(int ndim, double xold[], double fold, double g[], double p[], double xx[], double f[], double stpmax, int check[], double (func)(double[])) {

	int i;
	double a, alam, alam2, alamin, b, disc, f2, rhs1, rhs2, slope, sum, temp, test, tmplam;

	check[0] = 0;
	for (sum = 0.0, i = 0; i < ndim; i++) { sum += p[i] * p[i]; }
	sum = sqrt(sum);

	if (sum > stpmax)
		for (i = 0; i < ndim; i++) p[i] *= stpmax / sum;
	for (slope = 0.0, i = 0; i < ndim; i++) {
		slope += g[i] * p[i];
	}

	if (slope >= 0.0) {
		return;
		exit(1);
	}
	test = 0.0;
	for (i = 0; i < ndim; i++) {
		temp = fabs(p[i]) / FMAX(fabs(xold[i]), 1.0);
		if (temp > test) test = temp;
	}
	alamin = TOLX / test;
	alam = 1.0;
	for (;;) {
		for (i = 0; i < ndim; i++) xx[i] = xold[i] + alam * p[i];

		f[0] = (func)(xx);


		if (alam < alamin) {
			for (i = 0; i < ndim; i++) { xx[i] = xold[i]; }
			check[0] = 1;
			return;
		}
		else if (f[0] <= fold + ALF * alam * slope) {
			return;
		}
		else {
			if (alam == 1.0)
				tmplam = -slope / (2.0 * (f[0] - fold - slope));
			else {
				rhs1 = f[0] - fold - alam * slope;
				rhs2 = f2 - fold - alam2 * slope;
				a = (rhs1 / (alam * alam) - rhs2 / (alam2 * alam2)) / (alam - alam2);
				b = (-alam2 * rhs1 / (alam * alam) + alam * rhs2 / (alam2 * alam2)) / (alam - alam2);
				if (a == 0.0) { tmplam = -slope / (2.0 * b); }
				else {
					disc = b * b - 3.0 * a * slope;
					if (disc < 0.0) { tmplam = 0.5 * alam; }
					else if (b <= 0.0) { tmplam = (-b + sqrt(disc)) / (3.0 * a); }
					else { tmplam = -slope / (b + sqrt(disc)); }
				}

				if (tmplam > 0.5 * alam)
					tmplam = 0.5 * alam;
			}
		}
		alam2 = alam;
		f2 = f[0];
		alam = FMAX(tmplam, 0.1 * alam);
	}

}
void dfpmin(double* p, int ndim, double gtol, int* iter, double* fret, double (func)(double*), void (dfunc)(double (func)(double*), double*, double*)) {

	int check, i, ii, its, j;
	double den, fac, fad, fae, fp, stpmax, sum = 0.0, sumdg, sumxi, temp, test;
	double* dg = new double[ndim];
	double* g = new double[ndim];
	double* hdg = new double[ndim];
	double* pnew = new double[ndim];
	double* xi = new double[ndim];

	double** hessin = new double* [ndim];
	for (int rr = 0; rr < ndim; rr++) {
		hessin[rr] = new double[ndim];
	}

	for (ii = 0; ii < ndim; ii++) {
		dg[ii] = 0.0;
		g[ii] = 0.0;
		hdg[ii] = 0.0;
		pnew[ii] = 0.0;
		xi[ii] = 0.0;
	}

	fp = (func)(p);

	(dfunc)((func), p, g);

	for (i = 0; i < ndim; i++) {
		for (j = 0; j < ndim; j++) {
			hessin[i][j] = 0.0;
		}
		hessin[i][i] = 1.0;
		xi[i] = -g[i];
		sum += p[i] * p[i];
	}

	stpmax = STPMX * FMAX(sqrt(sum), (double)ndim);

	for (its = 0; its < ITMAX; its++) {
		iter[0] = its;

		//fprintf(debugFile, "Iteration %i\n", iter[0] + 1);

		lnsrch(ndim, p, fp, g, xi, pnew, fret, stpmax, &check, (func));
		fp = fret[0];

		for (i = 0; i < ndim; i++) {
			xi[i] = pnew[i] - p[i];
			p[i] = pnew[i];
		}
		test = 0.0;
		for (i = 0; i < ndim; i++) {
			temp = fabs(xi[i]) / FMAX(fabs(p[i]), 1.0);
			if (temp > test) { test = temp; }
		}
		if (test < TOLY) {
			delete[] dg;
			delete[] g;
			delete[] hdg;
			delete[] pnew;
			delete[] xi;
			for (int rr = 0; rr < ndim; rr++) {
				delete[] hessin[rr];
			}
			delete[] hessin;

			return;
		}

		for (i = 0; i < ndim; i++) { dg[i] = g[i]; }

		(dfunc)((func), p, g);

		test = 0.0;
		den = FMAX(fret[0], 1.0);
		for (i = 0; i < ndim; i++) {
			temp = fabs(g[i]) * FMAX(fabs(p[i]), 1.0) / den;
			if (temp > test) { test = temp; }
		}

		if (test < gtol) {
			delete[] dg;
			delete[] g;
			delete[] hdg;
			delete[] pnew;
			delete[] xi;
			for (int rr = 0; rr < ndim; rr++) {
				delete[] hessin[rr];
			}
			delete[] hessin;
			return;
		}

		for (i = 0; i < ndim; i++) { dg[i] = g[i] - dg[i]; }
		for (i = 0; i < ndim; i++) {
			hdg[i] = 0.0;
			for (j = 0; j < ndim; j++) { hdg[i] += hessin[i][j] * dg[j]; }
		}
		fac = fae = sumdg = sumxi = 0.0;
		for (i = 0; i < ndim; i++) {
			fac += dg[i] * xi[i];
			fae += dg[i] * hdg[i];
			sumdg += SQR(dg[i]);
			sumxi += SQR(xi[i]);
		}

		if (fac > sqrt(EPS * sumdg * sumxi)) {
			fac = 1.0 / fac;
			fad = 1.0 / fae;
			for (i = 0; i < ndim; i++) { dg[i] = fac * xi[i] - fad * hdg[i]; }
			for (i = 0; i < ndim; i++) {
				for (j = i; j < ndim; j++) {
					hessin[i][j] += fac * xi[i] * xi[j]
						- fad * hdg[i] * hdg[j] + fae * dg[i] * dg[j];
					hessin[j][i] = hessin[i][j];
				}
			}
		}
		for (i = 0; i < ndim; i++) {
			xi[i] = 0.0;
			for (j = 0; j < ndim; j++) xi[i] -= hessin[i][j] * g[j];
		}
	}

	delete[] dg;
	delete[] g;
	delete[] hdg;
	delete[] pnew;
	delete[] xi;
	for (int rr = 0; rr < ndim; rr++) {
		delete[] hessin[rr];
	}
	delete[] hessin;

	//fprintf(debugFile, "dfpmin fail\n"); return; exit(-1);
}

void dfunc(double (func)(double*), double* yy, double* ans) {

	double h = 1.e-8, ERR;
	int i, j, indice, indice2;
	double errt, fac, hh;

	int ndim = DIM;

	double* veclocal1 = new double[ndim];
	double* veclocal2 = new double[ndim];

	double a[NTAB][NTAB];

	for (int aa = 0; aa < NTAB; aa++) {
		for (int bb = 0; bb < NTAB; bb++) {
			a[aa][bb] = 0.0;
		}
	}

	for (indice = 0; indice < ndim; indice++) {

		hh = h;
		for (indice2 = 0; indice2 < ndim; indice2++) {
			if (indice2 == indice) {
				veclocal1[indice2] = yy[indice2] + hh;
				veclocal2[indice2] = yy[indice2] - hh;
			}
			else {
				veclocal1[indice2] = yy[indice2];
				veclocal2[indice2] = yy[indice2];
			}
		}

		a[0][0] = ((func)(veclocal1)-(func)(veclocal2)) / (2.0 * hh);

		ERR = BIG;

		for (i = 1; i < NTAB; i++) {
			hh /= CON;

			for (indice2 = 0; indice2 < ndim; indice2++) {
				if (indice2 == indice) {
					veclocal1[indice2] = yy[indice2] + hh;
					veclocal2[indice2] = yy[indice2] - hh;
				}
				else {
					veclocal1[indice2] = yy[indice2];
					veclocal2[indice2] = yy[indice2];
				}
			}

			a[0][i] = ((func)(veclocal1)-(func)(veclocal2)) / (2.0 * hh);

			fac = CON2;
			for (j = 1; j < i; j++) {
				a[j][i] = (a[j - 1][i] * fac - a[j - 1][i - 1]) / (fac - 1.0);
				fac = CON2 * fac;
				errt = FMAX(fabs(a[j][i] - a[j - 1][i]), fabs(a[j][i] - a[j - 1][i - 1]));
				if (errt <= ERR) {
					ERR = errt;
					ans[indice] = a[j][i];
				}
			}
			if (fabs(a[i][i] - a[i - 1][i - 1]) >= SAFE * (ERR)) break;
		}

	}

	delete[] veclocal1;
	delete[] veclocal2;

	return;
}

/************************************************************************************/
/***************************    utilities    ****************************************/
/************************************************************************************/
void give_mean_variance_displacement(double* dx_mean, double*  dy_mean, double* dz_mean, double* Var){

	//double  dt;
	//double Var;
	int indice;

	double res     = 0.0 ;
	*dx_mean = 0.0 ;
	*dy_mean = 0.0 ;
	*dz_mean = 0.0 ;
	indice  = 0   ;

	for (int i = 0; i < N-1; i++) {

		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];

			*dx_mean +=  dx;
			*dy_mean +=  dy;
			*dz_mean +=  dx;
			indice  +=  1;

		}
	}

	*dx_mean /= indice * 1.;
	*dy_mean /= indice * 1.;
	*dz_mean /= indice * 1.;

	indice = 0;
	*Var    = 0.;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];

			*Var += pow(dx - *dx_mean, 2.0) + pow(dy - *dy_mean, 2.0) + pow(dz - *dz_mean, 2.0);
			indice  +=  1;

		}
	}

	*Var /= indice * 1.0;


}



double DFxFyFzPosterior(double* DFxFyFz) {

	double res, dt;

	res = 0.0;

	for (int i = 0; i < N - 1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i + 1] - T[i]);

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];
			const double D_bruit = Sigma * Sigma / dt;

			res += -log(4.0 * PI * (DFxFyFz[0] + D_bruit) * dt)
				- pow(fabs(dx - DFxFyFz[0] * DFxFyFz[1] * dt), 2.0) / (4.0 * (DFxFyFz[0] + D_bruit) * dt)
				- pow(fabs(dy - DFxFyFz[0] * DFxFyFz[2] * dt), 2.0) / (4.0 * (DFxFyFz[0] + D_bruit) * dt)
				- pow(fabs(dx - DFxFyFz[0] * DFxFyFz[3] * dt), 2.0) / (4.0 * (DFxFyFz[0] + D_bruit) * dt);
		}
	}
	res += 2.0 * log(DFxFyFz[0]) - 2.0 * log(DFxFyFz[0] * dt + Sigma * Sigma);

	return -res;
}

/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double DPosterior(double* D) {

	double result = 0.0;
	double dt;

	for (int i = 0; i < N - 1; i++) {

		// check that index is not at end of list
		// check that deltas between data points in same file
		if (Tr[i] == Tr[i + 1]) {

			dt = fabs(T[i + 1] - T[i]);
			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];
			const double D_bruit = Sigma * Sigma / dt;

			result += -log(4.0 * PI * (D[0] + D_bruit) * dt) - (dx * dx) / (4.0 * (D[0] + D_bruit) * dt) - (dy * dy) / (4.0 * (D[0] + D_bruit) * dt - (dz * dz) / (4.0 * (D[0] + D_bruit) * dt));
		}
	}

	result += -log(D[0] * dt + Sigma * Sigma);

	return -result;
}

/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/***************************   pure diffusion    ****************************************/
/************************************************************************************/
double Dlikelihood_no_noise(double *D){

	double res, dt;
	res = 0.0;


	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			dt = fabs(T[i+1] - T[i]);

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];


			res += -3. / 2. * log(4.0 * PI * (D[0]) * dt); // 3D en 3/2
			res += -pow(fabs(dx), 2.0) / (4.0 * (D[0]) * dt);
			res += -pow(fabs(dy), 2.0) / (4.0 * (D[0]) * dt);
			res += - pow(fabs(dz ), 2.0) / (4.0*(D[0] )*dt);
		}
	}



	return -res;


}

/************************************************************************************/
/*********************************    ***************************************/
/************************************************************************************/
double Dposterior_no_noise_non_informative_prior(double* D){

	double res, dt;
	res = 0.0;


	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			dt = fabs(T[i+1] - T[i]);

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];


			res += -3. / 2. * log(4.0 * PI * (D[0]) * dt); // 3D en 3/2
			res += -pow(fabs(dx), 2.0) / (4.0 * (D[0]) * dt);
			res += -pow(fabs(dy), 2.0) / (4.0 * (D[0]) * dt);
			res += - pow(fabs(dz ), 2.0) / (4.0*(D[0] ) * dt);
		}
	}

	res += - log(*D);


	return -res;


}

/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dposterior_no_noise_conjugate_prior(double *D){

	double res, dt;
	double a, b, b2;
	double d;
	int    n_pi;
	double V_pi;
	double ratio_var;
	int    indice;
	double dt_mean;
	double dx_mean, dy_mean, dz_mean, Var;

	// better to have generated it before we calculate twice
	give_mean_variance_displacement(&dx_mean, &dy_mean, &dz_mean, &Var);

// hyperparameters
	d         = 3.0;
	n_pi      = 4;
	ratio_var = 1.0;
	V_pi      = ratio_var* Var;
	b         = sqrt( 2.0 * D[0] );
	b2        = pow(b, 2.0);
// 	
	dt_mean = 0.;
	indice  = 0;
	res       = 0.0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			dt = fabs(T[i+1] - T[i]);

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];


			res += -3. / 2. * log(4.0 * PI * (D[0]) * dt); // 3D en 3/2
			res += -pow(fabs(dx), 2.0) / (4.0 * (D[0]) * dt);
			res += -pow(fabs(dy), 2.0) / (4.0 * (D[0]) * dt);
			res += - pow(fabs(dz ), 2.0) / (4.0*( D[0] ) * dt);
			dt_mean += dt;
			indice +=1;

		}
	}

	dt_mean /= indice;


	res += d*(1.0 - n_pi)/2.0 * log(b2) - n_pi*V_pi*1.0/(2.0 * b2 * dt_mean);


	return -res;


}

/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


double Dlikelihood_uniform_noise(double *D){

	double res, dt;
	res = 0.0;
	//indice = 0


	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];
			const double D_bruit = Sigma * Sigma / dt;


			res += -3.0 / 2.0 * log(4.0 * PI * (D[0] + D_bruit) * dt);
			res += -pow(fabs(dx), 2.0) / (4.0 * (D[0] + D_bruit) * dt);
			res += -pow(fabs(dy), 2.0) / (4.0 * (D[0] + D_bruit) * dt);
			res += - pow(fabs(dz ), 2.0)  / (4.0*(D[0] + D_bruit)*dt);

		}
	}


	return -res;


}


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dposterior_uniform_noise_non_informative_prior(double *D){
	double res, dt;
	double dt_mean;
	int indice;
	res = 0.0;
	indice = 0;
	dt_mean = 0.0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];
			const double D_bruit = Sigma * Sigma / dt;


			res += -3.0 / 2.0 * log(4.0 * PI * (D[0] + D_bruit) * dt);
			res += -pow(fabs(dx), 2.0) / (4.0 * (D[0] + D_bruit) * dt);
			res     += - pow(fabs(dy ), 2.0)  / (4.0*(D[0] + D_bruit)*dt);
			res     += - pow(fabs(dz ), 2.0)  / (4.0*(D[0] + D_bruit)*dt);
			indice  += 1;
			dt_mean += dt;
		}
	}

	dt_mean /= (indice)*1.;

	res  -= 1.*log(D[0]*dt_mean + Sigma * Sigma);

	return -res;

}


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dlikelihood_asymetric_noise(double *D){

	double res, dt;
	res = 0.0;
	//indice = 0


	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];
			const double D_bruit_x_y = Sigma_x_y * Sigma_x_y / dt;
			const double D_bruit_z   = Sigma_z   * Sigma_z / dt;



			res += -2.0 / 2.0 * log(4.0 * PI * (D[0] + D_bruit_x_y) * dt);
			res += -1.0 / 2.0 * log(4.0 * PI * (D[0] + D_bruit_z) * dt);
			res += -pow(fabs(dx), 2.0) / (4.0 * (D[0] + D_bruit_x_y) * dt);
			res += - pow(fabs(dy ), 2.0)  / (4.0*(D[0] + D_bruit_x_y)*dt);
			res += - pow(fabs(dz ), 2.0)  / (4.0*(D[0] + D_bruit_z)*dt);

		}
	}


	return -res;


}


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dposterior_asymetric_noise_non_informative(double *D){

	double res, dt;
	double num, den; 
	double dt_mean;
	int indice;
	res = 0.0;
	indice = 0;


	dt_mean = 0.;
	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];
			const double D_bruit_x_y = Sigma_x_y * Sigma_x_y / dt;
			const double D_bruit_z   = Sigma_z   * Sigma_z / dt;



			res += -2.0 / 2.0 * log(4.0 * PI * (D[0] + D_bruit_x_y) * dt);
			res += -1.0 / 2.0 * log(4.0 * PI * (D[0] + D_bruit_z) * dt);
			res += -pow(fabs(dx), 2.0) / (4.0 * (D[0] + D_bruit_x_y) * dt);
			res += - pow(fabs(dy ), 2.0)  / (4.0*(D[0] + D_bruit_x_y)*dt);
			res += - pow(fabs(dz ), 2.0)  / (4.0*(D[0] + D_bruit_z)*dt);
			indice  += 1;
			dt_mean += dt;
		}
	}
	dt_mean /= indice *1.0;


	num   = (2.0*D[0] * dt_mean + Sigma_x_y * Sigma_x_y + 3.0*pow(D[0],2.0) * pow(dt_mean,2.0) + 2.0 * pow(Sigma_z,4.0) + pow(Sigma_x_y, 4.0) +4.0*D[0]*dt_mean*pow(Sigma_z,2.0) ); 
	den   = pow(D[0]  * dt_mean + Sigma_x_y * Sigma_x_y,2.0)* pow(D[0] * dt_mean + Sigma_z * Sigma_z,2.0) ;

	res += 1.0/2.0*( log(num) - log(den) );

	//1/2 sqrt(2) sqrt((dt^2 (2 D dt Sigma^2+3 D^2 dt^2+2 eta^4+Sigma^4+4 D dt eta^2))/


	return -res;


}
/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dlikelihood_approx_noise_only_z(double *D){

	double res, dt;
	res = 0.0;
	//indice = 0


	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];
			const double D_bruit_z   = Sigma_z   * Sigma_z / dt;



			res += -2.0 / 2.0 * log(4.0 * PI * (D[0]) * dt);
			res += -1.0 / 2.0 * log(4.0 * PI * (D[0] + D_bruit_z) * dt);
			res += -pow(fabs(dx), 2.0) / (4.0 * (D[0]) * dt);
			res += - pow(fabs(dy ), 2.0)  / (4.0*(D[0] )*dt);
			res += - pow(fabs(dz ), 2.0)  / (4.0*(D[0] + D_bruit_z)*dt);

		}
	}


	return -res;


}



/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dposterior_approx_noise_only_z_non_informative(double *D){

	double res, dt;
	double num, den; 
	double dt_mean ;
	int indice;
	res = 0.0;
	indice = 0;


	dt_mean = 0.0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx = X[i + 1] - X[i];
			const double dy = Y[i + 1] - Y[i];
			const double dz = Z[i + 1] - Z[i];
			const double D_bruit_z   = Sigma_z   * Sigma_z / dt;



			res += -2.0 / 2.0 * log(4.0 * PI * (D[0]) * dt);
			res += -1.0 / 2.0 * log(4.0 * PI * (D[0] + D_bruit_z) * dt);
			res += -pow(fabs(dx), 2.0) / (4.0 * (D[0]) * dt);
			res += - pow(fabs(dy ), 2.0)  / (4.0*(D[0] )*dt);
			res += - pow(fabs(dz ), 2.0)  / (4.0*(D[0] + D_bruit_z)*dt);
			indice  += 1;
			dt_mean += dt;
		}
	}
	dt_mean /= indice *1.0;



	num   = ( 3.0*pow(D[0],2.0)*pow(dt_mean, 2.0) + 4.0*D[0]*pow(Sigma_z,2.0) + 2.0 * pow(Sigma_z, 4.0) ); 
	den   = pow(D[0] ,2.0)* pow(D[0] * dt_mean + Sigma_z * Sigma_z,2.0) ;


	res += 1.0/2.0*( log(num) - log(den) );


	return -res;


}


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/



/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/**************************diffusion velocity **********************************/
/************************************************************************************/

double Dvlikelihood_no_noise(double *Dv) {

	double res, dt;
	res = 0.0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx          = X[i + 1] - X[i];
			const double dy          = Y[i + 1] - Y[i];
			const double dz          = Z[i + 1] - Z[i];
	//		const double D_bruit_x_y = Sigma_x_y * Sigma_x_y / dt;
	//		const double D_bruit_z   = Sigma_z * Sigma_z / dt;

			res += -3.0/2.0*log(4.0*PI*(Dv[0] )*dt);
			res +=  - pow(fabs(dx -  Dv[1] * dt), 2.0) / (4.0*(Dv[0])*dt);
			res +=  - pow(fabs(dy -  Dv[2] * dt), 2.0) / (4.0*(Dv[0])*dt);
			res +=  - pow(fabs(dz -  Dv[3] * dt), 2.0) / (4.0*(Dv[0])*dt);
				
		}
	}



	return -res;
}
/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dvposterior_no_noise_non_informative(double *Dv) {

	double res, dt;
	double dt_mean;
	int    indice ;

	indice  = 0;
	dt_mean = 0.0;
	res     = 0.0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx          = X[i + 1] - X[i];
			const double dy          = Y[i + 1] - Y[i];
			const double dz          = Z[i + 1] - Z[i];

			res     +=  - 3.0/2.0 * log( 4.0 * PI * ( Dv[0] ) * dt);
			res     +=  - pow(fabs(dx -  Dv[1] * dt), 2.0) / (4.0*(Dv[0])*dt);
			res     +=  - pow(fabs(dy -  Dv[2] * dt), 2.0) / (4.0*(Dv[0])*dt);
			res     +=  - pow(fabs(dz -  Dv[3] * dt), 2.0) / (4.0*(Dv[0])*dt);
			indice  +=  1 ;
			dt_mean += dt;

		}
	}

	dt_mean /= indice * 1.0;
	res     += -5.0/2.0 * log(Dv[0]);

	//II := (1/4)*sqrt(3)*sqrt(dt^8/(D*dt+Sigma^2)^5)



	return -res;
}
/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dvposterior_no_noise_conjugate_prior(double *Dv) {

	double res, dt;
	double a, b, b2;
	double d;
	int    n_pi;
	double V_pi;
	double ratio_var;
	int    indice;
	double dt_mean;
	double dx_mean, dy_mean, dz_mean, Var;

		// better to have generated it before we calculate twice
	give_mean_variance_displacement(&dx_mean, &dy_mean, &dz_mean, &Var);

// hyperparameters
	d         = 3.0;
	n_pi      = 4;
	ratio_var = 1.0;
	V_pi      = ratio_var* Var;
	b         = sqrt( 2.0 * Dv[0] );
	b2        = pow(b, 2.0);
//

	indice  = 0;
	dt_mean = 0.0;
	res     = 0.0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx          = X[i + 1] - X[i];
			const double dy          = Y[i + 1] - Y[i];
			const double dz          = Z[i + 1] - Z[i];

			res     +=  - 3.0/2.0 * log( 4.0 * PI * ( Dv[0] ) * dt);
			res     +=  - pow(fabs(dx -  Dv[1] * dt), 2.0) / (4.0*(Dv[0])*dt);
			res     +=  - pow(fabs(dy -  Dv[2] * dt), 2.0) / (4.0*(Dv[0])*dt);
			res     +=  - pow(fabs(dz -  Dv[3] * dt), 2.0) / (4.0*(Dv[0])*dt);
			indice  +=  1 ;
			dt_mean += dt;

		}
	}

	dt_mean /= indice * 1.0;


	res += -1.0 * d * (n_pi) / 2.0 * log(b2);
	res += -(n_pi * ( pow( Dv[1],2.0) + pow( Dv[2],2.0) + pow( Dv[3],2.0)) + n_pi*V_pi )/(2.0 * b2 *dt_mean);

	//II := (1/4)*sqrt(3)*sqrt(dt^8/(D*dt+Sigma^2)^5)



	return -res;
}
/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dvlikelihood_uniform_noise(double *Dv) {

	double res, dt;
	res = 0.0;
	double dt_mean;
	int indice ;

	indice  = 0;
	dt_mean = 0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx          = X[i + 1] - X[i];
			const double dy          = Y[i + 1] - Y[i];
			const double dz          = Z[i + 1] - Z[i];
	    	const double D_bruit     = Sigma * Sigma / dt;

			res += -3.0/2.0 * log(4.0*PI*(Dv[0] +  D_bruit )*dt);
			res +=  - pow(fabs(dx -  Dv[1] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit)*dt);
			res +=  - pow(fabs(dy -  Dv[2] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit)*dt);
			res +=  - pow(fabs(dz -  Dv[3] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit)*dt);
				
		}
	}



	return -res;
}




/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dvposterior_uniform_noise_non_informative(double *Dv) {

	double res, dt;
	res = 0.0;
	double dt_mean;
	int indice ;

	indice  = 0;
	dt_mean = 0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx          = X[i + 1] - X[i];
			const double dy          = Y[i + 1] - Y[i];
			const double dz          = Z[i + 1] - Z[i];
	    	const double D_bruit     = Sigma * Sigma / dt;

			res += -3.0/2.0 * log(4.0*PI*(Dv[0] +  D_bruit )*dt);
			res +=  - pow(fabs(dx -  Dv[1] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit)*dt);
			res +=  - pow(fabs(dy -  Dv[2] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit)*dt);
			res +=  - pow(fabs(dz -  Dv[3] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit)*dt);
			dt_mean += dt;
			indice += 1;
		}
	}
	dt_mean /= indice*1.0;
	// proper prior
	res += log(3.0 * pow(Sigma,3.0)*dt_mean*1.0/2.0 ) - 5.0/2.0 *(log( Dv[0] *dt_mean + Sigma*Sigma ) ); 


	return -res;
}

/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dvlikelihood_asymetric_noise(double *Dv) {

	double res, dt;
	res = 0.0;
	double dt_mean;
	int indice ;

	indice  = 0;
	dt_mean = 0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx          = X[i + 1] - X[i];
			const double dy          = Y[i + 1] - Y[i];
			const double dz          = Z[i + 1] - Z[i];
	 		const double D_bruit_x_y = Sigma_x_y * Sigma_x_y / dt;
			const double D_bruit_z   = Sigma_z * Sigma_z / dt;


			res += -2.0/2.0 * log(4.0*PI*(Dv[0] +  D_bruit_x_y )*dt);
			res += - 1.0/2.0 * log(4.0*PI*(Dv[0] + D_bruit_z  )*dt);
			res +=  - pow(fabs(dx -  Dv[1] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit_x_y)*dt);
			res +=  - pow(fabs(dy -  Dv[2] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit_x_y)*dt);
			res +=  - pow(fabs(dz -  Dv[3] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit_z  )*dt);
				
		}
	}



	return -res;
}




/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dvposterior_asymetric_noise_non_informative(double *Dv) {

	double res, dt;
	res = 0.0;
	double dt_mean;
	int indice ;
	double num, den;

	indice  = 0;
	dt_mean = 0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx          = X[i + 1] - X[i];
			const double dy          = Y[i + 1] - Y[i];
			const double dz          = Z[i + 1] - Z[i];
	 		const double D_bruit_x_y = Sigma_x_y * Sigma_x_y / dt;
			const double D_bruit_z   = Sigma_z * Sigma_z / dt;

			res     += -2.0/2.0 * log(4.0*PI*(Dv[0] +  D_bruit_x_y )*dt);
			res     += - 1.0/2.0 * log(4.0*PI*(Dv[0] + D_bruit_z  )*dt);
			res     +=  - pow(fabs(dx -  Dv[1] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit_x_y)*dt);
			res     +=  - pow(fabs(dy -  Dv[2] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit_x_y)*dt);
			res     +=  - pow(fabs(dz -  Dv[3] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit_z  )*dt);
			dt_mean += dt;
			indice  += 1;
		}
	}
	dt_mean /= indice*1.0;

	num = 2.0* pow(Sigma_z, 4.0) + pow(Sigma_x_y, 4.0) + 3.0 * pow(Dv[0], 2.0)* pow(dt_mean, 2.0) + 4.0 *Dv[0]*dt_mean*pow(Sigma_z,2.0) + 2.0 *Dv[0]* dt_mean + pow(Sigma_x_y,2.0);
	den = pow(Dv[0]*dt_mean + Sigma_z* Sigma_z,3.0)* pow(Dv[0]*dt_mean + Sigma_x_y* Sigma_x_y,4.0) ;

	// proper prior
	res += 1.0/2.0*( log(num) - log(den) );

	


	return -res;
}

/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dvlikelihood_approx_noise_only_z(double *Dv) {

	double res, dt;
	res = 0.0;
	double dt_mean;
	int indice ;

	indice  = 0;
	dt_mean = 0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx          = X[i + 1] - X[i];
			const double dy          = Y[i + 1] - Y[i];
			const double dz          = Z[i + 1] - Z[i];
			const double D_bruit_z   = Sigma_z * Sigma_z / dt;


			res += - 2.0/2.0 * log(4.0*PI*(Dv[0]  )*dt);
			res += - 1.0/2.0 * log(4.0*PI*(Dv[0] + D_bruit_z  )*dt);
			res +=  - pow(fabs(dx -  Dv[1] * dt), 2.0) / (4.0*(Dv[0])*dt);
			res +=  - pow(fabs(dy -  Dv[2] * dt), 2.0) / (4.0*(Dv[0])*dt);
			res +=  - pow(fabs(dz -  Dv[3] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit_z  )*dt);
				
		}
	}



	return -res;
}




/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

double Dvposterior_approx_noise_only_z_non_informative(double *Dv) {

	double res, dt;
	res = 0.0;
	double dt_mean;
	int indice ;
	double num, den;

	indice  = 0;
	dt_mean = 0;

	for (int i = 0; i < N-1; i++) {
		if (Tr[i] == Tr[i + 1]) {
			// "next" index does not necessarily lie in the "current zone"
			dt = fabs(T[i+1] - T[i]);

			const double dx          = X[i + 1] - X[i];
			const double dy          = Y[i + 1] - Y[i];
			const double dz          = Z[i + 1] - Z[i];
			const double D_bruit_z   = Sigma_z * Sigma_z / dt;

			res     +=  - 2.0/2.0 * log(4.0*PI*(Dv[0]  )*dt);
			res     +=  - 1.0/2.0 * log(4.0*PI*(Dv[0] + D_bruit_z  )*dt);
			res     +=  - pow(fabs(dx -  Dv[1] * dt), 2.0) / (4.0*(Dv[0])*dt);
			res     +=  - pow(fabs(dy -  Dv[2] * dt), 2.0) / (4.0*(Dv[0])*dt);
			res     +=  - pow(fabs(dz -  Dv[3] * dt), 2.0) / (4.0*(Dv[0]+  D_bruit_z  )*dt);
			dt_mean += dt;
			indice  += 1;
		}
	}

	dt_mean /= indice*1.0;

	num = 3.0*pow(Dv[0], 2.0) * pow(dt_mean,2.0) + 4.0 * Dv[0] * dt_mean * pow(Sigma_z,2.0) + 2.0 * pow(Sigma_z, 4.0);
	den =  pow(Dv[0]*dt_mean + Sigma_z*Sigma_z, 3.0) *pow(Dv[0], 4.0) ;

	// proper prior
	res += 1.0/2.0*( log(num) - log(den) );



	return -res;
}


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/

/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


/************************************************************************************/
/************************************************************************************/
/************************************************************************************/




/************************************************************************************/
/************************************************************************************/
/************************************************************************************/




/************************************************************************************/
/************************************************************************************/
/************************************************************************************/


extern "C" DllExport void Infer3D(int headerID, int paramID, double sigma, double sigmaxy, double sigmaz, int NumberOfPoints, void* TrajectoryNumber, void* xCoordinates, void* yCoordinates, void* zCoordinates, void* TimeStamp, double* Diffusion, double* ForceX, double* ForceY, double* ForceZ) {
	Tr = (double*)TrajectoryNumber;
	X = (double*)xCoordinates;
	Y = (double*)yCoordinates;
	Z = (double*)zCoordinates;
	T = (double*)TimeStamp;
	N = NumberOfPoints;
	Sigma = sigma;
	Sigma_x_y = sigmaxy;
	Sigma_z = sigmaz;

	// count number of trajectories
	int numberOfTrajectories = 0;
	for (int n = 0; n < N-1; n++) {
		if (Tr[n] != Tr[n + 1]) {
			numberOfTrajectories++;
		}
	}
	
	// determine average displacement size
	double averageDx = 0.0;
	double averageDy = 0.0;
	double averageDz = 0.0;
	double averageDt = 0.0;
	int numberOfDisplacements = 0;
	for (int n = 0; n < N - 1; n++) {
		if (Tr[n] == Tr[n + 1]) {
			averageDx += fabs(X[n] - X[n + 1]);
			averageDy += fabs(Y[n] - Y[n + 1]);
			averageDz += fabs(Z[n] - Z[n + 1]);
			averageDt += fabs(T[n] - T[n + 1]);
			numberOfDisplacements++;
		}
	}
	averageDx /= (double)numberOfDisplacements;
	averageDy /= (double)numberOfDisplacements;
	averageDz /= (double)numberOfDisplacements;
	averageDt /= (double)numberOfDisplacements;

	double effectiveDx = averageDx * averageDx / averageDt;
	double effectiveDy = averageDy * averageDy / averageDt;
	double effectiveDz = averageDz * averageDz / averageDt;

	double optimizationArray[4];
	int iterations[1];
	double fret[1];
	DIM = 4;

	//debugFile = fopen("debug.txt", "w");

	optimizationArray[0] = (effectiveDx + effectiveDy + effectiveDz)/2.0;
	optimizationArray[1] = optimizationArray[2] = optimizationArray[3] = 0.0;

	switch (headerID) {
	case DIFFUSION:
		switch (paramID) {

		case LIKELYHOOD_NO_NOISE:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dlikelihood_no_noise, (dfunc));
			break;

		case LIKELYHOOD_UNIFORM_NOISE:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dlikelihood_uniform_noise, (dfunc));
			break;

		case LIKELYHOOD_ASYMETRIC_NOISE:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dlikelihood_asymetric_noise, (dfunc));
			break;

		case LIKELYHOOD_APPROX_NOISE_ONLY_Z:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dlikelihood_approx_noise_only_z, (dfunc));
			break;

		case POSTERIOR_NO_NOISE_NON_INFORMATIVE_PRIOR:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dposterior_no_noise_non_informative_prior, (dfunc));
			break;

		case POSTERIOR_NO_NOISE_CONGUGATEPRIOR:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dposterior_no_noise_conjugate_prior, (dfunc));
			break;

		case POSTERIOR_UNIFORM_NOISE_NON_INFORMATIVE_PRIOR:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dposterior_uniform_noise_non_informative_prior, (dfunc));
			break;

		case POSTERIOR_ASYMETRIC_NOISE_NON_INFORMATIVE:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dposterior_asymetric_noise_non_informative, (dfunc));
			break;

		case POSTERIOR_APPROX_NOISE_ONLY_Z_NON_INFORMATIVE:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dposterior_approx_noise_only_z_non_informative, (dfunc));
			break;

		default:
			break;
		}
		break;
	case DIFFUSION_VELOCITY:
		switch (paramID) {

		case LIKELYHOOD_NO_NOISE:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dvlikelihood_no_noise, (dfunc));
			break;

		case LIKELYHOOD_UNIFORM_NOISE:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dvlikelihood_uniform_noise, (dfunc));
			break;

		case LIKELYHOOD_ASYMETRIC_NOISE:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dvlikelihood_asymetric_noise, (dfunc));
			break;

		case LIKELYHOOD_APPROX_NOISE_ONLY_Z:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dvlikelihood_approx_noise_only_z, (dfunc));
			break;

		case POSTERIOR_NO_NOISE_NON_INFORMATIVE_PRIOR:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dvposterior_no_noise_non_informative, (dfunc));
			break;

		case POSTERIOR_NO_NOISE_CONGUGATEPRIOR:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dvposterior_no_noise_conjugate_prior, (dfunc));
			break;

		case POSTERIOR_UNIFORM_NOISE_NON_INFORMATIVE_PRIOR:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dvposterior_uniform_noise_non_informative, (dfunc));
			break;

		case POSTERIOR_ASYMETRIC_NOISE_NON_INFORMATIVE:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dvposterior_asymetric_noise_non_informative, (dfunc));
			break;

		case POSTERIOR_APPROX_NOISE_ONLY_Z_NON_INFORMATIVE:
			dfpmin(optimizationArray, DIM, GTOL, iterations, fret, Dvposterior_approx_noise_only_z_non_informative, (dfunc));
			break;

		default:
			break;
		}
		break;	
	default:
		break;
		
	}
	//fprintf(debugFile, "%f\t%f\t%f\t%f\n", optimizationArray[0], optimizationArray[1], optimizationArray[2], optimizationArray[3]);
	//fprintf(debugFile, "%f\t%f\t%f\t%f\n", optimizationArray[0], optimizationArray[1], optimizationArray[2], optimizationArray[3]);
	//fprintf(debugFile, "cost = %f\n", DFxFyFzPosterior(optimizationArray));

	*Diffusion = optimizationArray[0];
	*ForceX = optimizationArray[1];
	*ForceY = optimizationArray[2];
	*ForceZ = optimizationArray[3];

	//fclose(debugFile);
	//fprintf(debugFile, "\n\n\n");

	//double optimizationArrayD[1];
	//optimizationArrayD[0] = (effectiveDx + effectiveDy + effectiveDz) / 2.0;
	//DIM = 1;

	//dfpmin(optimizationArrayD, DIM, GTOL, iterations, fret, DPosterior, (dfunc));
	//fprintf(debugFile, "diffusion = %f\n", optimizationArrayD[0]);
	//fprintf(debugFile, "cost = %f\n", DPosterior(optimizationArray));

	//fprintf(debugFile, "\n\n\n");
	//fprintf(debugFile, "number of trajectories = %i\n", numberOfTrajectories);
	//fprintf(debugFile, "number of points = %i\n", NumberOfPoints);
	//for (int n = 0; n < N; n++) {
	//	fprintf(debugFile, "%f\t%f\t%f\t%f\t%f\n", Tr[n], X[n], Y[n], Z[n], T[n]);
	//}
	//fclose(debugFile);
}
/************************************************************************************/
/************************************************************************************/
/************************************************************************************/



/************************************************************************************/
/************************************************************************************/
/************************************************************************************/




/************************************************************************************/
/************************************************************************************/
/************************************************************************************/
