#pragma once
void LaunchKMeans(int point_nbr, int cluster_nbr, int iteration_nbr,
    double* x_coordinates, double* y_coordinates, double* z_coordinates,
    int* points_result,
    double* cluster_centroids_results_x, double* cluster_centroids_results_y, double* cluster_centroids_results_z);
