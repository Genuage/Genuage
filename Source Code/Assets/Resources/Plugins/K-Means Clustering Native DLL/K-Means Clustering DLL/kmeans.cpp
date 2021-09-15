#include "pch.h"
#include <omp.h>
#include <algorithm>
#include <cmath>
#include <fstream>
#include <vector>
#include "kmeans.h"
using namespace std;

class Point
{
private:
    int pointId, clusterId;
    int dimensions;
    vector<double> values;


public:
    Point(int id, double x, double y, double z)
    {
        pointId = id;
        
        values.push_back(x);
        values.push_back(y);
        values.push_back(z);
        //values = lineToVec(line);
        dimensions = values.size();
        clusterId = 0; // Initially not assigned to any cluster
    }

    int getDimensions() { return dimensions; }

    int getCluster() { return clusterId; }

    int getID() { return pointId; }

    void setCluster(int val) { clusterId = val; }

    double getVal(int pos) { return values[pos]; }
};

class Cluster
{
private:
    int clusterId;
    vector<double> centroid;
    vector<Point> points;

public:
    Cluster(int clusterId, Point ext_centroid)
    {
        this->clusterId = clusterId;
        for (int i = 0; i < ext_centroid.getDimensions(); i++)
        {
            this->centroid.push_back(ext_centroid.getVal(i));
        }
        this->addPoint(ext_centroid);
    }

    void addPoint(Point p)
    {
        p.setCluster(this->clusterId);
        points.push_back(p);
    }

    bool removePoint(int pointId)
    {
        int size = points.size();

        for (int i = 0; i < size; i++)
        {
            if (points[i].getID() == pointId)
            {
                points.erase(points.begin() + i);
                return true;
            }
        }
        return false;
    }

    void removeAllPoints() { points.clear(); }

    int getId() { return clusterId; }

    Point getPoint(int pos) { return points[pos]; }

    int getSize() { return points.size(); }

    double getCentroidByPos(int pos) { return centroid[pos]; }

    void setCentroidByPos(int pos, double val) { this->centroid[pos] = val; }
};

class KMeans
{
private:
    int K, iters, dimensions, total_points;
    vector<Cluster> clusters;
    string output_dir;

    void clearClusters()
    {
        for (int i = 0; i < K; i++)
        {
            clusters[i].removeAllPoints();
        }
    }

    int getNearestClusterId(Point point)
    {
        double sum = 0.0, min_dist;
        int NearestClusterId;

        for (int i = 0; i < dimensions; i++)
        {
            sum += pow(clusters[0].getCentroidByPos(i) - point.getVal(i), 2.0);
            // sum += abs(clusters[0].getCentroidByPos(i) - point.getVal(i));
        }

        min_dist = sqrt(sum);
        // min_dist = sum;
        NearestClusterId = clusters[0].getId();

        for (int i = 1; i < K; i++)
        {
            double dist;
            sum = 0.0;

            for (int j = 0; j < dimensions; j++)
            {
                sum += pow(clusters[i].getCentroidByPos(j) - point.getVal(j), 2.0);
                // sum += abs(clusters[i].getCentroidByPos(j) - point.getVal(j));
            }

            dist = sqrt(sum);
            // dist = sum;

            if (dist < min_dist)
            {
                min_dist = dist;
                NearestClusterId = clusters[i].getId();
            }
        }

        return NearestClusterId;
    }

public:
    KMeans(int cluster_nbr, int iterations)
    {
        this->K = cluster_nbr;
        this->iters = iterations;
    }

    void run(vector<Point> &all_points, int* points_result,
             double* cluster_centroids_results_x, double* cluster_centroids_results_y, double* cluster_centroids_results_z)
    {
        total_points = all_points.size();
        dimensions = all_points[0].getDimensions();

        // Initializing Clusters
        vector<int> used_pointIds;

        for (int i = 1; i <= K; i++)
        {
            while (true)
            {
                int index = rand() % total_points;

                if (find(used_pointIds.begin(), used_pointIds.end(), index) ==
                    used_pointIds.end())
                {
                    used_pointIds.push_back(index);
                    all_points[index].setCluster(i);
                    Cluster cluster(i, all_points[index]);
                    clusters.push_back(cluster);
                    break;
                }
            }
        }
        
        int iter = 1;
        while (true)
        {
            bool done = true;

            // Add all points to their nearest cluster
            #pragma omp parallel for reduction(&&: done) num_threads(16)
            for (int i = 0; i < total_points; i++)
            {
                int currentClusterId = all_points[i].getCluster();
                int nearestClusterId = getNearestClusterId(all_points[i]);

                if (currentClusterId != nearestClusterId)
                {
                    all_points[i].setCluster(nearestClusterId);
                    done = false;
                }
            }

            // clear all existing clusters
            clearClusters();

            // reassign points to their new clusters
            for (int i = 0; i < total_points; i++)
            {
                // cluster index is ID-1
                clusters[all_points[i].getCluster() - 1].addPoint(all_points[i]);
            }

            // Recalculating the center of each cluster
            for (int i = 0; i < K; i++)
            {
                int ClusterSize = clusters[i].getSize();

                for (int j = 0; j < dimensions; j++)
                {
                    double sum = 0.0;
                    if (ClusterSize > 0)
                    {
                        #pragma omp parallel for reduction(+: sum) num_threads(16)
                        for (int p = 0; p < ClusterSize; p++)
                        {
                            sum += clusters[i].getPoint(p).getVal(j);
                        }
                        clusters[i].setCentroidByPos(j, sum / ClusterSize);
                    }
                }
            }

            if (done || iter >= iters)
            {

                break;
            }
            iter++;
        }
        //Save Data

        for (int i = 0; i < total_points; i++)
        {
            points_result[i] = all_points[i].getCluster();
        }
        
        for (int i = 0; i < K; i++)
        {
            cluster_centroids_results_x[i] = clusters[i].getCentroidByPos(0);
            cluster_centroids_results_y[i] = clusters[i].getCentroidByPos(1);
            cluster_centroids_results_z[i] = clusters[i].getCentroidByPos(2);
        }
        
    }
};

void LaunchKMeans(int point_nbr, int cluster_nbr, int iteration_nbr, 
            double* x_coordinates, double* y_coordinates, double* z_coordinates, 
            int* points_result, 
            double* cluster_centroids_results_x, double* cluster_centroids_results_y, double* cluster_centroids_results_z)
{
    

    // Fetching points from file
    int pointId = 1;
    vector<Point> all_points;
    //string line;

    for (int i = 0; i < point_nbr; i++)
    {
        Point point(pointId, x_coordinates[i], y_coordinates[i], z_coordinates[i]);
        all_points.push_back(point);
        pointId++;
    }


    

    // Running K-Means Clustering
    int iters = iteration_nbr;

    KMeans kmeans(cluster_nbr, iters);
    kmeans.run(all_points, points_result,
        cluster_centroids_results_x, cluster_centroids_results_y, cluster_centroids_results_z);

    
}
