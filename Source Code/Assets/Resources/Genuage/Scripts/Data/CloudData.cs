/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson, Bassam Hajj and Clement caporal
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
**/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Data
{

    /// <summary>
    /// Class to hold all the Data on one point.
    /// </summary>
    /// 
    public class PointData
    {
        public int _pointID;
        public Vector3 _position;
        public Vector3 _normed_position;
        public float _intensity;
        public float _time;
        public int _frame;
        public float _trajectory;
        public float _size;
        public float _color_index;
        public float _depth;
        public float _phi_angle;
        public float _theta_angle;
        


        #region Get/Setters
        public int pointID
        {
            get { return _pointID; }

            set { _pointID = value; }
        }

        public Vector3 position
        {
            get { return _position; }

            set { _position = value; }
        }

        public Vector3 normed_position
        {
            get { return _normed_position; }

            set { _normed_position = value; }
        }

        public float intensity
        {
            get { return _intensity; }

            set { _intensity = value; }
        }

        public float time
        {
            get { return _time; }
            
            set { _time = value; }
        }
        public int frame
        {
            get { return _frame; }

            set { _frame = value; }
        }

        public float trajectory
        {
            get { return _trajectory; }

            set { _trajectory = value; }
        }

        public float size
        {
            get { return _size; }

            set { _size = value; }
        }


        public float depth
        {
            get { return _depth; }

            set { _depth = value; }
        }
        #endregion

        public float theta_angle
        {
            get { return _theta_angle; }
            set { _theta_angle = value; }
        }
        public float phi_angle
        {
            get { return _phi_angle; }
            set { _phi_angle = value; }
        }
    }

    /// <summary>
    /// Class to hold all the Metadata on one point.
    /// </summary>
    /// 

    public class PointMetaData
    {
        private int _pointID;

        public int trajectoryID;

        public bool isHidden;
        private bool _isSelected;
        public bool trueColorOverride;    

        private int _cloudzone;
        private float _local_density;

        #region Get/Setters
        public int pointID
        {
            get { return _pointID; }

            set { _pointID = value; }
        }

        public bool isSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public int cloudzone
        {
            get { return _cloudzone; }

            set { _cloudzone = value; }
        }

        public float local_density
        {
            get { return _local_density; }

            set { _local_density = value; }
        }
        #endregion
    }


    public class TrajectoryData
    {
        public float trajectoryID;
        public List<int> pointsIDList;
        public List<Vector3> pointsNormedPositionList;
        public List<float> pointsTimeList;
        public Color color;
        public TrajectoryMetaData metadata;

        public TrajectoryData(float id)
        {
            trajectoryID = id;
            pointsIDList = new List<int>();
            pointsNormedPositionList = new List<Vector3>();
            pointsTimeList = new List<float>();
            metadata = new TrajectoryMetaData();
            
        }
    }

    public class TrajectoryMetaData
    {
        public bool isSelected;
        public HashSet<int> selectedpointsIDList;
        public TrajectoryMetaData()
        {
            selectedpointsIDList = new HashSet<int>();
        }
    }


    /// <summary>
    /// Class used to hold metadata specific to one column of the raw data.
    /// </summary>
    /// 
    public class ColumnMetadata
    {
        public int ColumnID;

        public float MinValue;
        public float MaxValue;
        public float Range;
        public float MinThreshold;
        public float MaxThreshold;


    }

    /// <summary>
    /// Class to hold all the Metadata relevant for the whole cloud.
    /// </summary>
    /// 
    public class CloudMetaData
    {
        private int _cloud_id;
        private Vector3 _scale;
        private float _point_size;
        //private float _depth;
        private float _xMax, _xMin; // x position
        private float _yMax, _yMin; // y position
        private float _zMax, _zMin; // z position
        private float _iMax, _iMin; // intensity
        private float _tMax, _tMin; // frame
        private float _dMax, _dMin; // density
        private float _sMax, _sMin; // size


        public float xMinThreshold;
        public float xMaxThreshold;
        public float yMinThreshold;
        public float yMaxThreshold;
        public float zMinThreshold;
        public float zMaxThreshold;
        public float tMinThreshold;
        public float tMaxThreshold;

        public List<ColumnMetadata> columnMetaDataList;

        private Vector3 _box_scale;

        private float _maxRange;
        private float _maxLinkedRange;
        private float _normedxMax;
        private float _normedxMin;
        private float _normedyMax;
        private float _normedyMin;
        private float _normedzMax;
        private float _normedzMin;

        private string _fileName;




        private Vector3 _offsetVector;
        private Vector3 _normed_offsetVector;

        private Vector2 _normed_x;
        private Vector2 _normed_y;
        private Vector2 _normed_z;
        private Vector2 _normed_i;
        private Vector2 _normed_f;
        private Vector2 _normed_d;


        //public float meanPhiAngle;
        //public float meanThetaAngle;


        private Dictionary<int, List<int>> _pointbyLocationList;

        private int _locationMax;

        public bool densityCalculated;

        public HashSet<int> SelectedPointsList;

        public List<float> timeList; //Sorted list of the timepoints in the file 
        public HashSet<float> SelectedTRajectories;

        public float upperframeLimit;
        public float lowerframeLimit;

        public float uppertimeLimit;
        public float lowertimeLimit;


        private string _colormapName;
        public bool colormapReversed;
        //public float cmaxslidervalue;
        //public float cminslidervalue;
        // _current_color_map_variable is used to store the current float list used by the shader to map color. For example depth or intensity
        private Dictionary<int, float> _current_colormap_variable;
        // _current_normed_variable holds min and max of the _current_color_map_variable variables in order to map between (0 and 256) the color map.
        private Vector2 _current_normed_variable;

        // serves as blueprint to know which collumn corresponds to what value, by default should be {0,1,2,3,4}
        public int[] displayCollumnsConfiguration;

        #region VR_Interaction_Metadata


        public Dictionary<int,GameObject> counterPointsList;
        public Dictionary<int, GameObject> rulerPointsList;
        public Dictionary<int, List<float>> rulerPointsDistanceList;
        public Dictionary<int, GameObject> convexHullsList;
        public Dictionary<int, GameObject> sphereList;
        public Dictionary<int, GameObject> angleMeasurementsList;
        public Dictionary<int, GameObject> histogramList;

        public HashSet<int> FreeSelectionIDList;
        public bool FreeSelectionON = true;

        public GameObject[] ClippingPlanesList;

        #endregion




        #region Get/Setters

        public int cloud_id
        {
            get { return _cloud_id; }
            set { _cloud_id = value; }
        }

        public Vector3 scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public string colormapName
        {
            get { return _colormapName; }
            set { _colormapName = value; }
        }

        public float point_size
        {
            get { return _point_size; }
            set { _point_size = value; }
        }

        public float xMax
        {
            get { return _xMax; }
            set { _xMax = value; }
        }

        public float xMin
        {
            get { return _xMin; }
            set { _xMin = value; }
        }

        public float yMax
        {
            get { return _yMax; }
            set { _yMax = value; }
        }

        public float yMin
        {
            get { return _yMin; }
            set { _yMin = value; }
        }

        public float zMax
        {
            get { return _zMax; }
            set { _zMax = value; }
        }

        public float zMin
        {
            get { return _zMin; }
            set { _zMin = value; }
        }

        public float iMax
        {
            get { return _iMax; }
            set { _iMax = value; }
        }

        public float iMin
        {
            get { return _iMin; }
            set { _iMin = value; }
        }

        public float tMax
        {
            get { return _tMax; }
            set { _tMax = value; }
        }

        public float tMin
        {
            get { return _tMin; }
            set { _tMin = value; }
        }

        public float sMax
        {
            get { return _sMax; }
            set { _sMax = value; }
        }

        public float sMin
        {
            get { return _sMin; }
            set { _sMin = value; }
        }


        public float dMax
        {
            get { return _dMax; }
            set { _dMax = value; }
        }

        public float dMin
        {
            get { return _dMin; }
            set { _dMin = value; }
        }

        public float normed_xMax
        {
            get { return _normedxMax; }
            set { _normedxMax = value; }
        }

        public float normed_xMin
        {
            get { return _normedxMin; }
            set { _normedxMin = value; }
        }

        public float normed_yMax
        {
            get { return _normedyMax; }
            set { _normedyMax = value; }
        }

        public float normed_yMin
        {
            get { return _normedyMin; }
            set { _normedyMin = value; }
        }

        public float normed_zMax
        {
            get { return _normedzMax; }
            set { _normedzMax = value; }
        }

        public float normed_zMin
        {
            get { return _normedzMin; }
            set { _normedzMin = value; }
        }

        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public Vector2 normed_x
        {
            get { return _normed_x; }
            set { _normed_x = value; }
        }

        public Vector2 normed_y
        {
            get { return _normed_y; }
            set { _normed_y = value; }
        }

        public Vector2 normed_z
        {
            get { return _normed_z; }
            set { _normed_z = value; }
        }

        public Vector2 normed_i
        {
            get { return _normed_i; }
            set { _normed_i = value; }
        }

        public Vector2 normed_t
        {
            get { return _normed_f; }
            set { _normed_f = value; }
        }

        public Vector2 normed_d
        {
            get { return _normed_d; }
            set { _normed_d = value; }
        }


        public Dictionary<int, List<int>> pointbyLocationList
        {
            get { return _pointbyLocationList; }
            set { _pointbyLocationList = value; }
        }

        public int locationMax
        {
            get { return _locationMax; }
            set { _locationMax = value; }
        }

        public Dictionary<int, float> current_colormap_variable
        {
            get { return _current_colormap_variable; }
            set { _current_colormap_variable = value; }
        }

        public Vector2 current_normed_variable
        {
            get { return _current_normed_variable; }
            set { _current_normed_variable = value; }
        }

        public Vector3 offsetVector
        {
            get { return _offsetVector; }
            set { _offsetVector = value; }
        }

        public Vector3 normed_offsetVector
        {
            get { return _normed_offsetVector; }
            set { _normed_offsetVector = value; }
        }

        public float maxRange
        {
            get { return _maxRange; }
            set { _maxRange = value; }
        }

        public float maxLinkedRange
        {
            get { return _maxLinkedRange; }
            set { _maxLinkedRange = value; }
        }

        public Vector3 box_scale
        {
            get { return _box_scale; }
            set { _box_scale = value; }
        }
        #endregion


    }

    /// <summary>
    /// CloudData holds all the data and metadata for one cloud.
    /// </summary>
    /// 
    public class CloudData : MonoBehaviour
    {


        public List<float[]> columnData;
        public Dictionary<int, PointData> pointDataTable;
        public Dictionary<int, PointMetaData> pointMetaDataTable;
        public CloudMetaData globalMetaData;
        public Dictionary<float, TrajectoryData> pointTrajectoriesTable;

        public GameObject trajectoryObject = null;
        public GameObject orientationObject = null;

        private void Awake()
        {
            pointDataTable = new Dictionary<int, PointData>();
            pointMetaDataTable = new Dictionary<int, PointMetaData>();
            globalMetaData = new CloudMetaData();

            pointTrajectoriesTable = new Dictionary<float, TrajectoryData>();

            globalMetaData.pointbyLocationList = new Dictionary<int, List<int>>();

            globalMetaData.SelectedPointsList = new HashSet<int>();
            globalMetaData.SelectedTRajectories = new HashSet<float>();
            globalMetaData.columnMetaDataList = new List<ColumnMetadata>();
            globalMetaData.counterPointsList = new Dictionary<int, GameObject>();
            globalMetaData.convexHullsList = new Dictionary<int, GameObject>();
            globalMetaData.sphereList = new Dictionary<int, GameObject>();
            globalMetaData.rulerPointsList = new Dictionary<int, GameObject>();
            globalMetaData.rulerPointsDistanceList = new Dictionary<int, List<float>>();
            globalMetaData.angleMeasurementsList = new Dictionary<int, GameObject>();
            globalMetaData.histogramList = new Dictionary<int, GameObject>();
            globalMetaData.FreeSelectionIDList = new HashSet<int>();
        }

        public void CreatePointData(int id, Vector3 position, Vector3 normedposition, float intensity,
            float time ,float trajectory, Color color, float depth )
        {
            if (!pointDataTable.ContainsKey(id))
            {
                PointData newdata = new PointData();
                newdata.pointID = id;
                newdata.position = position;
                newdata.normed_position = normedposition;
                newdata.intensity = intensity;
                newdata.time = time;
                newdata.trajectory = trajectory;
                newdata.depth = depth;
;

                pointDataTable.Add(id, newdata);

            }
            else
            {
                Debug.Log("id" + id);
                Debug.Log("Tried creating a point with an id already existing in the pointTable, this should not happen.");
            }
        }

        public void CreatePointMetaData(int id, int cloudzone = 0, float local_density = 0, int trajectoryID = -1)
        {
            if (!pointMetaDataTable.ContainsKey(id))
            {
                PointMetaData newdata = new PointMetaData();
                newdata.pointID = id;
                newdata.trajectoryID = trajectoryID;
                newdata.isHidden = false;
                newdata.isSelected = false;
                newdata.trueColorOverride = false;
                newdata.cloudzone = cloudzone;
                newdata.local_density = local_density;
                pointMetaDataTable.Add(id, newdata);
            }
            else
            {
                Debug.Log("id" + id);
                Debug.Log("Tried creating a point with an id already existing in the pointMetadataTable, this should not happen.");
            }

        }

        public void InitGlobalCloudConstant(float xmax, float xmin, float ymax, float ymin, float zmax,
            float zmin, float imax, float imin, float tmax, float tmin, float dMax = 10000000.0f,
            float dMin = 0.0f)
        {
            globalMetaData.normed_x = new Vector2(xmin, xmax);
            globalMetaData.normed_y = new Vector2(ymin, ymax);
            globalMetaData.normed_z = new Vector2(zmin, zmax);
            globalMetaData.normed_i = new Vector2(imin, imax);
            globalMetaData.normed_t = new Vector2(tmin, tmax);

            globalMetaData.xMax = xmax;
            globalMetaData.xMin = xmin;
            globalMetaData.yMax = ymax;
            globalMetaData.yMin = ymin;
            globalMetaData.zMax = zmax;
            globalMetaData.zMin = zmin;
            globalMetaData.iMax = imax;
            globalMetaData.iMin = imin;
            globalMetaData.tMax = tmax;
            globalMetaData.tMin = tmin;

            globalMetaData.xMaxThreshold = xmax;
            globalMetaData.xMinThreshold = xmin;
            globalMetaData.yMaxThreshold = ymax;
            globalMetaData.yMinThreshold = ymin;
            globalMetaData.zMaxThreshold = zmax;
            globalMetaData.zMinThreshold = zmin;
            globalMetaData.tMaxThreshold = tmax;
            globalMetaData.tMinThreshold = tmin;


            globalMetaData.scale = Vector3.one;

            globalMetaData.densityCalculated = false;

            globalMetaData.timeList = new List<float>();

            Dictionary<int, float> colorDict = new Dictionary<int, float>();

            foreach (KeyValuePair<int, PointData> item in this.pointDataTable)
            {
                colorDict.Add(item.Key, item.Value.intensity);
            }
            globalMetaData.current_normed_variable = globalMetaData.normed_i;
            globalMetaData.current_colormap_variable = colorDict;

        }

        


    }
}
    